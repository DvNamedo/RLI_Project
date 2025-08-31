using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Singleton pattern base on Unity Library 
/// </summary>
public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Represents a collection of object pools, where each pool is associated with a specific key.
    /// </summary>
    /// <remarks>
    /// The dictionary maps a <see cref="GameObject"/> key to 
    /// a stack of pooled <see cref="GameObject"/> instances. <br></br>
    /// This structure is typically used to manage object pooling, allowing efficient. <br></br>
    /// usually key is parents and value is children that instantiated by parents in the unity hierarchy.
    /// </remarks>
    private Dictionary<GameObject, Stack<GameObject>> PoolDictionary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="popped"></param>
    /// <param name="degree"></param>
    /// <returns></returns>
    public bool TryPooledGameObject(GameObject obj, Vector2 position, out GameObject popped, float degree = 0)
    {
        if (PoolDictionary.TryGetValue(obj, out Stack<GameObject> pool) && pool.TryPop(out GameObject go))
        {
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(0, 0, degree);
            go.SetActive(true);

            popped = go;
            return true;
        }

        popped = null;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="multiply"></param>
    public void PoolingConcurrentlyGameObject(GameObject obj, uint multiply)
    {
        for (uint i = 0; i < multiply; i++)
        {
            GameObject instance = Instantiate(obj, );
            instance.SetActive(false);
            PoolDictionary[obj].Push(instance);
        }
    }

    // Pooling GameObject 
    


    // 특정 gameobject가 속한 scene의 나머지 모든 gameobject를 반환하는 기능의 함수

    

}
