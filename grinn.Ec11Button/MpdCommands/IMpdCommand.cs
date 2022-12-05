namespace grinn.Ec11Button.MpdCommands;

public interface IMpdCommand<T>
{
    string CommandName { get; }

    IList<T> ParseCommandResponse(string response);
}