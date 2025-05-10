namespace SpaceWar_workspace;

public interface ICommandReceiver
{
    void Receive(ICommand command);
}
