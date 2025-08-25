using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public struct Shape
{
    public RootMapNode root;
    public List<MapNodeBase> shape;

    public readonly int Count() => shape.Count;

    public Shape(RootMapNode _root)
    {
        root = _root;
        shape = new List<MapNodeBase>();
    }

    public void Init(RootMapNode _root)
    {
        root = _root;
        shape = new List<MapNodeBase>();
    }

    public readonly void Add(MapNodeBase sample)
    {
        if( sample is RootMapNode ) 
            throw new ArgumentException($"{typeof(MapNodeBase)}{sample} CANNOT be a backward elememt");
        if (this.Count() == 0)
            throw new ArgumentException("First element must add with Init() or Constructor");

        shape.Add(sample);
    }

    public MapNodeBase this[int idx]
    {
        get => shape[idx];
        set 
        {
            if (this.Count() == 0)
                throw new ArgumentException("First element must add with Init() or Constructor");
            shape[idx] = value;
        }
    }
};

public class MapTree
{
    Dictionary<int, MapNodeBase> TreeDict = new Dictionary<int, MapNodeBase>();

    RootMapNode Root;

    HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

    

    public MapTree(Vector2Int pivot) 
    {
        Init(new RootMapNode(pivot));
    }

    public void Init(RootMapNode new_root)
    {
        TreeDict.Clear();
        Root = new_root;
        this[Root.ID] = Root;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="axis_node_idx">PathMapNode CANNOT Rotate</param>
    /// <param name="action_dir">can -1(right rotate), 0(none) , 1(left rotate)</param>
    /// <returns>return whether success</returns>
    public bool BendingShape(ref Shape original, int axis_node_idx, int action_dir)
    {
        var axis_node = original[axis_node_idx];
        if(axis_node is MapNode)
        {
            var axis_parent = ((MapNode)axis_node).Parent;
            var parent_dir_vec = -axis_parent.Position.DirectionTo(original[axis_node_idx].Position);
            var parent_dir = Mathf.RoundToInt(2f * Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, (Vector2)parent_dir_vec) / Mathf.PI);

            // 상대 방향으로 보정
            action_dir += parent_dir - 2;
        }


        if (axis_node_idx > original.Count() - 1)
            return false;

        for (int i = axis_node_idx + 1; i < original.Count(); i++) 
        {

            if (original[axis_node_idx] is PathMapNode) // path
            {
                return false;
            }
            else // nither path
            {
                original[i].Position = original[axis_node_idx].Position.RotateAround(original[i].Position, action_dir * Mathf.PI / 2f);
            }
        }

        return true;
    }

    public Shape GetStraightShape(int[] path_interval)
    {
        MapNodeBase recent_node = Root;
        
        var shape = new Shape(Root);

        for (int i = 0; i < path_interval.Length; i++)
        {

            // in between node
            for (int j = 0; j < path_interval[i] - 1; j++)
            {
                var node = Activator.CreateInstance(typeof(FullConnectedMapNode)
                                                   , recent_node.Position + Vector2Int.right
                                                   , recent_node
                                                   ) as FullConnectedMapNode;
                shape.Add(node);
                recent_node = node;
            }

            // path node or Exit
            if (i < path_interval.Length - 1)
            {
                var path = Activator.CreateInstance(typeof(PathMapNode)
                                   , recent_node.Position + Vector2Int.right
                                   , recent_node
                                   ) as PathMapNode;
                shape.Add(path);
                recent_node = path;
            }
            else 
            {
                var exit = Activator.CreateInstance(typeof(MapNode)
                                                   , recent_node.Position + Vector2Int.right
                                                   , recent_node
                                                   ) as MapNode;
                shape.Add(exit);
            }
        }

        return shape;
    }

    public void AssignShapeToTree(Shape nodes)
    {

        foreach (var node in nodes.shape)
        {
            TreeDict.Clear();
            this[node.ID] = node;
        }
    }

    /// <summary>
    /// Add Any Type <b>Child</b> connected on a graph with a Any Type <b>Parent</b>
    /// </summary>
    /// <typeparam name="TLeaf">means the Child object's Type to be added</typeparam>
    /// <param name="parent">means the Parent object to be used for Child Generate</param>
    /// <param name="child_dir">
    /// child_dir means the angle of half PI units in the counterclockwise direction, based on the x+ direction or
    /// </param>
    /// <returns>
    ///  true : successfully work <br/>
    /// false : position conflict <br/>
    ///  null : parent do NOT matched
    /// </returns>
    public bool? AddLeaf<TLeaf>(MapNodeBase parent, int child_dir) where TLeaf : MapNode, new()
    {
        // parent: Path
        var pparent = parent as PathMapNode;
        if (!object.Equals(null, pparent))
        {
            return AddAtPath<TLeaf>(pparent);
        }
        // parent: FullyConnected
        var fparent = parent as FullConnectedMapNode;
        if(!object.Equals(null, fparent))
        {
            return AddAtFulCont<TLeaf>(fparent, child_dir);
        }

        // parent: Root
        var rparent = parent as RootMapNode;
        if (!object.Equals(null, rparent)) 
        {
            return AddAtRoot<TLeaf>(rparent, child_dir);
        }

        // CANNOT add
        return null;
    }

    public bool AddAtPath<TLeaf>(PathMapNode parent) where TLeaf : MapNode, new()
    {
        var dir = parent.Position - parent.Parent.Position;
        if (IsFreePosition(parent.Position + dir))
        {
            var child_pos = parent.Position + dir;
            parent.Child = Activator.CreateInstance(typeof(TLeaf)
                                                    , child_pos
                                                    , parent
                                                    ) as TLeaf;
            this[parent.Child.ID] = parent.Child;
            return true;
        }
        return false;
    }

    public bool AddAtFulCont<TLeaf>(FullConnectedMapNode parent, int child_dir) where TLeaf: MapNode, new()
    {
        var parent_dir_vec = - parent.Parent.Position.DirectionTo(parent.Position);

        //var parent_dir = Mathf.Round((-2f / Mathf.PI) * Mathf.Atan2(parent_dir_vec.y, parent_dir_vec.x));
        var parent_dir = Mathf.RoundToInt(2f * Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, (Vector2)parent_dir_vec) / Mathf.PI);
        

        var child_idx = child_dir < parent_dir ? child_dir : child_dir - 1;

        var child_pos = parent.Position + Vector2Int.zero.RotateAround(Vector2Int.right, Mathf.PI * child_dir / 2f);
        if (IsFreePosition(child_pos))
        {
            parent.Child[child_idx] = Activator.CreateInstance(typeof(TLeaf)
                                                              , child_pos
                                                              , parent
                                                              ) as TLeaf;
            this[parent.Child[child_idx].ID] = parent.Child[child_idx];
            return true;
        }
        return false;
    }

    public bool AddAtRoot<TLeaf>(RootMapNode parent, int child_dir) where TLeaf: MapNode, new()
    {
        var child_pos = parent.Position + Vector2Int.zero.RotateAround(Vector2Int.right, Mathf.PI * child_dir / 2f);
        if (IsFreePosition(child_pos)) 
        {
            parent.Child[child_dir] = Activator.CreateInstance(typeof(TLeaf)
                                                              , child_pos
                                                              , parent
                                                              ) as TLeaf;
            this[parent.Child[child_dir].ID] = parent.Child[child_dir];
            return true;
        }
        return false;
    }

    public Vector2Int GetRandomPositionOnValid(params Vector2Int[] dirs)
    {
        if (dirs == null || dirs.Length == 0)
            throw new ArgumentException("dirs must not be empty.", nameof(dirs));

        dirs.ShuffleInPlace<Vector2Int>();

        // 3) 유효한 위치를 찾는 즉시 반환
        foreach (var next in dirs)
            if (IsFreePosition(next))
                return next;

        return Root.Position;
    }

    public bool IsOverlappedPosition(List<MapNode> shape)
    {
        var checkedPositionSet = new HashSet<Vector2Int>();
        
        foreach (MapNode node in shape)
        {
            if(!checkedPositionSet.Add(node.Position))
                return true;
        }
        return false;
    }
    public bool IsFreePosition(Vector2Int next) => !occupied.Contains(next);

    public MapNodeBase this[int id]
    {
        get => this.TreeDict[id];
        set
        {
            // ID 단독 교체 금지
            if (TreeDict.ContainsKey(id))
            {
                Debug.LogError($"Duplicate ID: {id}");
                return;
            }
            // 좌표 점유 시도. 실패하면 등록 금지
            if (!occupied.Add(value.Position))
            {
                Debug.LogError($"Position conflict at {value.Position}");
                return;
            }
            TreeDict.Add(id, value);
        }
    }



}
