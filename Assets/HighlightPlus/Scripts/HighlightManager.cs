using System;
using System.Collections.Generic;
using UnityEngine;

namespace HighlightPlus {

    public delegate bool OnObjectSelectionEvent(GameObject obj);


    [RequireComponent(typeof(HighlightEffect))]
    [DefaultExecutionOrder(100)]
    public class HighlightManager : MonoBehaviour {

        [Tooltip("Enables highlight when pointer is over this object.")]
        public bool highlightOnHover = true;

        public LayerMask layerMask = -1;
        public Camera raycastCamera;
        public RayCastSource raycastSource = RayCastSource.MousePosition;
        [Tooltip("Max Distance for target. 0 = infinity")]
        public float maxDistance;

        [Tooltip("If the object will be selected by clicking with mouse or tapping on it.")]
        public bool selectOnClick;
        [Tooltip("Optional profile for objects selected by clicking on them")]
        public HighlightProfile selectedProfile;
        [Tooltip("Profile to use whtn object is selected and highlighted.")]
        public HighlightProfile selectedAndHighlightedProfile;
        [Tooltip("Automatically deselects other previously selected objects")]
        public bool singleSelection;
        [Tooltip("Toggles selection on/off when clicking object")]
        public bool toggle;

        HighlightEffect baseEffect, currentEffect;
        Transform currentObject;

        public readonly static List<HighlightEffect> selectedObjects = new List<HighlightEffect>();
        public event OnObjectSelectionEvent OnObjectSelected;
        public event OnObjectSelectionEvent OnObjectUnSelected;
        public static int lastTriggerTime;

        static HighlightManager _instance;
        public static HighlightManager instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<HighlightManager>();
                }
                return _instance;
            }
        }

        void OnEnable() {
            currentObject = null;
            currentEffect = null;
            if (baseEffect == null) {
                baseEffect = GetComponent<HighlightEffect>();
                if (baseEffect == null) {
                    baseEffect = gameObject.AddComponent<HighlightEffect>();
                }
            }
            raycastCamera = GetComponent<Camera>();
            if (raycastCamera == null) {
                raycastCamera = GetCamera();
                if (raycastCamera == null) {
                    Debug.LogError("Highlight Manager: no camera found!");
                }
            }
        }


        void OnDisable() {
            SwitchesCollider(null);
            internal_DeselectAll();
        }

        void Update() {
            if (raycastCamera == null)
                return;
            Ray ray;
            if (raycastSource == RayCastSource.MousePosition) {
                ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
            } else {
                ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
            }
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance > 0 ? maxDistance : raycastCamera.farClipPlane, layerMask)) {
                Transform t = hitInfo.collider.transform;
                // Toggles selection
                if (Input.GetMouseButtonDown(0)) {
                    if (selectOnClick) {
                        ToggleSelection(t);
                    } else if (lastTriggerTime < Time.frameCount) {
                        internal_DeselectAll();
                    }
                } else {
                    // Check if the object has a Highlight Effect
                    if (t != currentObject) {
                        SwitchesCollider(t);
                    }
                }
                return;
            }

            // no hit
            if (Input.GetMouseButtonDown(0) && lastTriggerTime < Time.frameCount) {
                internal_DeselectAll();
            }
            SwitchesCollider(null);
        }

        void SwitchesCollider(Transform newObject) {
            if (currentEffect != null) {
                if (highlightOnHover) {
                    Highlight(false);
                }
                currentEffect = null;
            }
            currentObject = newObject;
            if (newObject == null) return;
            HighlightTrigger ht = newObject.GetComponent<HighlightTrigger>();
            if (ht != null && ht.enabled)
                return;

            HighlightEffect otherEffect = newObject.GetComponent<HighlightEffect>();
            if (otherEffect == null) {
                // Check if there's a parent highlight effect that includes this object
                HighlightEffect parentEffect = newObject.GetComponentInParent<HighlightEffect>();
                if (parentEffect != null && parentEffect.Includes(newObject)) {
                    currentEffect = parentEffect;
                    if (highlightOnHover) {
                        Highlight(true);
                    }
                    return;
                }
            }
            currentEffect = otherEffect != null ? otherEffect : baseEffect;
            baseEffect.enabled = currentEffect == baseEffect;
            currentEffect.SetTarget(currentObject);

            if (highlightOnHover) {
                Highlight(true);
            }
        }


        void ToggleSelection(Transform t) {

            // We need a highlight effect on each selected object
            HighlightEffect hb = t.GetComponent<HighlightEffect>();
            if (hb == null) {
                HighlightEffect parentEffect = t.GetComponentInParent<HighlightEffect>();
                if (parentEffect != null && parentEffect.Includes(t)) {
                    hb = parentEffect;
                    if (hb.previousSettings == null) {
                        hb.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
                    }
                    hb.previousSettings.Save(hb);
                } else {
                    hb = t.gameObject.AddComponent<HighlightEffect>();
                    hb.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
                    // copy default highlight effect settings from this manager into this highlight plus component
                    hb.previousSettings.Save(baseEffect);
                    hb.previousSettings.Load(hb);
                }
            }

            bool newState = toggle ? !currentEffect.isSelected : true;
            if (newState) {
                if (OnObjectSelected != null && !OnObjectSelected(t.gameObject)) return;
            } else {
                if (OnObjectUnSelected != null && !OnObjectUnSelected(t.gameObject)) return;
            }

            if (singleSelection) {
                internal_DeselectAll();
            }

            currentEffect = hb;
            currentEffect.isSelected = newState;
            baseEffect.enabled = false;

            if (currentEffect.isSelected) {
                if (currentEffect.previousSettings == null) {
                    currentEffect.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
                }
                hb.previousSettings.Save(hb);

                if (!selectedObjects.Contains(currentEffect)) {
                    selectedObjects.Add(currentEffect);
                }
            } else {
                if (currentEffect.previousSettings != null) {
                    currentEffect.previousSettings.Load(hb);
                }
                if (selectedObjects.Contains(currentEffect)) {
                    selectedObjects.Remove(currentEffect);
                }
            }

            Highlight(true);
        }

        void Highlight(bool state) {
            if (selectOnClick) {
                if (currentEffect.isSelected) {
                    if (state) {
                        if (selectedAndHighlightedProfile != null) {
                            selectedAndHighlightedProfile.Load(currentEffect);
                        }
                    } else {
                        if (selectedProfile != null) {
                            selectedProfile.Load(currentEffect);
                        } else {
                            currentEffect.previousSettings.Load(currentEffect);
                        }
                    }
                    if (currentEffect.highlighted) {
                        currentEffect.UpdateMaterialProperties();
                    } else {
                        currentEffect.SetHighlighted(true);
                    }
                    return;
                } else if (!highlightOnHover) {
                    currentEffect.SetHighlighted(false);
                    return;
                }
            }
            currentEffect.SetHighlighted(state);
        }

        public static Camera GetCamera() {
            Camera raycastCamera = Camera.main;
            if (raycastCamera == null) {
                raycastCamera = FindObjectOfType<Camera>();
            }
            return raycastCamera;
        }

        void internal_DeselectAll() {
            foreach (HighlightEffect hb in selectedObjects) {
                if (hb != null && hb.gameObject != null) {
                    if (OnObjectUnSelected != null) {
                        if (!OnObjectUnSelected(hb.gameObject)) continue;
                    }
                    hb.isSelected = false;
                    hb.SetHighlighted(false);
                }
            }
        }


        public static void DeselectAll() {
            foreach (HighlightEffect hb in selectedObjects) {
                if (hb != null && hb.gameObject != null) {
                    hb.isSelected = false;
                    hb.SetHighlighted(false);
                }
            }
        }


    }

}