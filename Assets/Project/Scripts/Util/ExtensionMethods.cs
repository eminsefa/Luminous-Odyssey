using System;
using UnityEngine;

namespace Util
{
    public static class ExtensionMethods
    {
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            if (aParent != null)
            {
                var result = aParent.Find(aName);
                if (result != null)
                    return result;

                foreach (Transform child in aParent)
                {
                    result = child.FindDeepChild(aName);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
        
        public static T FindDeepChild<T>(this Transform aParent, string aName)
        {
            T result = default(T);

            var transform = aParent.FindDeepChild(aName);

            if (transform != null)
            {
                result = (typeof(T) == typeof(GameObject)) ? (T)Convert.ChangeType(transform.gameObject, typeof(T)) : transform.GetComponent<T>();
            }

            if (result == null)
            {
                Debug.LogError($"FindDeepChild didn't find: '{aName}' on GameObject: '{aParent.name}'");
            }

            return result;
        }

        public enum Axis
        {
            x,
            y,
            z,
        }
        public static void SetPos(this Transform i_Tr, Axis i_Axis, float i_Value)
        {
            var pos = i_Tr.position;
            switch (i_Axis)
            {
                case Axis.x:
                    pos.x = i_Value;
                    break;
                case Axis.y:
                    pos.y = i_Value;
                    break;
            }
            i_Tr.position = pos;
        }
        
        public static void SetLocalScale(this Transform i_Tr, Axis i_Axis, float i_Value)
        {
            var scale = i_Tr.localScale;
            switch (i_Axis)
            {
                case Axis.x:
                    scale.x = i_Value;
                    break;
                case Axis.y:
                    scale.y = i_Value;
                    break;
                case Axis.z:
                    scale.z = i_Value;
                    break;
            }
            i_Tr.localScale = scale;
        }
    }
}