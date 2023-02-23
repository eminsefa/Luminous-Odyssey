using System;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class JoystickUI : MonoBehaviour
{
//     [SerializeField, ReadOnly] private RectTransform joystickArea;
//     [SerializeField, ReadOnly] private RectTransform joystickAreaScaleFix;
//     [SerializeField, ReadOnly] private Image         joystickOutline;
//     [SerializeField, ReadOnly] private Image         joystickHandle;
//
//     [SerializeField, ReadOnly] private Canvas       canvas;
//     [SerializeField, ReadOnly] private CanvasScaler canvasScaler;
//
//     private float currentMatchWidthOrHeight;
//
//     private InputVariables inputVars => GameConfig.Instance.Input;
//
//     [Button]
//     public void SetRefs()
//     {
//         joystickOutline      = transform.FindDeepChild<Image>("Joystick Outline");
//         joystickHandle       = transform.FindDeepChild<Image>("Joystick Handle");
//         canvas               = transform.GetComponentInParent<Canvas>();
//         canvasScaler         = transform.GetComponentInParent<CanvasScaler>();
//         joystickArea         = transform.FindDeepChild<RectTransform>("Joystick Area");
//         joystickAreaScaleFix = transform.FindDeepChild<RectTransform>("Scale Fix");
//     }
//
// #region Enable/Disable
//
//     private void OnEnable()
//     {
//         InputManager.OnInputJoystickDown     += onJoystickDown;
//         InputManager.OnInputJoystickUp       += onJoystickUp;
//         StorageManager.OnInputReverseChanged += OnInputReverseChanged;
//         Init();
//     }
//
//     private void OnDisable()
//     {
//         InputManager.OnInputJoystickDown     -= onJoystickDown;
//         InputManager.OnInputJoystickUp       -= onJoystickUp;
//         StorageManager.OnInputReverseChanged -= OnInputReverseChanged;
//     }
//
//     private void OnInputReverseChanged(bool reversed)
//     {
//         setUIPos();
//     }
//
// #endregion
//
//
// #region Events
//
//     private void onJoystickDown(Vector2 i_InputPos)
//     {
//         if (inputVars.Joystick.IsShowVisuals)
//         {
//             joystickOutline.gameObject.SetActive(true);
//         }
//     }
//
//     private void onJoystickUp(Vector2 i_InputPos)
//     {
//         if (inputVars.Joystick.IsShowVisuals)
//         {
//             joystickOutline.gameObject.SetActive(false);
//         }
//     }
//
// #endregion
//
//     private void Init()
//     {
//         gameObject.SetActive(inputVars.Joystick.IsShowVisuals);
//         currentMatchWidthOrHeight = canvasScaler.matchWidthOrHeight;
//         setUIPos();
//         setSizes();
//     }
//
//     private void Update()
//     {
//         if (Math.Abs(currentMatchWidthOrHeight - canvasScaler.matchWidthOrHeight) > 0.1f)
//         {
//             currentMatchWidthOrHeight = canvasScaler.matchWidthOrHeight;
//             setSizes();
//         }
//
//         if (inputVars.Joystick.IsShowVisuals)
//         {
//             var screenSizeInch = InputManager.Instance.ScreenData.ScreenSizeInch;
//             var screenSize     = Mathf.Lerp(screenSizeInch.x, screenSizeInch.y, canvasScaler.matchWidthOrHeight);
//             joystickOutline.rectTransform.localScale       = Vector3.one / screenSize                          * (inputVars.Joystick.Radius * 2);
//             joystickOutline.rectTransform.anchoredPosition = InputManager.Instance.JoystickPosition            / canvas.scaleFactor;
//             joystickHandle.rectTransform.anchoredPosition  = InputManager.Instance.JoystickHandleLocalPosition * (joystickHandle.rectTransform.sizeDelta.y * 0.5f);
//         }
//     }
//
//     private void setUIPos()
//     {
//         joystickArea.pivot            = joystickAreaScaleFix.pivot            = new Vector2(StorageManager.Instance.IsInputReverse ? 0 : 1, 0.5f);
//         joystickArea.anchoredPosition = joystickAreaScaleFix.anchoredPosition = Vector2.zero;
//     }
//
//     private void setSizes()
//     {
//         joystickOutline.rectTransform.sizeDelta = Vector2.one                             * Mathf.Lerp(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, canvasScaler.matchWidthOrHeight);
//         joystickHandle.rectTransform.sizeDelta  = joystickOutline.rectTransform.sizeDelta * inputVars.Joystick.HandleRadiusMultiplier;
//     }
}