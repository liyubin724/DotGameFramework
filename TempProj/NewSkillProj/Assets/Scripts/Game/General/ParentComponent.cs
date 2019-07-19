using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class ParentComponent : IComponent 
{
    [EntityIndex]
    public int entityID;
}

