using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetObjectVo : MonoBehaviour{   
    public object target;

    public T getObject<T>()
    {
        return (T)target;
    }

    public void setObject<T>(T obj)
    {
        target = obj;
    }
}