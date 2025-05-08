using App;

namespace SpaceWar_workspace;

public class RegisterIoCDependencyWriteObjectToFileCommand : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Commands.WriteObjectToFile",
                (object[] arg) => new WriteObjectToFileCommand((string)arg[0], arg[1])).Execute();
    }
}