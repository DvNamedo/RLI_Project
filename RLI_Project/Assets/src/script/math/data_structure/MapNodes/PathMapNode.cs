using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMapNode : MapNode, INodeConnectivity
{
    public int MaxChildCount { get; private set; } = 1;
    public MapNode Child { get; set; }

    public PathMapNode(Vector2Int position, MapNode parent) : base(position, parent)
    {
        
    }

    // �����������, �ڱ� �Ʒ��� ��� ������ �� �� ��
    public override bool IsPositionValid()
    {
        if (Child != null)
            return Child.IsPositionValid() && base.IsPositionValid();
        else return false;
    }

    public bool IsChildQuantityValid()
    {
        return Child != null;
    }
}
