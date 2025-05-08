namespace SpaceWar_workspace
{
    public class GameReceiver : ICommandReceiver
    {
        private readonly Queue<ICommand> _queue;

        public GameReceiver(Queue<ICommand> queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public void Receive(ICommand cmd)
        {
            _queue.Enqueue(cmd);
        }
    }
}
