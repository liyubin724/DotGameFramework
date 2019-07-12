using Entitas;

[Game]
public class MotionCurveTypeComponent : IComponent 
{
    public MotionCurveType value = MotionCurveType.None;
}	

public enum MotionCurveType
{
    None,
    Linear,
}