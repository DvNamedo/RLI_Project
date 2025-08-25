using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMapNode : MapNodeBase, INodeConnectivity
{
    public const int MaxChildCount = 4;
    public List<MapNode> Child { get; set; } = new();

    public RootMapNode(Vector2Int position) : base(position)
    {
        Init();
    }

    public void Init()
    {
        Child.Clear();
        for (int i = 0; i < MaxChildCount; i++)
        {
            Child.Add(null);
        }
    }


    // 연쇄반응으로, 자기 아래의 모든 연결을 다 돌 것
    public bool IsPositionValid()
    {
        bool isValid = true;
        for (int i = 0; i < MaxChildCount; i++)
        {
            if (Child[i] != null)
                isValid = isValid && Child[i].IsPositionValid();
        }
        return isValid;
    }

    public bool IsChildQuantityValid()
    {
        return Child.Count <= MaxChildCount;
    }
}
