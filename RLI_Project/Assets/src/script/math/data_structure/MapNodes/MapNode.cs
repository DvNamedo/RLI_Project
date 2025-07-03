using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapNode
{
    public int ID;
    public Vector2Int Position { get; set; }
    public int Depth { get; private set; }

    public MapNode Parent {  get; set; }

    public MapNode(Vector2Int position, MapNode parent)
    {
        Position = position;
        Parent = parent;

        Depth = parent != null ? Parent.Depth + 1 : 0;
        ID = Vars.NewMapNodeID;

    }

    public virtual bool IsPositionValid()
    {
        if (Parent != null)
        {
            var eps = 0.000001f;
            return Mathf.Abs(Vector2Int.Distance(this.Position, Parent.Position) - 1) < eps;
        }
        return true;
    }



}




