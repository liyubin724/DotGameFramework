using LitJson;
using System;
using System.Reflection;
using UnityEngine;

namespace Dot.Core.TimeLine.Data
{
    public static class JsonDataReader
    {
        public static TimeLineData ReadData(JsonData jsonData)
        {
            if (jsonData == null) return null;

            TimeLineData data = new TimeLineData();
            if(jsonData.ContainsKey(TimeLineConst.GROUPS))
            {
                JsonData groupsJsonData = jsonData[TimeLineConst.GROUPS];
                for (var i = 0; i < groupsJsonData.Count; ++i)
                {
                    data.groupList.Add(ReadGroup(groupsJsonData[i]));
                }
            }

            return data;
        }

        public static TrackGroup ReadGroup(JsonData jsonData)
        {
            if (jsonData == null) return null;
            if (!jsonData.ContainsKey(TimeLineConst.NAME)) return null;

            TrackGroup group = new TrackGroup
            {
                Name = (string)jsonData[TimeLineConst.NAME],
                TotalTime = (float)jsonData[TimeLineConst.TOTALTIME],
                CanRevert = (bool)jsonData[TimeLineConst.CANREVERT],
            };

            JsonData tracksJsonData = jsonData[TimeLineConst.TRACKS];
            if(tracksJsonData != null && tracksJsonData.Count>0)
            {
                for(int i =0;i< tracksJsonData.Count;++i)
                {
                    TrackLine track = ReadTrack(tracksJsonData[i]);
                    if(track!=null)
                    {
                        group.tracks.Add(track);
                    }
                }
            }

            return group;
        }

        public static TrackLine ReadTrack(JsonData jsonData)
        {
            if (jsonData == null) return null;
            if(!jsonData.ContainsKey(TimeLineConst.NAME))
            {
                return null;
            }

            TrackLine track = new TrackLine();
            track.Name = (string)jsonData[TimeLineConst.NAME];

            JsonData itemsJsonData = jsonData[TimeLineConst.ITEMS];
            for (int i = 0; i < itemsJsonData.Count; ++i)
            {
                AItem item = ReadItem(itemsJsonData[i]);
                if (item != null)
                {
                    track.items.Add(item);
                }
            }
            track.items.Sort();

            return track;
        }

        public static AItem ReadItem(JsonData jsonData)
        {
            return ReadFromJson<AItem>(jsonData);
        }

        private static T ReadFromJson<T>(JsonData jsonData) where T:class
        {
            if (jsonData == null)
                return null;
            if(!jsonData.ContainsKey(TimeLineConst.NAME))
            {
                return null;
            }
            
            string typeName = (string)jsonData["Name"];
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            Type type = Type.GetType(typeName);
            if (type == null) return null;

            var resultObj = type.Assembly.CreateInstance(typeName);
            if (resultObj == null) return null;

            PropertyInfo[] pInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in pInfos)
            {
                if (pi.GetSetMethod() == null || !jsonData.ContainsKey(pi.Name))
                {
                    continue;
                }
                Type pType = pi.PropertyType;
                if (pType == typeof(Vector3))
                {
                    float x = (float)jsonData["x"];
                    float y = (float)jsonData["y"];
                    float z = (float)jsonData["z"];
                    pi.SetValue(resultObj, new Vector3(x, y, z));
                }
                else if (pType.IsEnum)
                {
                    pi.SetValue(resultObj, (int)jsonData[pi.Name]);
                }
                else if (pType == typeof(float))
                {
                    pi.SetValue(resultObj, (float)jsonData[pi.Name]);
                }
                else if (pType == typeof(int))
                {
                    pi.SetValue(resultObj, (int)jsonData[pi.Name]);
                }
                else if (pType == typeof(double))
                {
                    pi.SetValue(resultObj, (double)jsonData[pi.Name]);
                }
                else if (pType == typeof(string))
                {
                    pi.SetValue(resultObj, (string)jsonData[pi.Name]);
                }else if(pType == typeof(bool))
                {
                    pi.SetValue(resultObj, (bool)jsonData[pi.Name]);
                }
                else
                {
                    pi.SetValue(resultObj, jsonData[pi.Name]);
                }
            }

            if(resultObj!=null)
            {
                return (T)resultObj;
            }

            return null;
        }
    }
}
