//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public TimeLineControllerComponent timeLineController { get { return (TimeLineControllerComponent)GetComponent(GameComponentsLookup.TimeLineController); } }
    public bool hasTimeLineController { get { return HasComponent(GameComponentsLookup.TimeLineController); } }

    public void AddTimeLineController(string newAssetPath, Dot.Core.TimeLine.Base.TimeLineController newController) {
        var index = GameComponentsLookup.TimeLineController;
        var component = (TimeLineControllerComponent)CreateComponent(index, typeof(TimeLineControllerComponent));
        component.assetPath = newAssetPath;
        component.controller = newController;
        AddComponent(index, component);
    }

    public void ReplaceTimeLineController(string newAssetPath, Dot.Core.TimeLine.Base.TimeLineController newController) {
        var index = GameComponentsLookup.TimeLineController;
        var component = (TimeLineControllerComponent)CreateComponent(index, typeof(TimeLineControllerComponent));
        component.assetPath = newAssetPath;
        component.controller = newController;
        ReplaceComponent(index, component);
    }

    public void RemoveTimeLineController() {
        RemoveComponent(GameComponentsLookup.TimeLineController);
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

    static Entitas.IMatcher<GameEntity> _matcherTimeLineController;

    public static Entitas.IMatcher<GameEntity> TimeLineController {
        get {
            if (_matcherTimeLineController == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TimeLineController);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTimeLineController = matcher;
            }

            return _matcherTimeLineController;
        }
    }
}
