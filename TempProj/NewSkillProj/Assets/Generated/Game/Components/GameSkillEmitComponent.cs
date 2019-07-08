//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public SkillEmitComponent skillEmit { get { return (SkillEmitComponent)GetComponent(GameComponentsLookup.SkillEmit); } }
    public bool hasSkillEmit { get { return HasComponent(GameComponentsLookup.SkillEmit); } }

    public void AddSkillEmit(System.Collections.Generic.Dictionary<int, SkillEmitData> newDataDic) {
        var index = GameComponentsLookup.SkillEmit;
        var component = (SkillEmitComponent)CreateComponent(index, typeof(SkillEmitComponent));
        component.dataDic = newDataDic;
        AddComponent(index, component);
    }

    public void ReplaceSkillEmit(System.Collections.Generic.Dictionary<int, SkillEmitData> newDataDic) {
        var index = GameComponentsLookup.SkillEmit;
        var component = (SkillEmitComponent)CreateComponent(index, typeof(SkillEmitComponent));
        component.dataDic = newDataDic;
        ReplaceComponent(index, component);
    }

    public void RemoveSkillEmit() {
        RemoveComponent(GameComponentsLookup.SkillEmit);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherSkillEmit;

    public static Entitas.IMatcher<GameEntity> SkillEmit {
        get {
            if (_matcherSkillEmit == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.SkillEmit);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherSkillEmit = matcher;
            }

            return _matcherSkillEmit;
        }
    }
}
