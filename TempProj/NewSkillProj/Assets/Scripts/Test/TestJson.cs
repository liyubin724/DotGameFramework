using Game.TimeLine;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

public class TestJson : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EffectAction eAction = new EffectAction();
        eAction.Index = 100;
        eAction.FireTime = 1.0f;
        eAction.EffectConfigID = 1;
        eAction.EmitIndex = 1;
        eAction.Duration = 11.1f;

        JsonData jsonData = new JsonData();
        jsonData["name"] = typeof(EffectAction).FullName;

        PropertyInfo[] pInfos = eAction.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach(var p in pInfos)
        {
            if(p.GetGetMethod()==null || p.GetSetMethod() == null)
            {
                continue;
            }
            jsonData[p.Name] = new JsonData(p.GetValue(eAction));
        }
        Debug.Log(jsonData.ToJson());

        string jsonStr = jsonData.ToJson();
        JsonData parseJsonData = JsonMapper.ToObject(jsonStr);
        Type refType = Type.GetType((string)parseJsonData["name"]);
        
        var typeInstance = refType.Assembly.CreateInstance((string)parseJsonData["name"]);
        var keys = parseJsonData.Keys;
        foreach(var key in keys)
        {
            if (key == "name")
                continue;
            var value = parseJsonData[key];
            if(value.IsInt)
            {
                refType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance).SetValue(typeInstance, (int)value);
            }else if(value.IsDouble)
            {
                refType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance).SetValue(typeInstance, (float)value);
            }
        }

        Debug.Log((typeInstance as EffectAction).Index);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
