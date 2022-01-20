using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("RESOURCES")]
        public Image detailImage;
        public Image detailIcon;
        public Image detailBackground;
        public TextMeshProUGUI detailTitle;
        public TextMeshProUGUI detailDescription;
        public TextMeshProUGUI buttonTitleObj;

        [Header("CONTENT")]
        public string buttonTitle;

        [Header("PREVIEW")]
        public string title;
        [TextArea] public string description;
        public bool enableIconPreview;
        public Sprite imageSprite;
        public Sprite iconSprite;
        public Sprite iconBackground;

        void Start()
        {
            buttonTitleObj.text = buttonTitle;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(enableIconPreview == true)
            {
                detailImage.gameObject.SetActive(false);
                detailIcon.gameObject.SetActive(true);
                detailBackground.gameObject.SetActive(true);
                detailIcon.sprite = iconSprite;
                detailBackground.sprite = iconBackground;
            }

            else
            {
                detailImage.gameObject.SetActive(true);
                detailIcon.gameObject.SetActive(false);
                detailBackground.gameObject.SetActive(false);
                detailImage.sprite = imageSprite;
            }

            detailTitle.text = title;
            detailDescription.text = description;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // You can put your OnPointerExit events here
        }
    }
}