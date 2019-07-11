using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Groups;
using Dot.Core.TimeLine.Base.Item;
using Dot.Core.TimeLine.Base.Tracks;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.TimeLine.Data
{
    public static class TimeLineWriter
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
            jsonData[TimeLineConst.TIME_LINE_GROUPS] = groupsData;

            return jsonData;
        }

        public static JsonData WriteGroup(TimeLineGroup group)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);
            if(group !=null)
            {
                jsonData[TimeLineConst.TIME_LINE_NAME] = group.Name;
                jsonData[TimeLineConst.TIME_LINE_GROUP_TOTALTIME] = group.TotalTime;
                jsonData[TimeLineConst.TIME_LINE_GROUP_ISEND] = group.IsAwaysRun;

                //jsonData[TimeLineConst.TIME_LINE_CONDITION_COMPOSE] = WriteConditionCompose(group.conditionCompose);

                JsonData tracksData = new JsonData();
                tracksData.SetJsonType(JsonType.Array);
                for(int i =0;i<group.tracks.Count;i++)
                {
                    tracksData.Add(WriteTrack(group.tracks[i]));
                }
                jsonData[TimeLineConst.TIME_LINE_TRACKS] = tracksData;
            }
            return jsonData;
        }

        public static JsonData WriteConditionCompose(ATimeLineComposeCondition conditionCompose)
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

        public static JsonData WriteTrack(TimeLineTrack track)
        {
            if (track == null) return null;

            JsonData jsonData = new JsonData
            {
                [TimeLineConst.TIME_LINE_NAME] = track.Name
            };

            JsonData itemsData = new JsonData();
            itemsData.SetJsonType(JsonType.Array);
            for (int i = 0; i < track.items.Count; i++)
            {
                ATimeLineItem item = track.items[i];
                JsonData itemData = WriteItem(item);
                if(itemData!=null)
                    itemsData.Add(itemData);
            }
            jsonData[TimeLineConst.TIME_LINE_ITEMS] = itemsData;

            return jsonData;
        }

        public static JsonData WriteItem(ATimeLineItem item)
        {
            return WriteToJson(item);
        }

        public static JsonData WriteConditoin(ATimeLineCondition condition)
        {
            return WriteToJson(condition);
        }

        public static JsonData WriteToJson<T>(T data) where T:class
        {
            if (data == null)
                return null;

            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);

            jsonData[TimeLineConst.TIME_LINE_NAME] = data.GetType().FullName;
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
