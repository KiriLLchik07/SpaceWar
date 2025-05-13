namespace SpaceWar_workspace;

public class RegisterIoCCollisionCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.Check",
            (object[] args) => new CollisionCommand(args[0], args[1])
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.GetState",
            (object[] args) =>
            {
                var obj1 = args[0];
                var obj2 = args[1];

                var type1 = IoC.Resolve<string>("Game.GetType", obj1);
                var type2 = IoC.Resolve<string>("Game.GetType", obj2);

                var referenceRules = IoC.Resolve<IDictionary<(string, string), string>>("Collision.ReferenceRules");

                string referenceType;
                if (referenceRules.TryGetValue((type1, type2), out var refType))
                {
                    referenceType = refType;
                }
                else if (referenceRules.TryGetValue((type2, type1), out refType))
                {
                    referenceType = refType;
                }
                else
                {
                    referenceType = type1;
                }

                string otherType;
                object reference, other;
                if (referenceType == type1)
                {
                    otherType = type2;
                    reference = obj1;
                    other = obj2;
                }
                else
                {
                    otherType = type1;
                    reference = obj2;
                    other = obj1;
                }

                var positionRef = IoC.Resolve<int[]>("Game.GetPosition", reference);
                var positionOther = IoC.Resolve<int[]>("Game.GetPosition", other);
                var velocityRef = IoC.Resolve<int[]>("Game.GetVelocity", reference);
                var velocityOther = IoC.Resolve<int[]>("Game.GetVelocity", other);

                var diffCount = positionRef.Length;
                var dPositions = new int[diffCount];
                var dVelocities = new int[diffCount];
                for (var i = 0; i < diffCount; i++)
                {
                    dPositions[i] = positionRef[i] - positionOther[i];
                    dVelocities[i] = velocityRef[i] - velocityOther[i];
                }

                var combined = new int[diffCount * 2];
                Array.Copy(dPositions, 0, combined, 0, diffCount);
                Array.Copy(dVelocities, 0, combined, diffCount, diffCount);

                return (object)(combined, $"{referenceType}{otherType}");
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Collision.Handle",
            (object[] args) => new ActionCommand(() => 
            {
                Console.WriteLine($"Collision handled between {args[0]} and {args[1]}");
            })
        ).Execute();
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}
