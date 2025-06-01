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
        try
        {
            using var stream = File.Create(filePath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(stream, objectToWrite, options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при записи в файл {filePath}", ex);
        }
    }
}
