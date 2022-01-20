using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    public class ShortcutButton : MonoBehaviour
    {
        [Header("RESOURCES")]
        public string keyText = "A";
        public string buttonText = "My Title";

        [Header("SETTINGS")]
        public bool useCustomText = false;
        public bool isGamepad = false;

        TextMeshProUGUI normalText;
        TextMeshProUGUI highlightedText;
        TextMeshProUGUI normalKeyText;
        TextMeshProUGUI highlightedKeyText;

        void Start()
        {
            if (useCustomText == false)
            {
                if (isGamepad == false)
                {
                    normalText = gameObject.transform.Find("Normal/Text").GetComponent<TextMeshProUGUI>();
                    highlightedText = gameObject.transform.Find("Highlighted/Text").GetComponent<TextMeshProUGUI>();
                    normalKeyText = gameObject.transform.Find("Normal/Border/Text").GetComponent<TextMeshProUGUI>();
                    highlightedKeyText = gameObject.transform.Find("Highlighted/Border/Text").GetComponent<TextMeshProUGUI>();

                    normalText.text = buttonText;
                    highlightedText.text = buttonText;
                    normalKeyText.text = keyText;
                    highlightedKeyText.text = keyText;
                }

                else
                {
                    normalText = gameObject.transform.Find("Normal/Text").GetComponent<TextMeshProUGUI>();
                    normalKeyText = gameObject.transform.Find("Normal/Border/Text").GetComponent<TextMeshProUGUI>();

                    normalText.text = buttonText;
                    normalKeyText.text = keyText;
                }
            }
        }
    }
}