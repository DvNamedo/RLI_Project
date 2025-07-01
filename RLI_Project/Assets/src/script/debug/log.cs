using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class log : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PerlinNoise pn = new PerlinNoise(40, 40, 20306, 6203);

        Debug.Log(pn.GetPerlinMatrix().ToString(3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
