using UnityEngine;

public class PlayerView : PackRootView
{
    public PlayerView(string name) : base(name)
    {
    }

    public PlayerView(string name, Transform parent) : base(name, parent)
    {
    }
}
