using App;
namespace SpaceWar_workspace;

public class RegisterIoCDependencyGetCollisionName : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Collision.GetCollisionName",
                (object[] arg) => (string)arg[0] + "_" + (string)arg[1]).Execute();
    }
}
