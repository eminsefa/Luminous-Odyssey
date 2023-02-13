using System;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UI
{
    public class InputButton : Button, IDragHandler
    {
        public event Action<InputButton> OnButtonDown;
        public event Action<InputButton> OnButtonUp;

        public Vector2 InputPosition { get; private set; }

        [field: SerializeField] public eButtonType ButtonType;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            InputPosition = eventData.position;
            OnButtonDown?.Invoke(this);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnButtonUp?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            InputPosition = eventData.position;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(InputButton))]
    public class MenuButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InputButton targetMenuButton = (InputButton) target;

            targetMenuButton.ButtonType = (eButtonType) EditorGUILayout.EnumPopup("ButtonType", (eButtonType) targetMenuButton.ButtonType);
        }
    }
#endif
}