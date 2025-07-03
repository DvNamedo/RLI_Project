using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeBase
{
    public int ID;
    public int Depth { get; protected set; }
    public Vector2Int Position { get; set; }

    public MapNodeBase(Vector2Int position)
    {
        this.Position = position;
        ID = Vars.NewMapNodeID;
    }

}
