using SpaceWar_workspace;

namespace SpaceBattle.Lib.Tests
{
    public class IoCRegisterGameOperationTests : IDisposable
    {
        public IoCRegisterGameOperationTests()
        {
            new App.Scopes.InitCommand().Execute();
            var iocScope = App.Ioc.Resolve<object>("IoC.Scope.Create");
            App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Positive_Execute_RegistersGameReceiver()
        {
            var queue = new Queue<ICommand>();
            App.Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue).Execute();

            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            var receiver = App.Ioc.Resolve<ICommandReceiver>("Game.Receiver");
            Assert.NotNull(receiver);
            Assert.IsType<GameReceiver>(receiver);

            var testCommand = new Mock<ICommand>();
            receiver.Receive(testCommand.Object);
            Assert.Single(queue);
            Assert.Equal(testCommand.Object, queue.Dequeue());
        }

        [Fact]
        public void Negative_Execute_RegistersReceiverWithDefaultQueue()
        {
            var operation = new IoCRegisterGameOperation();

            operation.Execute();

            var receiver = App.Ioc.Resolve<ICommandReceiver>("Game.Receiver");
            Assert.NotNull(receiver);
            Assert.IsType<GameReceiver>(receiver);

            var testCommand = new Mock<ICommand>();
            receiver.Receive(testCommand.Object);

            var defaultQueue = App.Ioc.Resolve<Queue<ICommand>>("Game.Queue");
            Assert.NotNull(defaultQueue);
            Assert.Single(defaultQueue);
            Assert.Equal(testCommand.Object, defaultQueue.Dequeue());
        }

        public void Dispose()
        {
            App.Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
