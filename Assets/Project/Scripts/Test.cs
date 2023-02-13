using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    
}

public class TowerLocation : MonoBehaviour
{
    public static           TowerLocation   Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] public List<Transform> Towers;
}

public class Mover : MonoBehaviour
{
    private void Start()
    {
        var towers = TowerLocation.Instance.Towers;
        var right  = towers[0];
        var random = towers[Random.Range(0, towers.Count)];
    }
}
