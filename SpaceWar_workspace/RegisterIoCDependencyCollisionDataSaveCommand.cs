using App;
namespace SpaceWar_workspace;

public class RegisterIoCDependencyCollisionDataSaveCommand : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Collision.DataSaveCommand",
                (object[] arg) => new CollisionDataSaveCommand((string)arg[0], (IList<int[]>)arg[1])).Execute();
    }
}
