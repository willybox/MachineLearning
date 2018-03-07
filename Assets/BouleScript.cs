using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouleScript : MonoBehaviour {

    [SerializeField]
    private Transform[] sphereTransform;

    // Use this for initialization
    void Start()
    {
        Debug.Log("test");

        sphereTransform[0].position += Vector3.down * 2f;
        sphereTransform[2].position += Vector3.up * 2f;
        sphereTransform[3].position += Vector3.forward * 2f;
    }

}
