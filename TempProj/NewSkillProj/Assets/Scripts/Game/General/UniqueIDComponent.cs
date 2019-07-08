using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class UniqueIDComponent : IComponent 
{
    [PrimaryEntityIndex]
    public int value;
}

