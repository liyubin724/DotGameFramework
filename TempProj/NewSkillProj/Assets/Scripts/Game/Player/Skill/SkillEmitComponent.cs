using System.Collections;
using System.Collections.Generic;
using Entitas;

[Game]
public class SkillEmitComponent : IComponent 
{
    public Dictionary<int, SkillEmitData> dataDic = new Dictionary<int, SkillEmitData>();
}	

public class SkillEmitData
{
    public int id;
    public int nodeIndex;
    public BindNodeData bindNodeData;
}

