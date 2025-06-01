using App;
using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests;

public class CollisionPrepareDataCommandTests
{
    public CollisionPrepareDataCommandTests()
    {
        new InitCommand().Execute();
        var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void Execute_GeneratesCollisionData_AndCallsSaveCommand()
    {
        dynamic mockDataGenerator = new Mock<object>().Object;

        var mockData = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
        mockData["firstShape"] = "ShapeA";
        mockData["secondShape"] = "ShapeB";
        mockData["GenerateCollisionData"] = (Func<IList<int[]>>)(() => new List<int[]> { new int[] { 1, 2, 3, 4 } });

        mockDataGenerator = mockData;

        var mockSaveCommand = new Mock<App.ICommand>();

        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Collision.DataSaveCommand",
            (object[] args) => mockSaveCommand.Object
        ).Execute();

        var regCollisisonPrepareData = new RegisterIoCDependencyCollisionPrepareData();
        regCollisisonPrepareData.Execute();
        var regCollisionName = new RegisterIoCDependencyGetCollisionName();
        regCollisionName.Execute();

        var collisionPrepareDataCommand = Ioc.Resolve<App.ICommand>("Collision.PrepareData", mockDataGenerator);

        collisionPrepareDataCommand.Execute();

        mockSaveCommand.Verify(c => c.Execute(), Times.Once);
    }
}
