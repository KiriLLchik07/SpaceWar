using App.Scopes;
using SpaceWar_workspace;

namespace SpaceWar_Tests;

public class GameTests : IDisposable
{
    public GameTests()
    {
        new InitCommand().Execute();
        var iocScope = App.Ioc.Resolve<object>("IoC.Scope.Create");
        App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
    }

    [Fact]
    public void ExecuteCommandsTest()
    {
        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.TimeQuant",
            (object[] args) => (object)TimeSpan.FromMilliseconds(50)
        ).Execute();

        var game_queue = new Queue<App.ICommand>();

        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.CommandsQueue",
            (object[] args) => game_queue
        ).Execute();

        var game_scope = App.Ioc.Resolve<object>("IoC.Scope.Current");

        var cmd1 = new Mock<App.ICommand>();
        cmd1.Setup(c => c.Execute());
        var cmd2 = new Mock<App.ICommand>();
        cmd2.Setup(c => c.Execute());

        game_queue.Enqueue(cmd1.Object);
        game_queue.Enqueue(cmd2.Object);

        var game = new Game(game_scope);

        game.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void ExecuteEmptyQueueTest()
    {
        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.TimeQuant",
            (object[] args) => (object)TimeSpan.FromMilliseconds(50)
        ).Execute();

        var game_queue = new Queue<App.ICommand>();

        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.CommandsQueue",
            (object[] args) => game_queue
        ).Execute();

        var game_scope = App.Ioc.Resolve<object>("IoC.Scope.Current");
        var game = new Game(game_scope);

        var exception = Record.Exception(() => game.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void ExecuteCommandsTimeExceededTest()
    {
        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.TimeQuant",
            (object[] args) => (object)TimeSpan.FromMilliseconds(10)
        ).Execute();

        var game_queue = new Queue<App.ICommand>();

        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.CommandsQueue",
            (object[] args) => game_queue
        ).Execute();

        var game_scope = App.Ioc.Resolve<object>("IoC.Scope.Current");

        var cmd1 = new Mock<App.ICommand>();
        cmd1.Setup(c => c.Execute()).Callback(() => Thread.Sleep(20));
        var cmd2 = new Mock<App.ICommand>();
        cmd2.Setup(c => c.Execute());

        game_queue.Enqueue(cmd1.Object);
        game_queue.Enqueue(cmd2.Object);

        var game = new Game(game_scope);

        game.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Never());
    }

    [Fact]
    public void ExecuteCommandExceptionTest()
    {
        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.TimeQuant",
            (object[] args) => (object)TimeSpan.FromMilliseconds(50)
        ).Execute();

        var game_queue = new Queue<App.ICommand>();

        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Game.CommandsQueue",
            (object[] args) => game_queue
        ).Execute();

        var game_scope = App.Ioc.Resolve<object>("IoC.Scope.Current");

        var cmd1 = new Mock<App.ICommand>();
        cmd1.Setup(c => c.Execute()).Throws(new Exception());

        var mockHandle = new Mock<App.ICommand>();
        mockHandle.Setup(e => e.Execute());

        App.Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "ExceptionHandler.Handle",
            (object[] args) => mockHandle.Object
        ).Execute();

        var cmd2 = new Mock<App.ICommand>();
        cmd2.Setup(c => c.Execute());
        game_queue.Enqueue(cmd1.Object);
        game_queue.Enqueue(cmd2.Object);

        var game = new Game(game_scope);

        game.Execute();

        mockHandle.Verify(e => e.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }

    public void Dispose()
    {
        App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }
}
