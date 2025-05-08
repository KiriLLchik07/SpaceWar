using System.Text.Json;
namespace SpaceWar_workspace;

public class WriteObjectToFileCommand : ICommand
{
    private readonly object objectToWrite;
    private readonly string filePath;

    public WriteObjectToFileCommand(string filePath, object objectToWrite)
    {
        this.objectToWrite = objectToWrite;
        this.filePath = filePath;
    }

    public void Execute()
    {
        var jsonString = JsonSerializer.Serialize(objectToWrite);
        File.WriteAllText(filePath, jsonString);
    }
}
