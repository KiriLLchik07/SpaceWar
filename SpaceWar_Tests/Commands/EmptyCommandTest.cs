using SpaceWar_workspace;

namespace SpaceWar_Tests;
public class EmptyCommandTest
{
    [Fact]
    public void EmptyCommand_ExecuteDoesNotThrow()
    {
        var command = new EmptyCommand();
        Assert.Null(Record.Exception(() => command.Execute()));
    }
}
