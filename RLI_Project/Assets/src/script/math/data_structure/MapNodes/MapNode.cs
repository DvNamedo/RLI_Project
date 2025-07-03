using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapNode : MapNodeBase
{
    public MapNodeBase Parent {  get; set; }

    public MapNode(Vector2Int position, MapNode parent) : base(position)
    {

        if (object.ReferenceEquals(parent, null))
            throw new NullReferenceException();
        Parent = parent;
        Depth = Parent.Depth + 1;
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




