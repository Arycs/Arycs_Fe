using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public static class ScenarioBlackboard  
    {
        [Serializable]
        public struct VarValuePair
        {
            public string name;
            public int value;

            public VarValuePair(string name, int value)
            {
                this.name = name;
                this.value = value;
            }
        }

        private readonly static Dictionary<string, VarValuePair> s_VarValues = new Dictionary<string, VarValuePair>();
        
        //省略 字典操作 具体实现
        //应该包含 Contains ,Set ,Get, TryGet, ToArray
        public static bool Contains(string name)
        {
            return s_VarValues.ContainsKey(name);
        }

        public static void Set(string name, int value)
        {
            s_VarValues[name] = new VarValuePair(name, value);
        }

        public static bool TryGet(string name, out int value)
        {
            value = 0;
            if (!s_VarValues.ContainsKey(name))
            {
                return false;
            }

            value = s_VarValues[name].value;
            return true;
        }

        public static int Get(string name, int defaultValue = 0)
        {
            int value = defaultValue;
            if (!TryGet(name,out value))
            {
                Set(name,value);
            }

            return value;
        }

        public static bool Rmove(string name)
        {
            return s_VarValues.Remove(name);
        }

        public static void Clear()
        {
            s_VarValues.Clear();
        }

        public static VarValuePair[] ToArray()
        {
            return s_VarValues.Values.ToArray();
        }
    }
}