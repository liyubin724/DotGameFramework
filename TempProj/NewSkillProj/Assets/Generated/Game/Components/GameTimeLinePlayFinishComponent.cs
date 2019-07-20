//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public TimeLinePlayFinishComponent timeLinePlayFinish { get { return (TimeLinePlayFinishComponent)GetComponent(GameComponentsLookup.TimeLinePlayFinish); } }
    public bool hasTimeLinePlayFinish { get { return HasComponent(GameComponentsLookup.TimeLinePlayFinish); } }

    public void AddTimeLinePlayFinish(string newGroupName) {
        var index = GameComponentsLookup.TimeLinePlayFinish;
        var component = (TimeLinePlayFinishComponent)CreateComponent(index, typeof(TimeLinePlayFinishComponent));
        component.groupName = newGroupName;
        AddComponent(index, component);
    }

    public void ReplaceTimeLinePlayFinish(string newGroupName) {
        var index = GameComponentsLookup.TimeLinePlayFinish;
        var component = (TimeLinePlayFinishComponent)CreateComponent(index, typeof(TimeLinePlayFinishComponent));
        component.groupName = newGroupName;
        ReplaceComponent(index, component);
    }

    public void RemoveTimeLinePlayFinish() {
        RemoveComponent(GameComponentsLookup.TimeLinePlayFinish);
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

    static Entitas.IMatcher<GameEntity> _matcherTimeLinePlayFinish;

    public static Entitas.IMatcher<GameEntity> TimeLinePlayFinish {
        get {
            if (_matcherTimeLinePlayFinish == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TimeLinePlayFinish);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTimeLinePlayFinish = matcher;
            }

            return _matcherTimeLinePlayFinish;
        }
    }
}
