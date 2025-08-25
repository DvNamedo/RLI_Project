using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullConnectedMapNode : MapNode, INodeConnectivity
{
    public const int MaxChildCount = 3;
    public List<MapNode> Child { get; set; } = new();

    public MapNode Sub { get; set; }

    public FullConnectedMapNode(Vector2Int position, MapNode parent ) : base(position, parent)
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
    public override bool IsPositionValid()
    {
        bool isValid = true;
        for (int i = 0; i < MaxChildCount; i++)
        {
            if(Child[i] != null)
                isValid = isValid && Child[i].IsPositionValid();
        }
        return isValid && base.IsPositionValid();
    }

    public bool IsChildQuantityValid()
    {
        return Child.Count <= MaxChildCount;
    }

}
