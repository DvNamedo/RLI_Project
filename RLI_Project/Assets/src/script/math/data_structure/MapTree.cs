using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTree
{
    Dictionary<int, MapNode> TreeDict = new Dictionary<int, MapNode>();

    public MapNode this[int id]
    {
        get => TreeDict[id];
        set => TreeDict[id] = value;
    }



}
