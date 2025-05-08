namespace SpaceWar_workspace
{
    public class IoCRegisterGameOperation : App.ICommand
    {
        public void Execute()
        {
            App.Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Receiver", (object[] args) =>
            {
                Queue<ICommand> queue;
                try
                {
                    queue = App.Ioc.Resolve<Queue<ICommand>>("Game.Queue");
                }
                catch (System.Exception)
                {
                    queue = new Queue<ICommand>();
                    App.Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue).Execute();
                }

                return new GameReceiver(queue);
            }).Execute();
        }
    }
}
