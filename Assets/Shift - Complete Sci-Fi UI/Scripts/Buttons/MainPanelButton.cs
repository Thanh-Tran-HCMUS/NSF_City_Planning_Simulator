using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class MainPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("TEXT")]
        public bool useCustomText = false;
        public string buttonText = "My Title";

        [Header("ICON")]
        public bool hasIcon = false;
        public Sprite iconSprite;

        Animator buttonAnimator;
        TextMeshProUGUI normalText;
        TextMeshProUGUI highlightedText;
        TextMeshProUGUI pressedText;
        Image normalIcon;
        Image highlightedIcon;
        Image pressedIcon;

        void Start()
        {
            buttonAnimator = gameObject.GetComponent<Animator>();

            if (useCustomText == false)
            {
                normalText = gameObject.transform.Find("Normal/Text").GetComponent<TextMeshProUGUI>();
                highlightedText = gameObject.transform.Find("Highlighted/Text").GetComponent<TextMeshProUGUI>();
                pressedText = gameObject.transform.Find("Pressed/Text").GetComponent<TextMeshProUGUI>();

                normalText.text = buttonText;
                highlightedText.text = buttonText;
                pressedText.text = buttonText;
            }

            if (hasIcon == true)
            {
                normalIcon = gameObject.transform.Find("Normal/Icon").GetComponent<Image>();
                highlightedIcon = gameObject.transform.Find("Highlighted/Icon").GetComponent<Image>();
                pressedIcon = gameObject.transform.Find("Pressed/Icon").GetComponent<Image>();

                normalIcon.sprite = iconSprite;
                highlightedIcon.sprite = iconSprite;
                pressedIcon.sprite = iconSprite;
            }

            else if (hasIcon == false)
            {
                try
                {
                    normalIcon = gameObject.transform.Find("Normal/Icon").GetComponent<Image>();
                    highlightedIcon = gameObject.transform.Find("Highlighted/Icon").GetComponent<Image>();
                    pressedIcon = gameObject.transform.Find("Pressed/Icon").GetComponent<Image>();

                    Destroy(normalIcon.gameObject);
                    Destroy(highlightedIcon.gameObject);
                    Destroy(pressedIcon.gameObject);
                }

                catch { }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Dissolve to Normal");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Dissolve");
        }
    }
}