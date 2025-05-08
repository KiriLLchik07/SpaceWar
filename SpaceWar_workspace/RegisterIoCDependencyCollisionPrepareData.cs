using App;
namespace SpaceWar_workspace;

public class RegisterIoCDependencyCollisionPrepareData : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Collision.PrepareData",
                (object[] arg) => new CollisionPrepareDataCommand((ICollisionDataGenerator)arg[0])).Execute();
    }
}
