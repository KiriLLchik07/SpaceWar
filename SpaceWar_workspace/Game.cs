using System.Diagnostics;
using App;

namespace SpaceWar_workspace
{
    public class Game : App.ICommand
    {
        private readonly object _scope;
        private readonly Stopwatch _stopwatch;

        public Game(object scope)
        {
            _scope = scope;
            _stopwatch = new Stopwatch();
        }

        public void Execute()
        {
            _stopwatch.Reset();
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _scope).Execute();
            var commandTimeLimit = Ioc.Resolve<TimeSpan>("Command.Time");

            while (Ioc.Resolve<Func<int>>("Game.Queue.Count")() > 0 && _stopwatch.Elapsed <= commandTimeLimit)
            {
                try
                {
                    _stopwatch.Start();
                    var cmd = Ioc.Resolve<App.ICommand>("Game.Queue.Take");
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    var currentCmd = Ioc.Resolve<App.ICommand>("Game.Queue.Current");
                    Ioc.Resolve<App.ICommand>("ExceptionHandler", ex, currentCmd).Execute();
                }
                finally
                {
                    _stopwatch.Stop();
                }
            }
        }
    }
}