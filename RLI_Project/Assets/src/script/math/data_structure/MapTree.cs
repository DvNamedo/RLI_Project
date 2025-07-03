using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;


public class MapTree
{
    Dictionary<int, MapNodeBase> TreeDict = new Dictionary<int, MapNodeBase>();

    RootMapNode Root;

    public MapTree(Vector2Int pivot) 
    {
        Root = new RootMapNode(pivot);
        TreeDict[Root.ID] = Root;

    }

    public List<MapNodeBase> BendingShape(List<MapNodeBase> original, int axis_node_idx, int action_dir)
    {
        if (axis_node_idx == original.Count - 1)
            return original;

        for (int i = axis_node_idx + 1; i < original.Count; i++) 
        {

            if (original[axis_node_idx] is PathMapNode) // path
            {
                return original;
            }
            else // nither path
            {
                original[i].Position = original[axis_node_idx].Position.RotateAround(original[i].Position, action_dir * Mathf.PI / 2f);
            }
        }

        return original;
    }

    public List<MapNodeBase> GetStraightShape(RootMapNode root, int[] path_interval)
    {
        var shape = new List<MapNodeBase>();
        
        MapNodeBase recent_node = root;
        shape.Add(recent_node);

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

    public void AssignShapeToTree(List<MapNodeBase> nodes)
    {
        foreach (var node in nodes)
        {
            TreeDict[node.ID] = node;
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
        if (IsNewPosition(parent.Position + dir))
        {
            var child_pos = parent.Position + dir;
            parent.Child = Activator.CreateInstance(typeof(TLeaf)
                                                    , child_pos
                                                    , parent
                                                    ) as TLeaf;
            TreeDict[parent.Child.ID] = parent.Child;
            return true;
        }
        return false;
    }

    public bool AddAtFulCont<TLeaf>(FullConnectedMapNode parent, int child_dir) where TLeaf: MapNode, new()
    {
        var parent_dir_vec = (-(parent.Position - parent.Parent.Position));

        var parent_dir = Mathf.Round((-2f / Mathf.PI) * Mathf.Atan2(parent_dir_vec.y, parent_dir_vec.x));

        var child_idx = child_dir < parent_dir ? child_dir : child_dir - 1;

        var child_pos = parent.Position + Vector2Int.zero.RotateAround(Vector2Int.right, Mathf.PI * child_dir / 2f);
        if (IsNewPosition(child_pos))
        {
            parent.Child[child_idx] = Activator.CreateInstance(typeof(TLeaf)
                                                              , child_pos
                                                              , parent
                                                              ) as TLeaf;
            TreeDict[parent.Child[child_idx].ID] = parent.Child[child_idx];
            return true;
        }
        return false;
    }

    public bool AddAtRoot<TLeaf>(RootMapNode parent, int child_dir) where TLeaf: MapNode, new()
    {
        var child_pos = parent.Position + Vector2Int.zero.RotateAround(Vector2Int.right, Mathf.PI * child_dir / 2f);
        if (IsNewPosition(child_pos)) 
        {
            parent.Child[child_dir] = Activator.CreateInstance(typeof(TLeaf)
                                                              , child_pos
                                                              , parent
                                                              ) as TLeaf;
            TreeDict[parent.Child[child_dir].ID] = parent.Child[child_dir];
            return true;
        }
        return false;
    }

    public Vector2Int GetRandomPositionOnValid(params Vector2Int[] dirs)
    {
        if (dirs == null || dirs.Length == 0)
            throw new ArgumentException("dirs must not be empty.", nameof(dirs));

        // 1) 로컬 복사본 생성
        var list = new List<Vector2Int>(dirs);

        // 2) Fisher–Yates 셔플 (int Range: 상한 배제라 i+1)
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        // 3) 유효한 위치를 찾는 즉시 반환
        foreach (var next in list)
            if (IsNewPosition(next))
                return next;

        return Root.Position;
    }

    public bool IsOverlappedPosition(List<MapNode> shape)
    {
        var checkedPositionSet = new HashSet<Vector2Int>();
        
        foreach (MapNodeBase node in shape)
        {
            checkedPositionSet.Add(node.Position);
        }

        return shape.Count != checkedPositionSet.Count;
    }

    public bool IsOverlappedPosition()
    {
        var checkedPositionSet = new HashSet<Vector2Int>();
        var nodeList = TreeDict.Values;
        foreach (MapNodeBase node in nodeList)
        {
            checkedPositionSet.Add(node.Position);
        }

        return nodeList.Count != checkedPositionSet.Count;

    }

    public bool IsNewPosition(Vector2Int next)
    {
        var checkedPositionSet = new HashSet<Vector2Int>();
        var nodeList = TreeDict.Values;
        foreach (MapNodeBase node in nodeList)
        {
            checkedPositionSet.Add(node.Position);
        }
        checkedPositionSet.Add(next);

        return (nodeList.Count + 1) == checkedPositionSet.Count;

    }

    public MapNodeBase this[int id]
    {
        get => TreeDict[id];
        set => TreeDict[id] = value;
    }



}
