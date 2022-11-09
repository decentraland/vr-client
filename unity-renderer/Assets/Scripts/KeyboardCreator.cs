using System;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace DCL.Interface
{
    public class KeyboardCreator : MonoBehaviour
    {
        private static GameObject canvasKeyboard;
        private static NonNativeKeyboard keyboard;
        private static Transform _keyboardTrans;
        private static Transform keyboardTrans
        {
            get
            {
                if (_keyboardTrans != null)
                    return _keyboardTrans;
                Create();
                return _keyboardTrans;
            }
        }

        private static void Create()
        {
            canvasKeyboard = Instantiate((GameObject)Resources.Load("Prefabs/KeyboardCanvas"));
            keyboard = NonNativeKeyboard.Instance;
            _keyboardTrans = canvasKeyboard.transform;
        }

        private TMP_InputField tmpInputField;

        private void Awake()
        {
            tmpInputField = GetComponent<TMP_InputField>();
            tmpInputField.onSelect.AddListener(OpenKeyboard);
        }

        private void Close(string arg0) { CleanUpEvents(); }

        private void OpenKeyboard(string arg0)
        {
            if (keyboardTrans == null)
                return;
            CleanUpEvents();
            SetupEvents();
            keyboard.PresentKeyboard(NonNativeKeyboard.LayoutType.URL);
            var rawForward = CommonScriptableObjects.cameraForward.Get();
            keyboardTrans.position = CommonScriptableObjects.cameraPosition.Get() + (.7f * rawForward) + new UnityEngine.Vector3(0, 0.3f, 0);
            keyboardTrans.forward = new UnityEngine.Vector3(rawForward.x, 0, rawForward.z);
            canvasKeyboard.SetActive(true);
        }

        private void HandleSubmit(object sender, EventArgs e)
        {
            tmpInputField.text = keyboard.InputField.text;
            tmpInputField.onSubmit.Invoke(tmpInputField.text);
        }

        private void CleanUpEvents() { keyboard.OnTextSubmitted -= HandleSubmit; }

        private void SetupEvents() { keyboard.OnTextSubmitted += HandleSubmit; }
    }

}