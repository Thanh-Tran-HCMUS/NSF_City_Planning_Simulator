using System;
using System.Collections;
using UnityEngine;

namespace HighlightPlus {

    public enum TriggerMode {
        ColliderEventsOnlyOnThisObject = 0,
        RaycastOnThisObjectAndChildren = 1,
        Volume = 2
    }

    public enum RayCastSource {
        MousePosition = 0,
        CameraDirection = 1
    }


    [RequireComponent(typeof(HighlightEffect))]
    [ExecuteInEditMode]
    public class HighlightTrigger : MonoBehaviour {

        [Tooltip("Enables highlight when pointer is over this object.")]
        public bool highlightOnHover = true;
        [Tooltip("Used to trigger automatic highlighting including children objects.")]
        public TriggerMode triggerMode = TriggerMode.ColliderEventsOnlyOnThisObject;
        public Camera raycastCamera;
        public RayCastSource raycastSource = RayCastSource.MousePosition;
        public float maxDistance;
        public LayerMask volumeLayerMask;

        const int MAX_RAYCAST_HITS = 100;


        [Tooltip("If the object will be selected by clicking with mouse or tapping on it.")]
        public bool selectOnClick;
        [Tooltip("Profile to use when object is selected by clicking on it.")]
        public HighlightProfile selectedProfile;
        [Tooltip("Profile to use whtn object is selected and highlighted.")]
        public HighlightProfile selectedAndHighlightedProfile;
        [Tooltip("Automatically deselects any other selected object prior selecting this one")]
        public bool singleSelection;
        [Tooltip("Toggles selection on/off when clicking object")]
        public bool toggle;

        [NonSerialized] public Collider[] colliders;

        Collider currentCollider;
        static RaycastHit[] hits;
        HighlightEffect hb;

        public HighlightEffect highlightEffect { get { return hb; } }

        public event OnObjectSelectionEvent OnObjectSelected;
        public event OnObjectSelectionEvent OnObjectUnSelected;

        void OnEnable() {
            Init();
        }

        public void Init() {
            if (raycastCamera == null) {
                raycastCamera = HighlightManager.GetCamera();
            }
            if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                colliders = GetComponentsInChildren<Collider>();
            }
            if (hb == null) {
                hb = GetComponent<HighlightEffect>();
            }
        }

        void Start() {
            if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                if (raycastCamera == null) {
                    raycastCamera = HighlightManager.GetCamera();
                    if (raycastCamera == null) {
                        Debug.LogError("Highlight Trigger on " + gameObject.name + ": no camera found!");
                    }
                }
                if (colliders != null && colliders.Length > 0) {
                    hits = new RaycastHit[MAX_RAYCAST_HITS];
                    StartCoroutine(DoRayCast());
                }
            } else {
                Collider collider = GetComponent<Collider>();
                if (collider == null) {
                    if (GetComponent<MeshFilter>() != null) {
                        gameObject.AddComponent<MeshCollider>();
                    }
                }
            }
        }


        IEnumerator DoRayCast() {
            while (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                if (raycastCamera != null) {
                    Ray ray;
                    if (raycastSource == RayCastSource.MousePosition) {
                        ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
                    } else {
                        ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
                    }
                    int hitCount;
                    if (maxDistance > 0) {
                        hitCount = Physics.RaycastNonAlloc(ray, hits, maxDistance);
                    } else {
                        hitCount = Physics.RaycastNonAlloc(ray, hits);
                    }
                    bool hit = false;
                    for (int k = 0; k < hitCount; k++) {
                        Collider theCollider = hits[k].collider;
                        for (int c = 0; c < colliders.Length; c++) {
                            if (colliders[c] == theCollider) {
                                hit = true;
                                if (selectOnClick && Input.GetMouseButtonDown(0)) {
                                    ToggleSelection();
                                    break;
                                } else if (theCollider != currentCollider) {
                                    SwitchCollider(theCollider);
                                    k = hitCount;
                                    break;
                                }
                            }
                        }
                    }
                    if (!hit && currentCollider != null) {
                        SwitchCollider(null);
                    }
                }
                yield return null;
            }
        }


        void SwitchCollider(Collider newCollider) {
            if (!highlightOnHover && !hb.isSelected) return;

            currentCollider = newCollider;
            if (currentCollider != null) {
                Highlight(true);
            } else {
                Highlight(false);
            }
        }


        void OnMouseDown() {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                if (selectOnClick && Input.GetMouseButtonDown(0)) {
                    ToggleSelection();
                    return;
                }
                Highlight(true);
            }
        }

        void OnMouseEnter() {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                Highlight(true);
            }
        }

        void OnMouseExit() {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                Highlight(false);
            }
        }

        void Highlight(bool state) {
            if (selectOnClick) {
                if (hb.isSelected) {
                    if (state) {
                        if (selectedAndHighlightedProfile != null) {
                            selectedAndHighlightedProfile.Load(hb);
                        }
                    } else {
                        if (selectedProfile != null) {
                            selectedProfile.Load(hb);
                        } else {
                            hb.previousSettings.Load(hb);
                        }
                    }
                    if (hb.highlighted) {
                        hb.UpdateMaterialProperties();
                    } else {
                        hb.SetHighlighted(true);
                    }
                    return;
                } else if (!highlightOnHover) {
                    hb.SetHighlighted(false);
                    return;
                }
            }
            hb.SetHighlighted(state);
        }


        void ToggleSelection() {

            HighlightManager.lastTriggerTime = Time.frameCount;

            bool newState = toggle ? !hb.isSelected : true;
            if (newState) {
                if (OnObjectSelected != null && !OnObjectSelected(gameObject)) return;
            } else {
                if (OnObjectUnSelected != null && !OnObjectUnSelected(gameObject)) return;
            }

            if (singleSelection && newState) {
                HighlightManager.DeselectAll();
            }
            hb.isSelected = newState;
            if (newState && !HighlightManager.selectedObjects.Contains(hb)) {
                HighlightManager.selectedObjects.Add(hb);
            } else if (!newState && HighlightManager.selectedObjects.Contains(hb)) {
                HighlightManager.selectedObjects.Remove(hb);
            }

            if (hb.isSelected) {
                if (hb.previousSettings == null) {
                    hb.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
                }
                hb.previousSettings.Save(hb);
            } else {
                if (hb.previousSettings != null) {
                    hb.previousSettings.Load(hb);
                }
            }

            Highlight(true);
        }

        public void OnTriggerEnter(Collider other) {
            if (triggerMode == TriggerMode.Volume) {
                if ((volumeLayerMask & (1 << other.gameObject.layer)) != 0) {
                    Highlight(true);
                }
            }
        }

        public void OnTriggerExit(Collider other) {
            if (triggerMode == TriggerMode.Volume) {
                if ((volumeLayerMask & (1 << other.gameObject.layer)) != 0) {
                    Highlight(false);
                }
            }
        }


    }

}