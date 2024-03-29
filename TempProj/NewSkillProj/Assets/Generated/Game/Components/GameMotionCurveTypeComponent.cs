//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public MotionCurveTypeComponent motionCurveType { get { return (MotionCurveTypeComponent)GetComponent(GameComponentsLookup.MotionCurveType); } }
    public bool hasMotionCurveType { get { return HasComponent(GameComponentsLookup.MotionCurveType); } }

    public void AddMotionCurveType(MotionCurveType newValue) {
        var index = GameComponentsLookup.MotionCurveType;
        var component = (MotionCurveTypeComponent)CreateComponent(index, typeof(MotionCurveTypeComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceMotionCurveType(MotionCurveType newValue) {
        var index = GameComponentsLookup.MotionCurveType;
        var component = (MotionCurveTypeComponent)CreateComponent(index, typeof(MotionCurveTypeComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveMotionCurveType() {
        RemoveComponent(GameComponentsLookup.MotionCurveType);
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

    static Entitas.IMatcher<GameEntity> _matcherMotionCurveType;

    public static Entitas.IMatcher<GameEntity> MotionCurveType {
        get {
            if (_matcherMotionCurveType == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MotionCurveType);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMotionCurveType = matcher;
            }

            return _matcherMotionCurveType;
        }
    }
}
