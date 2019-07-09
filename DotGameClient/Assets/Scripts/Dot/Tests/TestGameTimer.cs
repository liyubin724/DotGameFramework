using Dot.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameTimer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameApplication.GTimer.AddTimerTask(0.1f, 10, (obj) =>
          {
              Debug.Log($"Start ,value ={obj}");
          }, (obj) =>
          {
              Debug.Log($"Interval ,value ={obj}");
          }, (obj) =>
          {
              Debug.Log($"End ,value ={obj}");
          }, "Test UserData");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
