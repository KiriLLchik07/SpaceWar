using SpaceWar_workspace;
using Hwdtech.Ioc;

namespace SpaceWar_Tests
{
    public class CollisionCommandTests
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

        private static void Init_CollisionTree(IDictionary<int, object> collisionTree, string treeKey)
        {
            IoC.Resolve<ICommand>("IoC.Register", $"Game.CollisionTree.{treeKey}", (object[] args) => collisionTree).Execute();
        }

        public CollisionCommandTests() 
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.GetPosition", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (int[])obj["Position"];
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.GetVelocity", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (int[])obj["Velocity"];
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Game.GetType", (object[] args) =>
            {
                var obj = (IDictionary<string, object>)args[0];
                return (string)obj["Type"];
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Get.Collision.ReferenceRules", (object[] args) =>
            {
                return collisionRules;
            }).Execute();

            var registerCollDependencies = new RegisterIoCCollisionCommand();

            registerCollDependencies.Execute();
        }

        [Fact]
        public void CollisionCommand_ShouldNotHandleCollision_WhenObjectsFarApart()
        {
            
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<ICommand>();
            collisionHandle.Setup(c => c.Execute());

            var registerCommand = new RegisterIoCCollisionCommand();
            registerCommand.Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register", 
                "Collision.Handle", 
                (object[] args) => collisionHandle.Object
            ).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 0, 0 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 100, 100 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 0, 0 });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.ReferenceRules",
                (object[] args) => new Dictionary<(string, string), string>
                {
                    { ("Torpedo", "Ship"), "Torpedo" }
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.GetState",
                (object[] args) => new Tuple<int[], string>(new int[0], "empty_tree")
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.Tree.empty_tree",
                (object[] args) => new Dictionary<int, object>()
            ).Execute();

            var command = new CollisionCommand(obj1.Object, obj2.Object);

            command.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Never());
        }

        [Fact]
        public void CollisionCommand_ShouldHandleCollision_WhenObjectsCollide()
        {
            
            Init_CollisionTree(collisionTree, "ShipTorpedo");
            var collisionHandle = new Mock<ICommand>();
            collisionHandle.Setup(c => c.Execute());

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.GetState",
                (object[] args) => new Tuple<int[], string>(new[] {1, 2, 3}, "collision_tree")
            ).Execute();

            var obj1 = new Mock<IDictionary<string, object>>();
            var obj2 = new Mock<IDictionary<string, object>>();

            obj1.Setup(obj => obj["Position"]).Returns(new int[] { 10, 10 });
            obj2.Setup(obj => obj["Position"]).Returns(new int[] { 10, 10 });

            obj1.Setup(obj => obj["Velocity"]).Returns(new int[] { 1, 1 });
            obj2.Setup(obj => obj["Velocity"]).Returns(new int[] { 0, 0 });

            obj1.Setup(obj => obj["Type"]).Returns("Torpedo");
            obj2.Setup(obj => obj["Type"]).Returns("Ship");

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.ReferenceRules",
                (object[] args) => new Dictionary<(string, string), string>
                {
                    { ("Torpedo", "Ship"), "Torpedo" }
                }
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.GetState",
                (object[] args) => (new[] {1, 2, 3}, "collision_tree")
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Collision.Tree.collision_tree",
                (object[] args) => new Dictionary<int, object>
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
                }
            ).Execute();

            var command = new CollisionCommand(obj1.Object, obj2.Object);

            command.Execute();

            collisionHandle.Verify(c => c.Execute(), Times.Once());
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
            var path = new[] {1, 2, 3};
            
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
            var path = new[] {1, 2, 3};
            
            var result = CollisionCommand.CheckCollisionExists(tree, path);
            
            Assert.False(result);
        }
    }
}
