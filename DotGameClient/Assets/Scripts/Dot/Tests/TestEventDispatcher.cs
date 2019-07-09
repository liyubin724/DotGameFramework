using Dot.Core;
using UnityEngine;

public class TestEventDispatcher : MonoBehaviour
{
    void Start()
    {
        GameApplication.GEvent.RegisterEvent(1, (eData) =>
        {
            Debug.Log($"Hanle Eevent,value = {eData.GetValue<string>(0)}");
        });
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Trigger Event"))
        {
            GameApplication.GEvent.TriggerEvent(1, 0, "Test Event");
        }

        if (GUILayout.Button("Trigger Delay Event"))
        {
            GameApplication.GEvent.TriggerEvent(1, 5, "Test Delay Event");
        }
    }
}
