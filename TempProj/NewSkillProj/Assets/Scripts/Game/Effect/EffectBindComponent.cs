using Entitas;

[Game]
public class EffectBindComponent : IComponent 
{
    public EffectType bindType = EffectType.BindNode;
    public BindNodeType nodeType = BindNodeType.Main;
    public int bindIndex = -1;
}