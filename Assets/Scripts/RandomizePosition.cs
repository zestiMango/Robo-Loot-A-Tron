using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3( Random.Range(-6f, 95f), 7f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
