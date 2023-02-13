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
        public static event Action OnGameReset;
        public static event Action OnLevelStarted;

        public void LevelFailed()
        {
            OnGameReset?.Invoke();
        }
    }
}
