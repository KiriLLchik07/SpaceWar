using System.Diagnostics;
using App;
namespace SpaceWar_workspace;

public class Game : ICommand
{
    private readonly object _game_scope;
    public Game(object game_scope)
    {
        _game_scope = game_scope;
    }

    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _game_scope).Execute();

        var queue = Ioc.Resolve<Queue<ICommand>>("Game.CommandsQueue");

        var time_quant = (TimeSpan)Ioc.Resolve<object>("Game.TimeQuant");

        var timer = Stopwatch.StartNew();

        while ((timer.Elapsed < time_quant) && (queue.Count > 0))
        {
            var cmd = queue.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
            }
        }
    }
}
