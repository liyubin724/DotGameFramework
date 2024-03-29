//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly NewSkillComponent newSkillComponent = new NewSkillComponent();

    public bool isNewSkill {
        get { return HasComponent(GameComponentsLookup.NewSkill); }
        set {
            if (value != isNewSkill) {
                var index = GameComponentsLookup.NewSkill;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : newSkillComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
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

    static Entitas.IMatcher<GameEntity> _matcherNewSkill;

    public static Entitas.IMatcher<GameEntity> NewSkill {
        get {
            if (_matcherNewSkill == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NewSkill);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherNewSkill = matcher;
            }

            return _matcherNewSkill;
        }
    }
}
