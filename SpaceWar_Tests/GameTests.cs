using App;
using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class GameTests
    {
        private readonly object _scope;
        private readonly Mock<App.ICommand> _mockCommand1;
        private readonly Mock<App.ICommand> _mockCommand2;
        private readonly Mock<App.ICommand> _mockFailingCommand;
        private readonly Mock<App.ICommand> _mockExceptionHandler;
        private readonly Queue<App.ICommand> _commandQueue;
        private App.ICommand? _currentCommand;

        public GameTests()
        {
            new InitCommand().Execute();
            _scope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _scope).Execute();

            _mockCommand1 = new Mock<App.ICommand>();
            _mockCommand2 = new Mock<App.ICommand>();
            _mockFailingCommand = new Mock<App.ICommand>();
            _mockFailingCommand.Setup(c => c.Execute()).Throws<Exception>();
            _mockExceptionHandler = new Mock<App.ICommand>();
            _commandQueue = new Queue<App.ICommand>();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) =>
            {
                _currentCommand = _commandQueue.Dequeue();
                return _currentCommand;
            }).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Current", (object[] args) => _currentCommand).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue.Count", (object[] args) => () => _commandQueue.Count).Execute();
            Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler", (object[] args) =>
            {
                var ex = (Exception)args[0];
                var cmd = (App.ICommand)args[1];
                return _mockExceptionHandler.Object;
            }).Execute();

        }

        [Fact]
        public void Execute_ProcessesAllCommandsWithinTimeLimit()
        {
            // Arrange
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(500)).Execute();
            _commandQueue.Enqueue(_mockCommand1.Object);
            _commandQueue.Enqueue(_mockCommand2.Object);

            // Act
            new Game(_scope).Execute();

            // Assert
            _mockCommand1.Verify(c => c.Execute(), Times.Once);
            _mockCommand2.Verify(c => c.Execute(), Times.Once);
            Assert.Empty(_commandQueue);
        }

        [Fact]
        public void Execute_StopsWhenTimeLimitExceeded()
        {
            // Arrange
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(-1)).Execute();
            _commandQueue.Enqueue(_mockCommand1.Object);

            // Act
            new Game(_scope).Execute();

            // Assert
            _mockCommand1.Verify(c => c.Execute(), Times.Never);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Execute_HandlesExceptionsWithCurrentCommand()
        {
            // Arrange
            Ioc.Resolve<App.ICommand>("IoC.Register", "Command.Time", (object[] args) => (object)TimeSpan.FromMilliseconds(500)).Execute();
            _commandQueue.Enqueue(_mockFailingCommand.Object);

            // Act
            new Game(_scope).Execute();

            // Assert
            _mockExceptionHandler.Verify(h =>
                h.Execute(),
                Times.Once
            );

            _mockFailingCommand.Verify(c =>
                c.Execute(),
                Times.Once
            );
        }
    }
}
