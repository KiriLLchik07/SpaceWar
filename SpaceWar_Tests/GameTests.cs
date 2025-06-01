using App;
using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class GameTests
    {
        [Fact]
        public void Execute_ProcessesAllCommandsWithinTimeLimit()
        {
            // Arrange
            new InitCommand().Execute();
            var scope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();

            var mockCommand1 = new Mock<App.ICommand>();
            var mockCommand2 = new Mock<App.ICommand>();
            var commandQueue = new Queue<App.ICommand>();
            App.ICommand? currentCommand = null;

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) =>
            {
                currentCommand = commandQueue.Dequeue();
                return currentCommand;
            }).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Current", (object[] args) => currentCommand).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Count", (object[] args) => () => commandQueue.Count).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(500)).Execute();

            var mockExceptionHandler = new Mock<App.ICommand>();
            Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler", (object[] args) => mockExceptionHandler.Object).Execute();

            commandQueue.Enqueue(mockCommand1.Object);
            commandQueue.Enqueue(mockCommand2.Object);

            // Act
            new Game(scope).Execute();

            // Assert
            mockCommand1.Verify(c => c.Execute(), Times.Once);
            mockCommand2.Verify(c => c.Execute(), Times.Once);
            Assert.Empty(commandQueue);
        }

        [Fact]
        public void Execute_StopsWhenTimeLimitExceeded()
        {
            // Arrange
            new InitCommand().Execute();
            var scope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();

            var mockCommand1 = new Mock<App.ICommand>();
            var commandQueue = new Queue<App.ICommand>();
            App.ICommand? currentCommand = null;

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) =>
            {
                currentCommand = commandQueue.Dequeue();
                return currentCommand;
            }).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Current", (object[] args) => currentCommand).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Count", (object[] args) => () => commandQueue.Count).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(-1)).Execute();

            var mockExceptionHandler = new Mock<App.ICommand>();
            Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler", (object[] args) => mockExceptionHandler.Object).Execute();

            commandQueue.Enqueue(mockCommand1.Object);

            // Act
            new Game(scope).Execute();

            // Assert
            mockCommand1.Verify(c => c.Execute(), Times.Never);
            Assert.Single(commandQueue);
        }

        [Fact]
        public void Execute_HandlesExceptionsWithCurrentCommand()
        {
            // Arrange
            new InitCommand().Execute();
            var scope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();

            var mockFailingCommand = new Mock<App.ICommand>();
            mockFailingCommand.Setup(c => c.Execute()).Throws<Exception>();

            var commandQueue = new Queue<App.ICommand>();
            App.ICommand? currentCommand = null;

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) =>
            {
                currentCommand = commandQueue.Dequeue();
                return currentCommand;
            }).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Current", (object[] args) => currentCommand).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Count", (object[] args) => () => commandQueue.Count).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(500)).Execute();

            var mockExceptionHandler = new Mock<App.ICommand>();
            Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler", (object[] args) => mockExceptionHandler.Object).Execute();

            commandQueue.Enqueue(mockFailingCommand.Object);

            // Act
            new Game(scope).Execute();

            // Assert
            mockFailingCommand.Verify(c => c.Execute(), Times.Once);
            mockExceptionHandler.Verify(h => h.Execute(), Times.Once);
        }
    }
}
