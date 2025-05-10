namespace SpaceWar_workspace
{
    public class RegisterIoCDependencySendCommand : ICommand
    {
        public void Execute()
        {
            IoC.Resolve<ICommand>("IoC.Register", "Commands.Send",
                (object[] args) => new SendCommand((ICommand)args[0], (ICommandReceiver)args[1])
            ).Execute();
        }
    }
}
