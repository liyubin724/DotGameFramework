﻿using Dot.Core.TimeLine.Base.Item;
using System.Collections.Generic;

namespace Dot.Core.TimeLine.Base.Tracks
{
    public class TrackLine : AEntitasEnv
    {
        public string Name { get; set; } = "Time Line Track";
        public List<AItem> items = new List<AItem>();

        private readonly List<AItem> waitingItems = new List<AItem>();
        private readonly List<AItem> runningItems = new List<AItem>();
        private float elapsedTime = 0f;

        public void DoUpdate(float deltaTime)
        {
            if(elapsedTime == 0f && waitingItems.Count ==0 && items.Count>0)
            {
                waitingItems.AddRange(items);
            }

            float previousTime = elapsedTime;
            elapsedTime += deltaTime;

            if (runningItems.Count == 0 && waitingItems.Count == 0)
            {
                return;
            }
            while(waitingItems.Count>0)
            {
                AItem item = waitingItems[0];
                if(item.FireTime>elapsedTime)
                {
                    break;
                }
                if(item.FireTime>=previousTime && item.FireTime<elapsedTime)
                {
                    runningItems.Add(item);
                    waitingItems.RemoveAt(0);

                    item.Initialize(contexts, services, entity);
                }
            }
            
            for (int i=runningItems.Count-1;i>=0;--i)
            {
                AItem item = runningItems[i];
                if (item is AEventItem eventItem)
                {
                    if (previousTime <= eventItem.FireTime && elapsedTime > eventItem.FireTime)
                    {
                        eventItem.Trigger();
                    }
                    runningItems.RemoveAt(i);
                    item.DoReset();
                }else if(item is AActionItem actionItem)
                {
                    if (previousTime <= actionItem.FireTime && elapsedTime > actionItem.FireTime)
                    {
                        actionItem.Enter();
                    }
                    else if (previousTime <= actionItem.EndTime && elapsedTime > actionItem.EndTime)
                    {
                        actionItem.Exit();
                        runningItems.RemoveAt(i);
                        item.DoReset();
                    }
                    else if (previousTime >= actionItem.FireTime && elapsedTime <= actionItem.EndTime)
                    {
                        actionItem.DoUpdate(deltaTime);
                    }else
                    {
                        runningItems.RemoveAt(i);
                        item.DoReset();
                    }
                }
            }
        }
        
        public void Stop()
        {
            runningItems.ForEach((item) =>
            {
                if (item is AActionItem actionItem)
                {
                    actionItem.Stop();
                }
            });
            DoReset();
        }

        public void Pause()
        {
            runningItems.ForEach((item) =>
            {
                if (item is AActionItem actionItem)
                {
                    actionItem.Pause();
                }
            });
        }

        public void Resume()
        {
            runningItems.ForEach((item) =>
            {
                if (item is AActionItem actionItem)
                {
                    actionItem.Pause();
                }
            });
        }

        public override void DoReset()
        {
            base.DoReset();
            runningItems.ForEach((item) =>
            {
                item.DoReset();
            });
            runningItems.Clear();
            waitingItems.Clear();
            elapsedTime = 0f;
        }
    }
}