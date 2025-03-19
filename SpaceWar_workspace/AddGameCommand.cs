namespace SpaceWar_workspace
{
    public class AddGameCommand : ICommand
    {
        private readonly Dictionary<string, IDictionary<string, object>> _gameItems;
        private readonly string _gameObjectId;
        private readonly IDictionary<string, object> _parameters;

        public AddGameCommand(Dictionary<string, IDictionary<string, object>> gameItems, string gameObjectId, IDictionary<string, object> parameters)
        {
            _gameItems = gameItems; 
            _gameObjectId = gameObjectId;
            _parameters = parameters;
        }

        public void Execute()
        {
            if (_gameItems.ContainsKey(_gameObjectId))
            {
                throw new InvalidOperationException($"Объекь с ID {_gameObjectId} уже существует.");
            }

            _gameItems[_gameObjectId] = _parameters;
        }
    }
}