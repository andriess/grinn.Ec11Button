namespace Grinn.Ec11Button.UnitTests.Commands;

public class GetQueueInfoTests
{
    private GetQueueInfo _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GetQueueInfo();
    }
    
    [Test]
    public void CommandName_ShouldReturnPlaylistInfo()
    {
       _sut.CommandName.Should().Be("playlistinfo");
    }

    [Test]
    public void ParseCommandResponse_WithOnePlaylistItem_ShouldReturnOnePlayListItem()
    {
        const string mpdResponse = "file: http://uk2.internet-radio.com:8024/\nTitle: D'Angello & Francis Feat. Belle Humble - Gold\nName: Dance UK Radio danceradiouk aac+\nPos: 0\nId: 1\nOK";

        var result = _sut.ParseCommandResponse(mpdResponse);

        result.Count.Should().Be(1);
        result[0].Path.Should().Be("http://uk2.internet-radio.com:8024/");
        result[0].Id.Should().Be(1);
        result[0].Position.Should().Be(0);
        result[0].Title.Should().Be("D'Angello & Francis Feat. Belle Humble - Gold");
        result[0].Name.Should().Be("Dance UK Radio danceradiouk aac+");
    }

    [Test]
    public void ParseCommandResponse_WithThreePlayListItems_ShouldReturnMultiplePlayListItems()
    {
        const string mpdResponse =
            "file: http://uk2.internet-radio.com:8024/\nTitle: D'Angello & Francis Feat. Belle Humble - Gold\nName: Dance UK Radio danceradiouk aac+\nPos: 0\nId: 1\nfile: http://stream-sd.radioparadise.com:8056\nPos: 1\nId: 2\nfile: https://liveaudio.rte.ie/hls-radio/gold/\nPos: 2\nId: 3\nOK";        
       
        var result = _sut.ParseCommandResponse(mpdResponse);

        result.Count.Should().Be(3);
        result[0].Path.Should().Be("http://uk2.internet-radio.com:8024/");
        result[0].Id.Should().Be(1);
        result[0].Position.Should().Be(0);
        result[0].Title.Should().Be("D'Angello & Francis Feat. Belle Humble - Gold");
        result[0].Name.Should().Be("Dance UK Radio danceradiouk aac+");
        result[1].Path.Should().Be("http://stream-sd.radioparadise.com:8056");
        result[1].Id.Should().Be(2);
        result[1].Position.Should().Be(1);
        result[1].Title.Should().BeEmpty();
        result[1].Name.Should().BeEmpty();
        result[2].Path.Should().Be("https://liveaudio.rte.ie/hls-radio/gold/");
        result[2].Id.Should().Be(3);
        result[2].Position.Should().Be(2);
        result[2].Title.Should().BeEmpty();
        result[2].Name.Should().BeEmpty();
    }
}