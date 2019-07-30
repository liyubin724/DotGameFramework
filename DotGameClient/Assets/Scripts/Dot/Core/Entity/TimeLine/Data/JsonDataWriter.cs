using LitJson;
using System;
using System.Reflection;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.TimeLine.Data
{
    public static class JsonDataWriter
    {
        public static JsonData WriteData(TimeLineData data)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);

            JsonData groupsData = new JsonData();
            groupsData.SetJsonType(JsonType.Array);
            data.groupList.ForEach((group) =>
            {
                groupsData.Add(WriteGroup(group));
            });
            jsonData[TimeLineConst.GROUPS] = groupsData;

            return jsonData;
        }

        public static JsonData WriteGroup(TrackGroup group)
        {
            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);
            if(group !=null)
            {
                jsonData[TimeLineConst.NAME] = group.Name;
                jsonData[TimeLineConst.TOTALTIME] = group.TotalTime;
                jsonData[TimeLineConst.CANREVERT] = group.CanRevert;

                JsonData tracksData = new JsonData();
                tracksData.SetJsonType(JsonType.Array);
                group.tracks.ForEach((track) =>
                {
                    tracksData.Add(WriteTrack(track));
                });
                jsonData[TimeLineConst.TRACKS] = tracksData;
            }
            return jsonData;
        }

        public static JsonData WriteTrack(TrackLine track)
        {
            if (track == null) return null;

            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);
            jsonData[TimeLineConst.NAME] = track.Name;

            JsonData itemsData = new JsonData();
            itemsData.SetJsonType(JsonType.Array);
            track.items.ForEach((item) =>
            {
                itemsData.Add(WriteItem(item));
            });
            jsonData[TimeLineConst.ITEMS] = itemsData;

            return jsonData;
        }

        public static JsonData WriteItem(AItem item)
        {
            return WriteToJson(item);
        }

        public static JsonData WriteToJson<T>(T data) where T:class
        {
            if (data == null)
                return null;

            JsonData jsonData = new JsonData();
            jsonData.SetJsonType(JsonType.Object);

            jsonData[TimeLineConst.NAME] = data.GetType().FullName;
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
