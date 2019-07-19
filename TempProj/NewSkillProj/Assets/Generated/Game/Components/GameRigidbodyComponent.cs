//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public RigidbodyComponent rigidbody { get { return (RigidbodyComponent)GetComponent(GameComponentsLookup.Rigidbody); } }
    public bool hasRigidbody { get { return HasComponent(GameComponentsLookup.Rigidbody); } }

    public void AddRigidbody(bool newUseGravity, float newDrag, float newAngularDrag, UnityEngine.CollisionDetectionMode newMode, bool newFreezeRotation, UnityEngine.Vector3 newVelocity) {
        var index = GameComponentsLookup.Rigidbody;
        var component = (RigidbodyComponent)CreateComponent(index, typeof(RigidbodyComponent));
        component.useGravity = newUseGravity;
        component.drag = newDrag;
        component.angularDrag = newAngularDrag;
        component.mode = newMode;
        component.freezeRotation = newFreezeRotation;
        component.velocity = newVelocity;
        AddComponent(index, component);
    }

    public void ReplaceRigidbody(bool newUseGravity, float newDrag, float newAngularDrag, UnityEngine.CollisionDetectionMode newMode, bool newFreezeRotation, UnityEngine.Vector3 newVelocity) {
        var index = GameComponentsLookup.Rigidbody;
        var component = (RigidbodyComponent)CreateComponent(index, typeof(RigidbodyComponent));
        component.useGravity = newUseGravity;
        component.drag = newDrag;
        component.angularDrag = newAngularDrag;
        component.mode = newMode;
        component.freezeRotation = newFreezeRotation;
        component.velocity = newVelocity;
        ReplaceComponent(index, component);
    }

    public void RemoveRigidbody() {
        RemoveComponent(GameComponentsLookup.Rigidbody);
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

    static Entitas.IMatcher<GameEntity> _matcherRigidbody;

    public static Entitas.IMatcher<GameEntity> Rigidbody {
        get {
            if (_matcherRigidbody == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Rigidbody);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherRigidbody = matcher;
            }

            return _matcherRigidbody;
        }
    }
}
