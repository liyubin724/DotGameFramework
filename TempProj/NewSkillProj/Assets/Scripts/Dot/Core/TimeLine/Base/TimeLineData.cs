using Entitas;
using System.Collections.Generic;

namespace Dot.Core.TimeLine
{
    public delegate void OnTrackGroupStart(IEntity entity,string groupName);
    public delegate void OnTrackGroupChanged(IEntity entity, string preGroupName, string curGroupName);
    public delegate void OnTrackGroupFinish(IEntity entity, string groupName);

    public sealed class TimeLineData : AEntitasEnv
    {
        public List<TrackGroup> groupList = new List<TrackGroup>();

        public OnTrackGroupStart groupStartCallback = null;
        public OnTrackGroupChanged groupChangedCallback = null;
        public OnTrackGroupFinish groupFinishCallback = null;

        private Dictionary<string, TrackGroup> groupDic = new Dictionary<string, TrackGroup>();
        private string playingGroupName = string.Empty;

        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            groupList.ForEach((group) =>
            {
                group.Data = this;
                group.Initialize(contexts, services, entity);
                groupDic.Add(group.Name, group);
            });
        }

        private string[] groupNames = null;
        public string[] GetGroupNames()
        {
            if(groupNames == null)
            {
                groupNames = new string[groupList.Count];
                for(int i =0;i<groupList.Count;++i)
                {
                    groupNames[i] = groupList[i].Name;
                }
            }
            return groupNames;
        }

        public bool HasGroup(string groupName)
        {
            if(isInit)
            {
                return groupDic.ContainsKey(groupName);
            }else
            {
                foreach(var g in groupList)
                {
                    if(g.Name == groupName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsPlaying() => !string.IsNullOrEmpty(playingGroupName);

        public bool IsPlaying(string groupName)
        {
            return IsPlaying() && groupName == playingGroupName;
        }
        
        public void Play(string groupName)
        {
            if (!isInit)
            {
                services.logService.Log(DebugLogType.Error, "Not init");
                return;
            }
            if(!HasGroup(groupName))
            {
                services.logService.Log(DebugLogType.Error, "Not Found");
                return;
            }
            groupChangedCallback?.Invoke(entity,playingGroupName, groupName);
            if(IsPlaying())
            {
                Stop();
            }
            playingGroupName = groupName;
        }

        public void Stop()
        {
            if(IsPlaying())
            {
                TrackGroup group = groupDic[playingGroupName];
                group.Stop();
                playingGroupName = string.Empty;
            }
        }

        public void DoUpdate(float deltaTime)
        {
            if(isInit && IsPlaying())
            {
                groupDic[playingGroupName].DoUpdate(deltaTime);
            }
        }

        public override void DoReset()
        {
            groupList.ForEach((group) =>
            {
                group.DoReset();
            });

            groupStartCallback = null;
            groupChangedCallback = null;
            groupFinishCallback = null;
            groupDic.Clear();
            playingGroupName = string.Empty;

            base.DoReset();
        }

        internal void OnGroupFinish(TrackGroup group)
        {
            playingGroupName = string.Empty;
            groupFinishCallback?.Invoke(entity, group.Name);
        }

        internal void OnGroupStart(TrackGroup group)
        {
            groupStartCallback?.Invoke(entity, group.Name);
        }
    }
}
