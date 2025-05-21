using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class GameReceiverTests : IDisposable
    {
        public GameReceiverTests()
        {
            new App.Scopes.InitCommand().Execute();
            var iocScope = App.Ioc.Resolve<object>("IoC.Scope.Create");
            App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Positive_Receive_AddsCommandToQueue()
        {
            var queue = new Queue<SpaceWar_workspace.ICommand>();
            var receiver = new GameReceiver(queue);
            var commandMock = new Mock<SpaceWar_workspace.ICommand>();

            receiver.Receive(commandMock.Object);

            Assert.Single(queue);
            Assert.Equal(commandMock.Object, queue.Dequeue());
        }

        [Fact]
        public void Positive_Receive_AcceptsNullCommand()
        {
            var queue = new Queue<SpaceWar_workspace.ICommand>();
            var receiver = new GameReceiver(queue);

            receiver.Receive(null!);

            Assert.Single(queue);
            Assert.Null(queue.Dequeue());
        }

        [Fact]
        public void Negative_Constructor_ThrowsExceptionOnNullQueue()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new GameReceiver(null!));
            Assert.Equal("Value cannot be null. (Parameter 'queue')", exception.Message);
        }

        public void Dispose()
        {
            App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
