﻿using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class SendCommandTests
    {
        private readonly Mock<ICommandReceiver> _messageHandler;
        private readonly Mock<ICommand> _longRunningTask;
        private readonly ICommand _sendCommand;

        public SendCommandTests()
        {
            _messageHandler = new Mock<ICommandReceiver>();
            _longRunningTask = new Mock<ICommand>();
            _sendCommand = new SendCommand(_longRunningTask.Object, _messageHandler.Object);
        }

        [Fact]
        public void SendCommand_Successfully_Transfers_Command()
        {
            _messageHandler.Setup(x => x.Receive(_longRunningTask.Object));
            _sendCommand.Execute();
            _messageHandler.Verify(handler => handler.Receive(_longRunningTask.Object), Times.Once());
        }

        [Fact]
        public void SendCommand_Propagates_Handler_Exception()
        {
            _messageHandler
                .Setup(handler => handler.Receive(_longRunningTask.Object))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _sendCommand.Execute());
        }
    }
}
