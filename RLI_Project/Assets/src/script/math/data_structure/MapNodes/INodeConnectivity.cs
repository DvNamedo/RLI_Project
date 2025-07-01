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

    // ���߿� ���� �������� �Ǻ��ϴ� �ڵ尡 �������� �ְ���.
}