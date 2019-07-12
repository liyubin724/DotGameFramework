using Entitas;

[Game]
public class EffectBindComponent : IComponent 
{
    public EffectUsedEnv bindType = EffectUsedEnv.BindNode;
    public BindNodeType nodeType = BindNodeType.Main;
    public int bindIndex = -1;
}