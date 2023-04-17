using System;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using Enums;
using UnityEngine;
namespace Managers
{
    [ExecutionOrder(eExecutionOrder.GameManager)]
    public class GameManager : Singleton<GameManager>
    {
        protected override void OnAwakeEvent()
        {
            base.OnAwakeEvent();
            Application.targetFrameRate = 60;
        }
    }
}
