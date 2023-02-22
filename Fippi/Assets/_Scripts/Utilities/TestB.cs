using System;
using UnityEngine;
public class TestB : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<TestA>().TestEvent += ReactOnBool;
        enabled = false;
    }
    private void Start()
    {

    }
    private void ReactOnBool(bool b)
    {
        if (b)
        {
            Debug.Log("b is true");
            enabled = true;
        }
        else
        {
            Debug.Log("b is false");
            enabled = false;
        }
    }
}