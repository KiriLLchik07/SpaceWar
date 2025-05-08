using App;

namespace SpaceWar_workspace
{
    public class IoCRegisterGameOperation : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Receiver", (object[] args) =>
            {
                Queue<ICommand> queue;
                try
                {
                    queue = Ioc.Resolve<Queue<ICommand>>("Game.Queue");
                }
                catch (System.Exception)
                {
                    queue = new Queue<ICommand>();
                    Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue).Execute();
                }

                return new GameReceiver(queue);
            }).Execute();
        }
    }
}
