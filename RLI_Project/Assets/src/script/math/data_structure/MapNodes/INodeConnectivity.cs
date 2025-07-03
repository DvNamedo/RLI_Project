using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeConnectivity
{
    public int MaxChildCount { get; }
    public virtual List<MapNode> Child
    {
        get
        {
            return Child;
        }
        set
        {
            Child = value;
        }
    }


    public bool IsChildQuantityValid()
    {
        return Child.Count <= MaxChildCount;
    }

    // 나중에 생성 가능한지 판별하는 코드가 있을수도 있겠음.
}