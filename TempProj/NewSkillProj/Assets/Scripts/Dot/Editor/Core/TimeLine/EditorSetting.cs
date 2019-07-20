using Dot.Core.TimeLine;
using Dot.Core.TimeLine.Base.Condition;
using System;
using System.Linq;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class EditorSetting
    {
        public float timeStep = 0.1f;
        public int pixelForSecond = 100;
        public int timeHeight = 20;
        public int propertyWidth = 280;
        public int trackWidth = 100;
        public int groupWidth = 100;
        public int trackHeight = 40;
        public int groupHeight = 60;

        //runtime setting
        public Vector2 scrollPos = Vector2.zero;
        public bool isChanged = false;

        public float PixelForStep { get { return pixelForSecond * timeStep; } }

        private Type[] itemTypes = null;
        public Type[] ItemTypes
        {
            get
            {
                if(itemTypes == null)
                {
                    itemTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                 from type in assembly.GetExportedTypes()
                                 where type.IsSubclassOf(typeof(AEventItem)) || type.IsSubclassOf(typeof(AActionItem))
                                 select type).ToArray();
                }
                if (itemTypes == null)
                {
                    itemTypes = new Type[0];
                }
                return itemTypes;
            }
        }

        private Type[] conditionTypes = null;
        public Type[] ConditionTypes
        {
            get
            {
                if(conditionTypes == null)
                {
                    conditionTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                      from type in assembly.GetExportedTypes()
                                      where type.IsSubclassOf(typeof(ACondition))
                                      select type).ToArray();
                }
                if (conditionTypes == null) conditionTypes = new Type[0];

                return conditionTypes;
            }
        }
    }
}
