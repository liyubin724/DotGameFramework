using Entitas;

[Game]
public class MotionTypeComponent : IComponent 
{
    public MotionType value = MotionType.None;
}	

public enum MotionType
{
    None,
    Linear,
}