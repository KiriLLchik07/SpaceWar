using App;

namespace SpaceWar_workspace;

public class CollisionPrepareDataCommand : App.ICommand
{
    private readonly object dataGenerator;
    public CollisionPrepareDataCommand(object dataGenerator)
    {
        this.dataGenerator = dataGenerator;
    }

    public void Execute()
    {
        dynamic generator = dataGenerator;

        var collisionName = Ioc.Resolve<string>("Collision.GetCollisionName", generator.firstShape, generator.secondShape);
        var collisionDataSaveCommand = Ioc.Resolve<App.ICommand>("Collision.DataSaveCommand", collisionName, generator.GenerateCollisionData());
        collisionDataSaveCommand.Execute();
    }
}
