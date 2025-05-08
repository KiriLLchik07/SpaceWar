using SpaceWar_workspace;
namespace SpaceWar_Tests
{
    public class AddGameCommandTests
    {
        [Fact]
        public void AddGameCommand_AddsNewObject_Successfully()
        {
            var gameItems = new Dictionary<string, IDictionary<string, object>>();
            var parameters = new Dictionary<string, object> { { "Type", "Spaceship" } };
            var command = new AddGameCommand(gameItems, "Ship1", parameters);

            command.Execute();

            Assert.True(gameItems.ContainsKey("Ship1"));
            Assert.Equal("Spaceship", gameItems["Ship1"]["Type"]);
        }

        [Fact]
        public void AddGameCommand_ThrowsException_WhenObjectIdAlreadyExists()
        {
            var gameItems = new Dictionary<string, IDictionary<string, object>>
            {
                { "Ship1", new Dictionary<string, object> { { "Type", "Spaceship" } } }
            };
            var parameters = new Dictionary<string, object> { { "Type", "Fighter" } };
            var command = new AddGameCommand(gameItems, "Ship1", parameters);

            var exception = Assert.Throws<InvalidOperationException>(() => command.Execute());
            Assert.Equal("Объект с ID Ship1 уже существует.", exception.Message);
        }
    }
}
