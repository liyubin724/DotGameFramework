using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class ChildOfComponent : IComponent 
{
    [EntityIndex]
    public int entityID;
}

