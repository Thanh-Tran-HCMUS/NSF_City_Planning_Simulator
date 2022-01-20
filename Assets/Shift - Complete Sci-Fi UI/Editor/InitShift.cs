using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Shift
{
    public class InitShift : MonoBehaviour
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("ShiftUI.Installed"))
                {
                    EditorPrefs.SetInt("ShiftUI.Installed", 1);
                    EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Shift UI.\r\rFirst of all, import TextMesh Pro from Package Manager if you haven't already." +
                        "\r\rYou can gain more performance by unchecking 'Update Values' on Shift UI Manager. Do not forget to enable it while changing stuff on Shift UI Manager." +
                        "\r\rYou can contact me at isa.steam@outlook.com or Discord server for support.", "Got it!");
                }
            }
        }
    }
}