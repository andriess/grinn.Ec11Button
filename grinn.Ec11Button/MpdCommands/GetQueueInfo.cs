
namespace grinn.Ec11Button.MpdCommands;

public class GetQueueInfo : IMpdCommand<PlayListItem>
{
    public string CommandName => "playlistinfo";
    
    public IList<PlayListItem> ParseCommandResponse(string response)
    {
        var playlistItemBuilder = PlayListItemBuilder.CreateBuilder();
        var itemDictionary = new Dictionary<string, string>();
        var playlistItems = new List<PlayListItem>();
        
        var lines = response.Split("\n");
        for(var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Equals("OK"))
            {
                // when we reach the OK at the end of the response map whatever is in the dict to a playlistitem.
                playlistItems.Add(playlistItemBuilder.MapDictToObject(itemDictionary));
                break;
            }

            var values =  lines[i].Split(":", 2);

            // The mpd structure returned is something like this: file, pos, id, .. file, pos, id, ..
            // so I add values to the dictionary until it reaches the second 'file'. Then map it onto
            // a item object.
            var key = values[0];
            var value = values[1].Trim();
            if (itemDictionary.TryAdd(key, value)) continue;
                
            playlistItems.Add(playlistItemBuilder.MapDictToObject(itemDictionary));

            playlistItemBuilder = PlayListItemBuilder.CreateBuilder();
            itemDictionary.Clear();
            itemDictionary.Add(key, value);
        }

        return playlistItems;
    }
}

public class PlayListItemBuilder
{
    private PlayListItem _item;

    private PlayListItemBuilder(PlayListItem item)
    {
        _item = item; 
    }

    public static PlayListItemBuilder CreateBuilder()
    {
        return new PlayListItemBuilder(new PlayListItem());
    }

    public PlayListItem MapDictToObject(Dictionary<string, string> dictionary)
    {
        foreach (var valuePair in dictionary)
        {
            switch (valuePair.Key.ToLower())
            {
                case "file":
                    _item.Path = valuePair.Value;
                    break;
                case "id":
                    _item.Id = int.Parse(valuePair.Value);
                    break;
                case "pos":
                    _item.Position = int.Parse(valuePair.Value);
                    break;
                case "name":
                    _item.Name = valuePair.Value;
                    break;
                case "title":
                    _item.Title = valuePair.Value;
                    break;
            }
        }

        return _item;
    }
}

public class PlayListItem
{
    public string Path { get; set; }
    public int Id { get; set; }
    public int Position { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }

    public override string ToString()
    {
        return $"{nameof(PlayListItem)} - Path: {Path}, Id: {Id}, Position: {Position}, Name: {Name}, Title: {Title}";
    }
}