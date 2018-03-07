using System;
using UnityEngine;

public class TestWrapperScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Debug.Log(CPPTOUnityLibWrapper.add_to_42(51));
    }
}