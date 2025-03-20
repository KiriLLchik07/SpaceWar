using SpaceWar_workspace;
using Hwdtech.Ioc;

namespace SpaceWar_Tests
{
    public class RegisterIocDependencyGameRepositoryTests
    {
        public RegisterIocDependencyGameRepositoryTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        }
        
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

            addCommand.Execute();
            Assert.NotNull(addCommand);

            var getCommandResult = IoC.Resolve<IDictionary<string, object>>(
                "GameItem.Get",
                "Ship1"
            );
            Assert.NotNull(getCommandResult);
            
            var removeCommand = IoC.Resolve<ICommand>(
                "GameItem.Remove",
                "Ship1"
            );
            Assert.NotNull(removeCommand);

        }

        [Fact]
        public void RegisterIocDependencyGameRepository_ThrowsException_WhenDependencyNotRegistered()
        {
            var registerCommand = new RegisterIocDependencyGameRepository();
            registerCommand.Execute();

            Assert.Throws<ArgumentException>(() =>
                IoC.Resolve<ICommand>("Несуществующая команда")
            );
        }
    }
}
