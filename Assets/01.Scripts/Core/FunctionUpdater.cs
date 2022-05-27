using System;
using UnityEngine;

public class FunctionUpdater : MonoBehaviour
{
    private static FunctionUpdater _instance;

    private Action _UpdateAction;

    public static void Create(Action action)
    {
        _instance._UpdateAction += action;
    }

    private void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        _UpdateAction?.Invoke();
    }
}
