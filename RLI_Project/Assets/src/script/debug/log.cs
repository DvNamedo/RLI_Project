using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class log : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PerlinNoise pn = new PerlinNoise(10, 10, 2006, 603);

        Debug.Log(pn.getPerlinMatrix().ToString());
        Debug.Log("a");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
