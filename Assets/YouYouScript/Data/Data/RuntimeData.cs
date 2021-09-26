using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Arycs_Fe.Models;
using UnityEngine;

namespace Arycs_Fe.Models
{
    public class RuntimeData<T> : IRuntimeData<T> where T : RuntimeData<T>, new()
    {
        public virtual void CopyTo(T data)
        {
            if (data == null)
            {
                Debug.LogError("RuntimeData -> CopyTO : data is null");
                return;
            }

            if (data == this)
            {
                return;
            }

            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField |
                                                BindingFlags.SetField);
            for (int i = 0; i < fields.Length; i++)
            {
                object val = fields[i].GetValue(this);
                fields[i].SetValue(data,val);
            }
        }

        public virtual T Clone()
        {
            T clone = new T();
            CopyTo(clone);
            return clone;
        }

        object ICloneable.Clone()
        {
            return ((IRuntimeData<T>) this).Clone();
        }
    }
}