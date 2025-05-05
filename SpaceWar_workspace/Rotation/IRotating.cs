namespace SpaceWar_workspace;

public interface IRotating
{
    Angle AnglePosition { get; set; }
    Angle RotateVelocity { get; }
}
