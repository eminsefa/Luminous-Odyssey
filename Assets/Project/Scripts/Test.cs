using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool late;
    private void OnEnable()
    {
        transform.DOLocalMoveX(20, 2).SetLoops(-1, LoopType.Yoyo).SetUpdate(late? UpdateType.Late :UpdateType.Fixed);
    }
}