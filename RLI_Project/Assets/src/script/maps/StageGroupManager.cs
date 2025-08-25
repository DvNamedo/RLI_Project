using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public struct StageInfo
{
    public Biom id;
}

public class StageGroupManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // mapTree: 맵의 연결구조 인스턴스
        // pathes_depth
        // PathRoom 구현체
        // seed
        // privious stage info -> biom list(last -> idx[^1]), stage number, 
        MapTree mapTree = new MapTree(Vector2Int.zero);

        Shape shape = new Shape();

        int[] path_interval = { 2, 3, 1, 4, 5 }; // 임시
        int shape_size = 1 + path_interval.Sum();
        bool[] was_action_idx = new bool[shape_size];
        List<int> actions = new List<int>();
        actions.Add(-1);
        actions.Add(0);
        actions.Add(1);

        List<int> unused_actions = actions;
        
        shape = mapTree.GetStraightShape(path_interval);
        
        foreach( int i in Enumerable.Range(0, shape_size).ToArray().Shuffled())
        {
            unused_actions.Shuffled();
            foreach( int action in unused_actions) {
                if (!mapTree.BendingShape(ref shape, i, action))
                    unused_actions.Remove(action);
            }

            unused_actions = actions;
        }

        mapTree.AssignShapeToTree(shape);

        
        
        
        

        

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
