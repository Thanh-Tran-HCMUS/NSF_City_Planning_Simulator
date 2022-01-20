using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public class UIElementSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("RESOURCES")]
        public UIManager UIManagerAsset;
        public AudioSource audioObject;

        [Header("SETTINGS")]
        public bool enableHoverSound = true;
        public bool enableClickSound = true;

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
            if (audioObject == null)
                audioObject = gameObject.GetComponent<AudioSource>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableHoverSound == true)
                audioObject.PlayOneShot(UIManagerAsset.hoverSound);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableClickSound == true)
                audioObject.PlayOneShot(UIManagerAsset.clickSound);
        }
    }
}