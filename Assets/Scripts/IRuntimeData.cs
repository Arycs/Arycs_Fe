using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Models
{
    public interface IRuntimeData<T> : ICloneable where T:class,IRuntimeData<T> ,new()
    {
        void CopyTo(T data);

        new T Clone();
    }
}