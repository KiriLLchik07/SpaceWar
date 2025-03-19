namespace SpaceWar_workspace
{
    public class RegisterIocDependencyGameRepository : ICommand
    {
        public void Execute()
        {
            var gameItems = new Dictionary<string, IDictionary<string, object>>();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "GameItem.Get",
                (object[] args) => gameItems[(string)args[0]]
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "GameItem.Add",
                (object[] args) => new AddGameCommand(
                    gameItems,
                    (string)args[0],
                    (IDictionary<string, object>)args[1]
                )
            ).Execute();

            IoC.Resolve<ICommand>(
                "IoC.Register",
                "GameItem.Remove",
                (object[] args) => new RemoveGameCommand(
                    gameItems,
                    (string)args[0]
                )
            ).Execute();
        }
    }
}