using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool      moveX;
    public float      speed;
    private void OnEnable()
    {
        if(moveX)
        {
            transform.DOLocalMoveX(20, speed)
              .SetSpeedBased(true)
                       .SetLoops(-1, LoopType.Yoyo)
                       .SetRelative(true)
                       .SetUpdate(UpdateType.Fixed);
            
        }
        else transform.DOLocalMoveY(20, speed)
               .SetSpeedBased(true)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetRelative(true)
                        .SetUpdate(UpdateType.Fixed);
    }
}