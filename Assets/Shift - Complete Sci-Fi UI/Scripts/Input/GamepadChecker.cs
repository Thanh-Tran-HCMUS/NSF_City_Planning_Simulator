using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Shift
{
    public class GamepadChecker : MonoBehaviour
    {
        [Header("RESOURCES")]
        public VirtualCursor virtualCursor;
        public GameObject eventSystem;

        [Header("OBJECTS")]
        [Tooltip("Objects in this list will be active when gamepad is un-plugged.")]
        public List<GameObject> keyboardObjects = new List<GameObject>();
        [Tooltip("Objects in this list will be active when gamepad is plugged.")]
        public List<GameObject> gamepadObjects = new List<GameObject>();

        [Header("SETTINGS")]
        [Tooltip("Always update input device. If you turn off this feature, you won't able to change the input device after start, but it might increase the performance.")]
        public bool alwaysSearch = true;

        private GamepadChecker checkerScript;
        private int gamepadConnected = 0;
        private Vector3 startMousePos;
        private Vector3 startPos;
        bool gamepadEnabled;

        void Start()
        {
            checkerScript = gameObject.GetComponent<GamepadChecker>();

            if (virtualCursor == null)
            {
                try
                {
                    Debug.Log("Looking for Virtual Cursor automatically.");
                    var vCursor = (VirtualCursor)GameObject.FindObjectsOfType(typeof(VirtualCursor))[0];
                    virtualCursor = vCursor;
                }

                catch
                {
                    Debug.LogError("There isn't any Virtual Cursor component in the scene. Add one, otherwise it won't work");
                }
            }

            // We're doing this since unity layout group could be buggy while using content size fitter
            for (int i = 0; i < gamepadObjects.Count; i++)
                gamepadObjects[i].SetActive(true);

            for (int i = 0; i < gamepadObjects.Count; i++)
                gamepadObjects[i].SetActive(false);

            for (int i = 0; i < keyboardObjects.Count; i++)
                keyboardObjects[i].SetActive(true);

            for (int i = 0; i < keyboardObjects.Count; i++)
                keyboardObjects[i].SetActive(false);

            string[] names = Input.GetJoystickNames();

            for (int x = 0; x < names.Length; x++)
            {
                if (names[x].Length >= 1)
                    gamepadConnected = 1;

                else if (names[x].Length == 0)
                    gamepadConnected = 0;
            }

            if (gamepadConnected == 1)
                SwitchToController();

            else if (gamepadConnected == 0)
                SwitchToKeyboard();

            if (alwaysSearch == false)
                checkerScript.enabled = false;

            else
            {
                checkerScript.enabled = true;
                Debug.Log("Always Search is on. Input device will be updated in case of disconnecting/connecting.");
            }
        }

        void Update()
        {
            string[] names = Input.GetJoystickNames();

            for (int x = 0; x < names.Length; x++)
            {
                // print(names[x].Length); Just for testing stuff

                if (names[x].Length >= 1)
                    gamepadConnected = 1;

                else if (names[x].Length == 0)
                    gamepadConnected = 0;
            }

            if (gamepadConnected == 1 && gamepadEnabled == false)
                SwitchToController();

            else if (gamepadConnected == 0 && gamepadEnabled == true)
                SwitchToKeyboard();
        }

        public void SwitchToController()
        {
            for (int i = 0; i < keyboardObjects.Count; i++)
                keyboardObjects[i].SetActive(false);

            for (int i = 0; i < gamepadObjects.Count; i++)
                gamepadObjects[i].SetActive(true);

            gamepadEnabled = true;
            virtualCursor.gameObject.SetActive(true);
            eventSystem.SetActive(false);
            Cursor.visible = false;
            Debug.Log("Switching to gamepad input.");
        }

        public void SwitchToKeyboard()
        {
            for (int i = 0; i < keyboardObjects.Count; i++)
                keyboardObjects[i].SetActive(true);

            for (int i = 0; i < gamepadObjects.Count; i++)
                gamepadObjects[i].SetActive(false);

            gamepadEnabled = false;
            virtualCursor.gameObject.SetActive(false);
            eventSystem.SetActive(true);
            Cursor.visible = true;
            Debug.Log("Switching to mouse / keyboard input.");
        }
    }
}