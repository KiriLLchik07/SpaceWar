namespace SpaceWar_workspace;
public class CreateMacroCommandStrategy
{
    private readonly string _commandSpec;
    public CreateMacroCommandStrategy(string commandSpec)
    {
        _commandSpec = commandSpec;
    }
    public ICommand Resolve(object[] args)
    {
        var nameofcommand = IoC.Resolve<IEnumerable<string>>($"Specs.{_commandSpec}").ToList();
        var command = nameofcommand.Select((name, index) => 
        {
            if (index < args.Length)
            {
                return IoC.Resolve<ICommand>(name, new object[] { args[index] });
            }
            else
            {
                return IoC.Resolve<ICommand>(name);
            }
        }).ToList();

        return new MacroCommand(command);
    }
}
