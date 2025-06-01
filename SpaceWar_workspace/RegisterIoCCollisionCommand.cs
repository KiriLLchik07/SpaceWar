using App;

namespace SpaceWar_workspace;

public class RegisterIoCCollisionCommand : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Collision.GetState",
            (object[] args) =>
            {
                var obj1 = args[0];
                var obj2 = args[1];

                var type1 = Ioc.Resolve<string>("Game.GetType", obj1);
                var type2 = Ioc.Resolve<string>("Game.GetType", obj2);

                var referenceRules = Ioc.Resolve<IDictionary<(string, string), string>>("Collision.ReferenceRules");

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

                var otherType = referenceType == type1 ? type2 : type1;

                var reference = referenceType == type1 ? obj1 : obj2;
                var other = referenceType == type1 ? obj2 : obj1;

                var positionRef = Ioc.Resolve<int[]>("Game.GetPosition", reference);
                var positionOther = Ioc.Resolve<int[]>("Game.GetPosition", other);
                var velocityRef = Ioc.Resolve<int[]>("Game.GetVelocity", reference);
                var velocityOther = Ioc.Resolve<int[]>("Game.GetVelocity", other);

                var dPositions = positionRef.Zip(positionOther, (r, o) => r - o).ToArray();
                var dVelocities = velocityRef.Zip(velocityOther, (r, o) => r - o).ToArray();

                return (object)(dPositions.Concat(dVelocities).ToArray(), $"{referenceType}{otherType}");
            }
        ).Execute();
    }
}
