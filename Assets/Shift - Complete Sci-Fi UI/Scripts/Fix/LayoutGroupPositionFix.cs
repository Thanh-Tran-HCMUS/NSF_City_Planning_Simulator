using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Shift
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        void Start()
        {
            // Because Unity UI is buggy and needs refreshing a few times :P
            StartCoroutine(ExecuteAfterTime(5f));
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            StopCoroutine(ExecuteAfterTime(0.05f));
            Destroy(this);
        }
    }
}