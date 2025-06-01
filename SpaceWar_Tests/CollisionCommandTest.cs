using App;
using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class CollisionCommandTests : IDisposable
    {
        private readonly Dictionary<int, object> collisionTree = new Dictionary<int, object>(){
                {1,new Dictionary<int, object>(){
                    {1,new Dictionary<int, object>(){
                        {0, new Dictionary<int, object>(){
                            {0, new Dictionary<int, object>()}
                        }}
                    }}
                }}
            };

        private readonly Dictionary<(string, string), string> collisionRules = new Dictionary<(string, string), string>
        {
            { ("Torpedo", "Ship"), "Ship" },
            { ("Ship", "Asteroid"), "Ship" }
        };

        public CollisionCommandTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.GetPosition", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (int[])obj["Position"];
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.GetVelocity", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (int[])obj["Velocity"];
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.GetType", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (string)obj["Type"];
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.ReferenceRules", (object[] args) =>
            {
                return collisionRules;
            }).Execute();

            var registerCollDependencies = new RegisterIoCCollisionCommand();

            registerCollDependencies.Execute();

        }

        [Fact]
        public void CollisionCommand_CollisionTrueTest()
        {
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            var command = new CollisionCommand(obj1.Object, obj2.Object);

            command.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Once());
        }

        [Fact]
        public void CollisionCommand_CollisionFalseTest()
        {
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 3 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            var command = new CollisionCommand(obj1.Object, obj2.Object);

            command.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Never());
        }

        [Fact]
        public void CheckCollisionExists_TrueForValidPath()
        {
            var tree = new Dictionary<int, object>
            {
                {1, new Dictionary<int, object>
                    {
                        {2, new Dictionary<int, object>
                            {
                                {3, new object()}
                            }
                        }
                    }
                }
            };

            var path = new[] { 1, 2, 3 };
            var result = CollisionCommand.CheckCollisionExists(tree, path);
            Assert.True(result);
        }

        [Fact]
        public void CheckCollisionExists_FalseForInvalidPath()
        {
            var tree = new Dictionary<int, object>
            {
                {1, new Dictionary<int, object>()}
            };

            var path = new[] { 1, 2, 3 };
            var result = CollisionCommand.CheckCollisionExists(tree, path);
            Assert.False(result);
        }

        [Fact]
        public void CollisionCommandException_NonValideCollisionTree()
        {
            var newCollisionTree = new Dictionary<int, object>(){
                { 0, new Dictionary<int, object>(){
                    {0, "not a dictionary"}
                }}
            };

            Init_CollisionTree(newCollisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 3 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            var commandCollision = new CollisionCommand(obj1.Object, obj2.Object);

            commandCollision.Execute();
            collisionHandle.Verify(c => c.Execute(), Times.Never());
        }

        [Fact]
        public void CheckCollisionExists_EmptyPath_ReturnsFalse()
        {
            var result = CollisionCommand.CheckCollisionExists(new Dictionary<int, object>(), Array.Empty<int>());
            Assert.False(result);
        }

        [Fact]
        public void CollisionCommand_ReversePairRule()
        {
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Type"]).Returns("Ship");
            obj2.Setup(obj => obj["Type"]).Returns("Torpedo");

            var commandCollision = new CollisionCommand(obj1.Object, obj2.Object);

            commandCollision.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Once());
        }

        [Fact]
        public void CollisionCommand_True_NotRules()
        {
            Init_CollisionTree(collisionTree, "AsteroidTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });

            obj1.Setup(obj => obj["Type"]).Returns("Asteroid");
            obj2.Setup(obj => obj["Type"]).Returns("Torpedo");

            var commandCollision = new CollisionCommand(obj1.Object, obj2.Object);

            commandCollision.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Once());
        }

        [Fact]
        public void CollisionCommand_False_NullParams()
        {
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<App.ICommand>();
            collisionHandle.Setup(c => c.Execute());

            Ioc.Resolve<App.ICommand>("IoC.Register", "Collision.Handle", (object[] args) => collisionHandle.Object).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            var commandCollision = new CollisionCommand(obj1.Object, obj2.Object);

            commandCollision.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Never());
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
        private static void Init_CollisionTree(IDictionary<int, object> collisionTree, string treeKey)
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", $"Collision.Tree.{treeKey}", (object[] args) => collisionTree).Execute();
        }
    }
}
