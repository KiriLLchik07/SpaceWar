using System.Diagnostics;

namespace SpaceWar_workspace;

public class Game : ICommand
{
    private readonly object _gameScope;

    public Game(object gameScope)
    {
        _gameScope = gameScope;
    }

    public void Execute()
    {
        bool continuePlaying;

        do
        {
            var stopwatch = Stopwatch.StartNew();

            IoC.Resolve<ICommand>("Scopes.Current.Set", _gameScope).Execute();

            Func<long> getElapsed = () => stopwatch.ElapsedMilliseconds;

            while (IoC.Resolve<bool>("Game.CanContinue", getElapsed()))
            {
                IoC.Resolve<Action>("Game.GameBehaviour")();
            }

            stopwatch.Stop();

            Console.WriteLine("Игра окончена. Хотите сыграть еще раз? (y/n)");
            var input = Console.ReadLine()?.Trim().ToLower();
            continuePlaying = input == "y";

        } while (continuePlaying);
    }
}
