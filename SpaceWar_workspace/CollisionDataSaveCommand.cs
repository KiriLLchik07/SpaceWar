using App;

namespace SpaceWar_workspace;

public class CollisionDataSaveCommand : App.ICommand
{
    private readonly IList<int[]> collisionData;
    private readonly string collisionName;

    public CollisionDataSaveCommand(string collisionName, IList<int[]> collisionData)
    {
        this.collisionData = collisionData;
        this.collisionName = collisionName;
    }
    public void Execute()
    {
        var pathForCollisionFile = Ioc.Resolve<string>("Data.CollisionFilesPath");
        Ioc.Resolve<App.ICommand>("Commands.WriteObjectToFile", pathForCollisionFile + collisionName, collisionData).Execute();
        Ioc.Resolve<App.ICommand>("Collision.LoadDataToMemory", collisionName, collisionData).Execute();
    }
}
