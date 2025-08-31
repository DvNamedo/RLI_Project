using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> PooledGameObject;
    public List<uint> InstantiateCopyAmount;



    private void Awake()
    {
        if (PooledGameObject.Count != InstantiateCopyAmount.Count)
        {
            throw new System.ArgumentException("PooledGameObject and InstantiateCopyAmount must have the same number of elements.");
        }

        for (int i = 0; i < PooledGameObject.Count; i++)
        {
            GameObject obj = PooledGameObject[i];
            uint amount = InstantiateCopyAmount[i];
            Singleton.Instance.PooledGO(obj, amount);
        }
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
