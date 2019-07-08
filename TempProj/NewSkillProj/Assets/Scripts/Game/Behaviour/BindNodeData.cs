using System;
using UnityEngine;


public enum BindNodeType
{
    Main,
    Sub,
    Super,
    Furnace,
}

[Serializable]
public class BindNodeData
{
    public BindNodeType nodeType = BindNodeType.Main;
    public Transform nodeTransform = null;
}