namespace SpaceWar_workspace
{
    public class RemoveGameCommand : ICommand
    {
        private readonly Dictionary<string, IDictionary<string, object>> _gameItems;
        private readonly string _gameObjectId;

        public RemoveGameCommand(Dictionary<string, IDictionary<string, object>> gameItems, string objectId)
        {
            _gameItems = gameItems;
            _gameObjectId = objectId;
        }

        public void Execute()
        {
            if (!_gameItems.ContainsKey(_gameObjectId))
            {
                throw new KeyNotFoundException($"Object with ID {_gameObjectId} not found.");
            }

            _gameItems.Remove(_gameObjectId);
        }
    }
}