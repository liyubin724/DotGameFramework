using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Data;
using Game.TimeLine;
using LitJson;
using System.IO;
using UnityEngine;

public class TestTimeLineJson : MonoBehaviour
{
    public TextAsset asset = null;

    private JsonData jsonData;
    void Start()
    {
        jsonData = JsonMapper.ToObject(asset.text);        
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Convert To Item"))
        {
            JsonData itemJsonData = jsonData["groups"][0]["tracks"][0]["items"][0];

            CreateEmitEvent ceEvent = (CreateEmitEvent)TimeLineReader.ReadItem(itemJsonData);
            Debug.Log(ObjectDumper.Dump(ceEvent, DumpStyle.Console));

            string val = TimeLineWriter.WriteToJson(ceEvent).ToJson();
            Debug.Log(val);
        }
        if(GUILayout.Button("Convert To Condition"))
        {
            JsonData conditionJsonData = jsonData["groups"][0]["condition_compose"][0];
            WaitingTimeCondition wtCondition = (WaitingTimeCondition)TimeLineReader.ReadCondition(conditionJsonData);
            Debug.Log(wtCondition);
        }

        if(GUILayout.Button("Convert To Controller"))
        {
            TimeLineController controller = TimeLineReader.ReadController(jsonData);
            Debug.Log(ObjectDumper.Dump( controller, DumpStyle.Console));

            string configJson= TimeLineWriter.WriteController(controller).ToJson();
            File.WriteAllText(@"D:\config.json", configJson);
        }
    }
}
