using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Group;
using Dot.Core.TimeLine.Base.Item;
using Dot.Core.TimeLine.Base.Tracks;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

namespace Dot.Core.TimeLine.Data
{
    public static class JsonDataReader
    {
        public static TimeLineController ReadController(JsonData jsonData)
        {
            if (jsonData == null) return null;

            TimeLineController controller = new TimeLineController();

            if(jsonData.ContainsKey(DataConst.TIME_LINE_GROUPS))
            {
                JsonData groupsJsonData = jsonData[DataConst.TIME_LINE_GROUPS];
                for (var i = 0; i < groupsJsonData.Count; ++i)
                {
                    TrackGroup group = ReadGroup(groupsJsonData[i]);
                    if (group != null)
                    {
                        controller.groups.Add(group);
                    }
                }
            }

            return controller;
        }

        public static TrackGroup ReadGroup(JsonData jsonData)
        {
            if (jsonData == null) return null;
            if (!jsonData.ContainsKey(DataConst.TIME_LINE_NAME)) return null;

            TrackGroup group = new TrackGroup
            {
                Name = (string)jsonData[DataConst.TIME_LINE_NAME],
                TotalTime = (float)jsonData[DataConst.TIME_LINE_GROUP_TOTALTIME],
                IsAwaysRun = (bool)jsonData[DataConst.TIME_LINE_GROUP_ISEND],

                //conditionCompose = ReadConditionCompose(jsonData[TimeLineConst.TIME_LINE_CONDITION_COMPOSE])
            };

            JsonData tracksJsonData = jsonData[DataConst.TIME_LINE_TRACKS];
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

        public static AComposeCondition ReadConditionCompose(JsonData jsonData)
        {
            AComposeCondition result = null;//new ATimeLineComposeCondition();
            if(jsonData!=null && jsonData.Count>0)
            {
                for(int i =0;i<jsonData.Count;++i)
                {
                    ACondition condition = ReadCondition(jsonData[i]);
                    if(condition!=null)
                    {
                        result.conditions.Add(condition);
                    }
                }
            }
            return result;
        }

        public static TrackLine ReadTrack(JsonData jsonData)
        {
            if (jsonData == null) return null;
            if(!jsonData.ContainsKey(DataConst.TIME_LINE_NAME))
            {
                return null;
            }

            TrackLine track = new TrackLine();
            track.Name = (string)jsonData[DataConst.TIME_LINE_NAME];

            JsonData itemsJsonData = jsonData[DataConst.TIME_LINE_ITEMS];
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

        public static ACondition ReadCondition(JsonData jsonData)
        {
            return ReadFromJson<ACondition>(jsonData);
        }

        private static T ReadFromJson<T>(JsonData jsonData) where T:class
        {
            if (jsonData == null)
                return null;
            if(!jsonData.ContainsKey(DataConst.TIME_LINE_NAME))
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
