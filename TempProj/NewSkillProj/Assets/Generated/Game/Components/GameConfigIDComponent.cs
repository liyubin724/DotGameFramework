//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public ConfigIDComponent configID { get { return (ConfigIDComponent)GetComponent(GameComponentsLookup.ConfigID); } }
    public bool hasConfigID { get { return HasComponent(GameComponentsLookup.ConfigID); } }

    public void AddConfigID(int newValue) {
        var index = GameComponentsLookup.ConfigID;
        var component = (ConfigIDComponent)CreateComponent(index, typeof(ConfigIDComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceConfigID(int newValue) {
        var index = GameComponentsLookup.ConfigID;
        var component = (ConfigIDComponent)CreateComponent(index, typeof(ConfigIDComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveConfigID() {
        RemoveComponent(GameComponentsLookup.ConfigID);
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

    static Entitas.IMatcher<GameEntity> _matcherConfigID;

    public static Entitas.IMatcher<GameEntity> ConfigID {
        get {
            if (_matcherConfigID == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ConfigID);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherConfigID = matcher;
            }

            return _matcherConfigID;
        }
    }
}
