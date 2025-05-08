using System.Diagnostics;
using App;
namespace SpaceWar_workspace;

public class Game : App.ICommand
{
    private readonly object _game_scope;
    public Game(object game_scope)
    {
        _game_scope = game_scope;
    }

    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _game_scope).Execute();

        var queue = Ioc.Resolve<Queue<App.ICommand>>("Game.CommandsQueue");

        var time_quant = Ioc.Resolve<TimeSpan>("Game.TimeQuant");

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
                Ioc.Resolve<App.ICommand>("ExceptionHandler.Handle", cmd, ex).Execute();
            }
        }
    }
}
