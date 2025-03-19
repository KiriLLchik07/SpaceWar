using SpaceWar_workspace;

namespace SpaceWar_Tests
{
    public class RemoveGameCommandTests
    {
        [Fact]
        public void RemoveGameCommand_RemovesObject_Successfully()
        {
            var gameItems = new Dictionary<string, IDictionary<string, object>>
            {
                { "Ship1", new Dictionary<string, object> { { "Type", "Spaceship" } } }
            };
            var command = new RemoveGameCommand(gameItems, "Ship1");

            command.Execute();

            Assert.False(gameItems.ContainsKey("Ship1"));
        }

        [Fact]
        public void RemoveGameCommand_ThrowsException_WhenObjectIdNotFound()
        {
            var gameItems = new Dictionary<string, IDictionary<string, object>>();
            var command = new RemoveGameCommand(gameItems, "Ship1");

            var exception = Assert.Throws<KeyNotFoundException>(() => command.Execute());
            Assert.Equal("Object with ID Ship1 not found.", exception.Message);
        }
    }
}
