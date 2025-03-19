using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject object1;

    void Start()
    {
        Instantiate(object1, new Vector3(14, 33, 0), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(object1, new Vector3(14, 33, 0), Quaternion.identity);
        }
    }
}
