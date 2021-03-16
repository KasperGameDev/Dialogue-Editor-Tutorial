using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    private event Action<int> randomColorModel;

    public static GameEvents Instance { get; private set; }
    public Action<int> RandomColorModel { get => randomColorModel; set => randomColorModel = value; }

    private void Awake()
    {
        Instance = this;
    }

    public void CallRandomColorModel(int number)
    {
        randomColorModel?.Invoke(number);
    }
}
