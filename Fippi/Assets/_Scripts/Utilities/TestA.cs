using System;
using UnityEngine;
public class TestA : MonoBehaviour
{
    public event Action<bool> TestEvent;
    private bool _b = false;
    [ContextMenu("TestEvent")]
    public void TestMethod()
    {
        _b = !_b;
        TestEvent?.Invoke(_b);
    }
    private void Start() {
        
    }
}