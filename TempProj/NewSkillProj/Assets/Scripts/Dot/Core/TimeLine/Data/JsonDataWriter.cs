using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Group;
using Dot.Core.TimeLine.Base.Item;
using Dot.Core.TimeLine.Base.Tracks;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.TimeLine.Data
{
    public static class JsonDataWriter
    {
        public static JsonData WriteController(TimeLineController controller)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);

            JsonData groupsData = new JsonData();
            groupsData.SetJsonType(JsonType.Array);
            for(int i =0;i<controller.groups.Count;i++)
            {
                groupsData.Add(WriteGroup(controller.groups[i]));
            }
            jsonData[DataConst.TIME_LINE_GROUPS] = groupsData;

            return jsonData;
        }

        public static JsonData WriteGroup(TrackGroup group)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);
            if(group !=null)
            {
                jsonData[DataConst.TIME_LINE_NAME] = group.Name;
                jsonData[DataConst.TIME_LINE_GROUP_TOTALTIME] = group.TotalTime;
                jsonData[DataConst.TIME_LINE_GROUP_ISAWAYSRUN] = group.IsAwaysRun;

                if(group.beginCondition!=null)
                {
                    jsonData[DataConst.TIME_LINE_GROUP_BEGIN_CONDITION] = WriteConditoin(group.beginCondition);
                }
                if(group.endCondition ==null)
                {
                    group.endCondition = new ParallelCondition();
                    group.endCondition.IsReadonly = true;
                    TimeOverCondition toCondition = new TimeOverCondition();
                    toCondition.IsReadonly = true;
                    toCondition.TotalTime = group.TotalTime;
                    group.endCondition.conditions.Add(toCondition);
                }
                jsonData[DataConst.TIME_LINE_GROUP_END_CONDITION] = WriteConditoin(group.endCondition);

                JsonData tracksData = new JsonData();
                tracksData.SetJsonType(JsonType.Array);
                for(int i =0;i<group.tracks.Count;i++)
                {
                    tracksData.Add(WriteTrack(group.tracks[i]));
                }
                jsonData[DataConst.TIME_LINE_TRACKS] = tracksData;
            }
            return jsonData;
        }

        public static JsonData WriteConditionCompose(AComposeCondition conditionCompose)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Array);
            if(conditionCompose!=null)
            {
                for(int i =0;i<conditionCompose.conditions.Count;i++)
                {
                    JsonData conditionJsonData = WriteConditoin(conditionCompose.conditions[i]);
                    if(conditionJsonData != null)
                        jsonData.Add(conditionJsonData);
                }
            }

            return jsonData;
        }

        public static JsonData WriteTrack(TrackLine track)
        {
            if (track == null) return null;

            JsonData jsonData = new JsonData
            {
                [DataConst.TIME_LINE_NAME] = track.Name
            };

            JsonData itemsData = new JsonData();
            itemsData.SetJsonType(JsonType.Array);
            for (int i = 0; i < track.items.Count; i++)
            {
                AItem item = track.items[i];
                JsonData itemData = WriteItem(item);
                if(itemData!=null)
                    itemsData.Add(itemData);
            }
            jsonData[DataConst.TIME_LINE_ITEMS] = itemsData;

            return jsonData;
        }

        public static JsonData WriteItem(AItem item)
        {
            return WriteToJson(item);
        }


        public static JsonData WriteConditoin(ACondition condition)
        {
            if(condition == null)
            {
                return null;
            }
            JsonData conditionJsonData = WriteToJson(condition);

            if (condition.GetType().IsSubclassOf(typeof(AComposeCondition)))
            {
                JsonData childJsonData = new JsonData();
                childJsonData.SetJsonType(JsonType.Array);
                foreach(var c in ((AComposeCondition)condition).conditions)
                {
                    childJsonData.Add(WriteConditoin(c));
                }
                conditionJsonData[DataConst.TIME_LINE_Child_CONDITION] = childJsonData;
            }

            return conditionJsonData;
        }

        public static JsonData WriteToJson<T>(T data) where T:class
        {
            if (data == null)
                return null;

            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);

            jsonData[DataConst.TIME_LINE_NAME] = data.GetType().FullName;
            PropertyInfo[] pInfos = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in pInfos)
            {
                if (pi.GetGetMethod() == null || pi.GetSetMethod() == null)
                    continue;

                Type pType = pi.PropertyType;
                SystemObject value = pi.GetValue(data);
                if (pType == typeof(Vector3))
                {
                    Vector3 val = (Vector3)value;
                    JsonData vData = new JsonData();
                    vData["x"] = val.x;
                    vData["y"] = val.y;
                    vData["z"] = val.z;
                    jsonData[pi.Name] = vData;
                } else if (pType.IsEnum)
                {
                    jsonData[pi.Name] = new JsonData((int)value);
                }
                else
                {
                    jsonData[pi.Name] = new JsonData(value);
                }
            }
            return jsonData;
        }
    }
}
