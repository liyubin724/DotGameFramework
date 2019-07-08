using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class OwnerIDComponent : IComponent 
{
    [EntityIndex]
    public int value;
}

