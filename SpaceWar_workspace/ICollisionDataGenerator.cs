namespace SpaceWar_workspace;

public interface ICollisionDataGenerator
{
    string firstShape { get; }
    string secondShape { get; }

    IList<int[]> GenerateCollisionData();
}
