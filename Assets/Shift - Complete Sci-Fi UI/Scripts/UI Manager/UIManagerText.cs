using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerText : MonoBehaviour
    {
        [Header("RESOURCES")]
        public UIManager UIManagerAsset;
        public TextMeshProUGUI textObject;

        [Header("SETTINGS")]
        public bool keepAlphaValue = false;
        public bool useCustomColor = false;
        public ColorType colorType;
        public FontType fontType;

        bool dynamicUpdateEnabled;

        public enum ColorType
        {
            PRIMARY,
            SECONDARY,
            PRIMARY_REVERSED,
            NEGATIVE,
            BACKGROUND
        }

        public enum FontType
        {
            LIGHT,
            REGULAR,
            MEDIUM,
            SEMIBOLD,
            BOLD
        }

        void OnEnable()
        {
            if (UIManagerAsset == null)
            {
                try
                {
                    UIManagerAsset = Resources.Load<UIManager>("Shift UI Manager");
                }

                catch
                {
                    Debug.Log("No UI Manager found. Assign it manually, otherwise it won't work properly.");
                }
            }
        }

        void Awake()
        {
            if (dynamicUpdateEnabled == false)
            {
                this.enabled = true;
                UpdateButton();
            }

            if (textObject == null)
                textObject = gameObject.GetComponent<TextMeshProUGUI>();
        }

        void LateUpdate()
        {
            if (UIManagerAsset != null)
            {
                if (UIManagerAsset.enableDynamicUpdate == true)
                    dynamicUpdateEnabled = true;
                else
                    dynamicUpdateEnabled = false;

                if (dynamicUpdateEnabled == true)
                    UpdateButton();
            }
        }

        void UpdateButton()
        {
            try
            {
                // Colors
                if (useCustomColor == false)
                {
                    if (keepAlphaValue == false)
                    {
                        if (colorType == ColorType.PRIMARY)
                            textObject.color = UIManagerAsset.primaryColor;

                        else if (colorType == ColorType.SECONDARY)
                            textObject.color = UIManagerAsset.secondaryColor;

                        else if (colorType == ColorType.PRIMARY_REVERSED)
                            textObject.color = UIManagerAsset.primaryReversed;

                        else if (colorType == ColorType.NEGATIVE)
                            textObject.color = UIManagerAsset.negativeColor;

                        else if (colorType == ColorType.BACKGROUND)
                            textObject.color = UIManagerAsset.backgroundColor;
                    }

                    else
                    {
                        if (colorType == ColorType.PRIMARY)
                            textObject.color = new Color(UIManagerAsset.primaryColor.r, UIManagerAsset.primaryColor.g, UIManagerAsset.primaryColor.b, textObject.color.a);

                        else if (colorType == ColorType.SECONDARY)
                            textObject.color = new Color(UIManagerAsset.secondaryColor.r, UIManagerAsset.secondaryColor.g, UIManagerAsset.secondaryColor.b, textObject.color.a);

                        else if (colorType == ColorType.PRIMARY_REVERSED)
                            textObject.color = new Color(UIManagerAsset.primaryReversed.r, UIManagerAsset.primaryReversed.g, UIManagerAsset.primaryReversed.b, textObject.color.a);

                        else if (colorType == ColorType.NEGATIVE)
                            textObject.color = new Color(UIManagerAsset.negativeColor.r, UIManagerAsset.negativeColor.g, UIManagerAsset.negativeColor.b, textObject.color.a);

                        else if (colorType == ColorType.BACKGROUND)
                            textObject.color = new Color(UIManagerAsset.backgroundColor.r, UIManagerAsset.backgroundColor.g, UIManagerAsset.backgroundColor.b, textObject.color.a);
                    }
                }

                // Fonts
                if (fontType == FontType.LIGHT)
                    textObject.font = UIManagerAsset.lightFont;

                else if (fontType == FontType.REGULAR)
                    textObject.font = UIManagerAsset.regularFont;

                else if (fontType == FontType.MEDIUM)
                    textObject.font = UIManagerAsset.mediumFont;

                else if (fontType == FontType.SEMIBOLD)
                    textObject.font = UIManagerAsset.semiBoldFont;

                else if (fontType == FontType.BOLD)
                    textObject.font = UIManagerAsset.boldFont;
            }

            catch { }
        }
    }
}