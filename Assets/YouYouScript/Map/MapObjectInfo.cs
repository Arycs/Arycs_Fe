using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using UnityEngine;

public abstract class MapObjectInfo
{
    public abstract MapObjectType objectType { get; }

    public int x;
    public int y;
}

public class ObstacleInfo : MapObjectInfo
{
    public override MapObjectType objectType
    {
        get { return MapObjectType.Obstacle; }
    }

    public string prefab;
}

[Serializable]
public abstract class ClassInfo : MapObjectInfo
{
    public override MapObjectType objectType
    {
        get { return MapObjectType.Class; }
    }

    public AttitudeTowards attitudeTowards;

    public abstract RoleType roleType { get; }

    public int id;
}

[Serializable]
public class ClassUniqueInfo : ClassInfo
{
    public override RoleType roleType
    {
        get { return RoleType.Unique; }
    }
}

[Serializable]
public class ClassFollowingInfo : ClassInfo
{
    public override RoleType roleType
    {
        get { return RoleType.Following; }
    }

    public int level;
    public int[] items;
}