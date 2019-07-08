using DotTimeLine.Base.Groups;
using UnityEngine;

public class ConfigDataService : Service
{
    public ConfigDataService(Contexts contexts) : base(contexts)
    {
    }

    public override void DoDestroy()
    {

    }

    public override void DoReset()
    {

    }

    public T GetData<T>(string resPath) where T : ScriptableObject => Resources.Load<T>(resPath);

    private SkillConfig skillConfig = new SkillConfig();
    public SkillConfigData GetSkillData(int id) => skillConfig.GetData(id);

    SoundConfig soundConfig = new SoundConfig();
    public SoundConfigData GetSoundData(int id) => soundConfig.GetData(id);
    
    EffectConfig effectConfig = new EffectConfig();
    public EffectConfigData GetEffectData(int id) => effectConfig.GetData(id);

    BulletConfig bulletConfig = new BulletConfig();
    public BulletConfigData GetBulletData(int id) => bulletConfig.GetData(id);

}



//public class SkillController
//{
//    public List<FCController> datas = new List<FCController>();
//    public SkillController()
//    {
//        FCController fcc = new FCController();
//        fcc.actionComposeList.Add(Create1());
//        fcc.actionComposeList.Add(Create2());
//        fcc.actionComposeList.Add(Create3());
//        datas.Add(fcc);
//    }

//    FCSequence Create3()
//    {
//        WaitingTimeAction a1 = new WaitingTimeAction();
//        a1.Index = 200;
//        a1.Name = "Waiting Time";
//        a1.LeftTime = 5f;

//        RemoveAllAction a2 = new RemoveAllAction();
//        a2.Index = 201;
//        a2.Name = "Remove All";
        
//        FCSequence p = new FCSequence();
//        p.Index = 299;
//        p.Name = "End";
//        p.AddChildren(a1);
//        p.AddChildren(a2);

//        return p;
//    }

//    FCParallel Create2()
//    {

//        PlaySoundAction a1 = new PlaySoundAction();
//        a1.Index = 100;
//        a1.Name = "Play Sound";
//        a1.SoundConfigID = 2;

//        RemoveEffectAction a2 = new RemoveEffectAction();
//        a2.Index = 101;
//        a2.Name = "Remove Effect";
//        a2.EffectIndex = 3;
//        RemoveEffectAction a3 = new RemoveEffectAction();
//        a3.Index = 102;
//        a3.Name = "Remove Effect";
//        a3.EffectIndex = 4;
//        RemoveEffectAction a4 = new RemoveEffectAction();
//        a4.Index = 103;
//        a4.Name = "Remove Effect";
//        a4.EffectIndex = 5;

//        StopSoundAction a5 = new StopSoundAction();
//        a5.Index = 104;
//        a5.Name = "Stop Sound";
//        a5.SoundIndex = 2;


//        AddEffectAction a6 = new AddEffectAction();
//        a6.Index = 105;
//        a6.Name = "Add Effect";
//        a6.EffectConfigID = 2;
//        a6.EffectType = EffectType.BindNode;
//        a6.EffectNodeType = EntityBindNodeType.Main;
//        a6.NodeIndex = 0;
//        AddEffectAction a7 = new AddEffectAction();
//        a7.Index = 106;
//        a7.Name = "Add Effect";
//        a7.EffectConfigID = 2;
//        a7.EffectType = EffectType.BindNode;
//        a7.EffectNodeType = EntityBindNodeType.Main;
//        a7.NodeIndex = 1;
//        AddEffectAction a8 = new AddEffectAction();
//        a8.Index = 107;
//        a8.Name = "Add Effect";
//        a8.EffectConfigID = 2;
//        a8.EffectType = EffectType.BindNode;
//        a8.EffectNodeType = EntityBindNodeType.Main;
//        a8.NodeIndex = 2;

//        EmitBulletAction a9 = new EmitBulletAction();
//        a9.Index = 108;
//        a9.Name = "Emit Bullet";
//        a9.BulletConfigID = 1;
//        EmitBulletAction a10 = new EmitBulletAction();
//        a10.Index = 109;
//        a10.Name = "Emit Bullet";
//        a10.BulletConfigID = 1;
//        EmitBulletAction a11 = new EmitBulletAction();
//        a11.Index = 110;
//        a11.Name = "Emit Bullet";
//        a11.BulletConfigID = 1;

//        FCParallel p = new FCParallel();
//        p.Index = 199;
//        p.Name = "Emit";
//        p.AddChildren(a1);
//        p.AddChildren(a2);
//        p.AddChildren(a3);
//        p.AddChildren(a4);
//        p.AddChildren(a5);
//        p.AddChildren(a6);
//        p.AddChildren(a7);
//        p.AddChildren(a8);
//        p.AddChildren(a9);
//        p.AddChildren(a10);
//        p.AddChildren(a11);

//        return p;
//    }

//    FCParallel Create1()
//    {
//        WaitingTimeAction a1 = new WaitingTimeAction();
//        a1.Index = 1;
//        a1.Name = "Waiting Time";
//        a1.LeftTime = 5f;

//        PlaySoundAction a2 = new PlaySoundAction();
//        a2.Index = 2;
//        a2.Name = "Play Sound";
//        a2.SoundConfigID = 1;

//        AddEffectAction a3 = new AddEffectAction();
//        a3.Index = 3;
//        a3.Name = "Add Effect";
//        a3.EffectConfigID = 1;
//        a3.EffectType = EffectType.BindNode;
//        a3.EffectNodeType = EntityBindNodeType.Main;
//        a3.NodeIndex = 0;
//        AddEffectAction a4 = new AddEffectAction();
//        a4.Index = 4;
//        a4.Name = "Add Effect";
//        a4.EffectConfigID = 1;
//        a4.EffectType = EffectType.BindNode;
//        a4.EffectNodeType = EntityBindNodeType.Main;
//        a4.NodeIndex = 1;
//        AddEffectAction a5 = new AddEffectAction();
//        a5.Index = 5;
//        a5.Name = "Add Effect";
//        a5.EffectConfigID = 1;
//        a5.EffectType = EffectType.BindNode;
//        a5.EffectNodeType = EntityBindNodeType.Main;
//        a5.NodeIndex = 2;
//        FCParallel p = new FCParallel();
//        p.Index = 6;
//        p.Name = "Start";
//        p.AddChildren(a1);
//        p.AddChildren(a2);
//        p.AddChildren(a3);
//        p.AddChildren(a4);
//        p.AddChildren(a5);

//        return p;
//    }
//}