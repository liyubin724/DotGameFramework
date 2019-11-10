using System.Collections.Generic;
using UnityEditor;

namespace DotEditor.Core.Util
{
    public static class SerializedPropertyUtil
    {
        public static void SetArray<T>(SerializedProperty property,List<T> datas) where T: UnityEngine.Object
        {
            if(property == null)
            {
                return;
            }
            if(!property.isArray)
            {

                return;
            }

            property.ClearArray();
            property.arraySize = datas.Count;
            for(int i =0;i<datas.Count;++i)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = datas[i];
            }
        }
    }
}
