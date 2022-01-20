using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class ChapterButton : MonoBehaviour
    {
        [Header("RESOURCES")]
        public Sprite backgroundImage;
        public string buttonTitle = "My Title";
        [TextArea] public string buttonDescription = "My Description";

        [Header("SETTINGS")]
        public bool useCustomResources = false;

        [Header("STATUS")]
        public bool enableStatus;
        public StatusItem statusItem;

        Image backgroundImageObj;
        TextMeshProUGUI titleObj;
        TextMeshProUGUI descriptionObj;
        Transform statusNone;
        Transform statusLocked;
        Transform statusCompleted;

        public enum StatusItem
        {
            NONE,
            LOCKED,
            COMPLETED
        }

        void Start()
        {
            if (useCustomResources == false)
            {
                backgroundImageObj = gameObject.transform.Find("Content/Background").GetComponent<Image>();
                titleObj = gameObject.transform.Find("Content/Texts/Title").GetComponent<TextMeshProUGUI>();
                descriptionObj = gameObject.transform.Find("Content/Texts/Description").GetComponent<TextMeshProUGUI>();

                backgroundImageObj.sprite = backgroundImage;
                titleObj.text = buttonTitle;
                descriptionObj.text = buttonDescription;
            }

            if (enableStatus == true)
            {
                statusNone = gameObject.transform.Find("Content/Texts/Status/None").GetComponent<Transform>();
                statusLocked = gameObject.transform.Find("Content/Texts/Status/Locked").GetComponent<Transform>();
                statusCompleted = gameObject.transform.Find("Content/Texts/Status/Completed").GetComponent<Transform>();

                if (statusItem == StatusItem.NONE)
                {
                    statusNone.gameObject.SetActive(true);
                    statusLocked.gameObject.SetActive(false);
                    statusCompleted.gameObject.SetActive(false);
                }

                else if (statusItem == StatusItem.LOCKED)
                {
                    statusNone.gameObject.SetActive(false);
                    statusLocked.gameObject.SetActive(true);
                    statusCompleted.gameObject.SetActive(false);
                }

                else if (statusItem == StatusItem.COMPLETED)
                {
                    statusNone.gameObject.SetActive(false);
                    statusLocked.gameObject.SetActive(false);
                    statusCompleted.gameObject.SetActive(true);
                }
            }
        }
    }
}