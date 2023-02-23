using System;
using Attributes;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    [ExecutionOrder(eExecutionOrder.StorageManager)]
    public class StorageManager : Singleton<StorageManager>
    {
        // public static event Action<bool> OnInputReverseChanged;
        //
        // [ShowInInspector, PropertyOrder(0)]
        // public bool IsInputReverse
        // {
        //     get => PlayerPrefs.GetInt(nameof(IsInputReverse), 0) == 1;
        //     set
        //     {
        //         PlayerPrefs.SetInt(nameof(IsInputReverse), value == true ? 1 : 0);
        //         OnInputReverseChanged?.Invoke(value);
        //     }
        // }
    }
}