using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
   private void OnEnable()
   {
       transform.DOLocalMoveY(3, 3)
                .SetLoops(-1, LoopType.Yoyo);
   }
}
