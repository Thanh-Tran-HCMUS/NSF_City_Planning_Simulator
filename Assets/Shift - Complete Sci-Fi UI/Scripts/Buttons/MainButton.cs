using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    public class MainButton : MonoBehaviour
    {
        [Header("SETTINGS")]
        public string buttonText = "My Title";
        public bool useCustomText = false;

        TextMeshProUGUI normalText;
        TextMeshProUGUI highlightedText;
        TextMeshProUGUI pressedText;

        void Start()
        {
            if (useCustomText == false)
            {
                normalText = gameObject.transform.Find("Normal/Text").GetComponent<TextMeshProUGUI>();
                highlightedText = gameObject.transform.Find("Highlighted/Text").GetComponent<TextMeshProUGUI>();
                pressedText = gameObject.transform.Find("Pressed/Text").GetComponent<TextMeshProUGUI>();

                normalText.text = buttonText;
                highlightedText.text = buttonText;
                pressedText.text = buttonText;
            }
        }
    }
}