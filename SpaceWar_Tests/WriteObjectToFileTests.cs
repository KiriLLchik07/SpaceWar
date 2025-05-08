using System.Text.Json;
using App;
using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests;

public class WriteObjectToFileCommandTests
{
    public WriteObjectToFileCommandTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void Execute_WritesSerializedJsonToFile()
    {
        var data = new List<int[]> { new[] { 1, 2 }, new[] { 3, 4 } };
        var path = Path.GetTempFileName();

        new RegisterIoCDependencyWriteObjectToFileCommand().Execute();
        var command = Ioc.Resolve<App.ICommand>("Commands.WriteObjectToFile", path, data);
        command.Execute();

        var readText = File.ReadAllText(path);
        var deserialized = JsonSerializer.Deserialize<List<int[]>>(readText);

        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized!.Count);
        File.Delete(path);
    }
}
