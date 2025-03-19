using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class RegisterIocDependencyGameRepositoryTests
    {
        [Fact]
        public void RegisterIocDependencyGameRepository_RegistersDependencies_Successfully()
        {
            var registerCommand = new RegisterIocDependencyGameRepository();

            registerCommand.Execute();

            var addCommand = IoC.Resolve<ICommand>(
                "GameItem.Add",
                "Ship1",
                new Dictionary<string, object> { { "Type", "Spaceship" } }
            );
            Assert.NotNull(addCommand);

            var removeCommand = IoC.Resolve<ICommand>(
                "GameItem.Remove",
                "Ship1"
            );
            Assert.NotNull(removeCommand);

            var getCommandResult = IoC.Resolve<IDictionary<string, object>>(
                "GameItem.Get",
                "Ship1"
            );
            Assert.NotNull(getCommandResult);
        }

        [Fact]
        public void RegisterIocDependencyGameRepository_ThrowsException_WhenDependencyNotRegistered()
        {
            var registerCommand = new RegisterIocDependencyGameRepository();
            registerCommand.Execute();

            Assert.Throws<InvalidOperationException>(() =>
                IoC.Resolve<ICommand>("NonExistentCommand")
            );
        }
    }
}