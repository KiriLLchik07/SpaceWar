namespace SpaceWar_workspace;

public class CollisionCommand : ICommand
{
    private readonly object _obj1;
    private readonly object _obj2;

    public CollisionCommand(object obj1, object obj2)
    {
        _obj1 = obj1;
        _obj2 = obj2;
    }

    public void Execute()
    {
        var collisionState = IoC.Resolve<(int[] branch, string treeKey)>(
            "Collision.GetState", 
            _obj1, 
            _obj2
        );

        var collisionTree = IoC.Resolve<IDictionary<int, object>>(
            $"Collision.Tree.{collisionState.treeKey}"
        );

        if (CheckCollisionExists(collisionTree, collisionState.branch))
        {
            IoC.Resolve<ICommand>(
                "Collision.Handle", 
                _obj1, 
                _obj2
            ).Execute();
        }
    }

    private static bool CheckCollisionExists(IDictionary<int, object> collisionTree, int[] branchPath)
    {
        if (branchPath.Length == 0)
        {
            return false;
        }

        object current = collisionTree;
        
        foreach (var param in branchPath)
        {
            if (current is IDictionary<int, object> dict && dict.TryGetValue(param, out var next))
            {
                current = next;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}