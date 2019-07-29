//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public SkeletonComponent skeleton { get { return (SkeletonComponent)GetComponent(GameComponentsLookup.Skeleton); } }
    public bool hasSkeleton { get { return HasComponent(GameComponentsLookup.Skeleton); } }

    public void AddSkeleton(string newAssetPath) {
        var index = GameComponentsLookup.Skeleton;
        var component = (SkeletonComponent)CreateComponent(index, typeof(SkeletonComponent));
        component.assetPath = newAssetPath;
        AddComponent(index, component);
    }

    public void ReplaceSkeleton(string newAssetPath) {
        var index = GameComponentsLookup.Skeleton;
        var component = (SkeletonComponent)CreateComponent(index, typeof(SkeletonComponent));
        component.assetPath = newAssetPath;
        ReplaceComponent(index, component);
    }

    public void RemoveSkeleton() {
        RemoveComponent(GameComponentsLookup.Skeleton);
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

    static Entitas.IMatcher<GameEntity> _matcherSkeleton;

    public static Entitas.IMatcher<GameEntity> Skeleton {
        get {
            if (_matcherSkeleton == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Skeleton);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherSkeleton = matcher;
            }

            return _matcherSkeleton;
        }
    }
}