//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public AddSkeletonComponent addSkeleton { get { return (AddSkeletonComponent)GetComponent(GameComponentsLookup.AddSkeleton); } }
    public bool hasAddSkeleton { get { return HasComponent(GameComponentsLookup.AddSkeleton); } }

    public void AddAddSkeleton(string newSkeletonPath) {
        var index = GameComponentsLookup.AddSkeleton;
        var component = (AddSkeletonComponent)CreateComponent(index, typeof(AddSkeletonComponent));
        component.skeletonPath = newSkeletonPath;
        AddComponent(index, component);
    }

    public void ReplaceAddSkeleton(string newSkeletonPath) {
        var index = GameComponentsLookup.AddSkeleton;
        var component = (AddSkeletonComponent)CreateComponent(index, typeof(AddSkeletonComponent));
        component.skeletonPath = newSkeletonPath;
        ReplaceComponent(index, component);
    }

    public void RemoveAddSkeleton() {
        RemoveComponent(GameComponentsLookup.AddSkeleton);
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

    static Entitas.IMatcher<GameEntity> _matcherAddSkeleton;

    public static Entitas.IMatcher<GameEntity> AddSkeleton {
        get {
            if (_matcherAddSkeleton == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AddSkeleton);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAddSkeleton = matcher;
            }

            return _matcherAddSkeleton;
        }
    }
}
