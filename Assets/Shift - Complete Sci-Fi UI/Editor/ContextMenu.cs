using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Shift
{
    public class ContextMenu : MonoBehaviour
    {
        [MenuItem("Tools/Shift UI/Show UI Manager")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("Shift UI Manager");

            if (Selection.activeObject == null)
                Debug.Log("Can't find a file named 'Shift UI Manager'. Make sure you have 'Shift UI Manager' file in Resources folder.");
        }
    }
}