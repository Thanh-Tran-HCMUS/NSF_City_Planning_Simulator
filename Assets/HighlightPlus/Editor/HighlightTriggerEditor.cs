using UnityEditor;
using UnityEngine;

namespace HighlightPlus {
    [CustomEditor(typeof(HighlightTrigger))]
    public class HighlightTriggerEditor : Editor {

        SerializedProperty highlightOnHover, triggerMode, raycastCamera, raycastSource, maxDistance, volumeLayerMask;
        SerializedProperty selectOnClick, selectedProfile, selectedAndHighlightedProfile, singleSelection, toggleOnClick;
        HighlightTrigger trigger;

        void OnEnable() {
            highlightOnHover = serializedObject.FindProperty("highlightOnHover");
            triggerMode = serializedObject.FindProperty("triggerMode");
            raycastCamera = serializedObject.FindProperty("raycastCamera");
            raycastSource = serializedObject.FindProperty("raycastSource");
            maxDistance = serializedObject.FindProperty("maxDistance");
            volumeLayerMask = serializedObject.FindProperty("volumeLayerMask");
            selectOnClick = serializedObject.FindProperty("selectOnClick");
            selectedProfile = serializedObject.FindProperty("selectedProfile");
            selectedAndHighlightedProfile = serializedObject.FindProperty("selectedAndHighlightedProfile");
            singleSelection = serializedObject.FindProperty("singleSelection");
            toggleOnClick = serializedObject.FindProperty("toggle");
            trigger = (HighlightTrigger)target;
            trigger.Init();
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            if (trigger.triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                if (trigger.colliders == null || trigger.colliders.Length == 0) {
                    EditorGUILayout.HelpBox("No collider found on this object or any of its children. Add colliders to allow automatic highlighting.", MessageType.Warning);
                }
            } else {
                if (trigger.GetComponent<Collider>() == null) {
                    EditorGUILayout.HelpBox("No collider found on this object. Add a collider to allow automatic highlighting.", MessageType.Error);
                }
            }

            EditorGUILayout.PropertyField(triggerMode);
            switch (trigger.triggerMode) {
                case TriggerMode.RaycastOnThisObjectAndChildren:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(raycastCamera);
                    EditorGUILayout.PropertyField(raycastSource);
                    EditorGUILayout.PropertyField(maxDistance, new GUIContent("Max Distance", "Max distance for target. 0 = infinity")); ;
                    EditorGUI.indentLevel--;
                    break;
                case TriggerMode.Volume:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(volumeLayerMask);
                    EditorGUI.indentLevel--;
                    break;
            }

            EditorGUILayout.PropertyField(highlightOnHover);
            EditorGUILayout.PropertyField(selectOnClick);
            if (selectOnClick.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(selectedProfile);
                EditorGUILayout.PropertyField(selectedAndHighlightedProfile);
                EditorGUILayout.PropertyField(singleSelection);
                EditorGUILayout.PropertyField(toggleOnClick);
                EditorGUILayout.HelpBox("To deselect any object by clicking outside, add a Highlight Manager to the scene.", MessageType.Info);
                EditorGUI.indentLevel--;
            }

            if (serializedObject.ApplyModifiedProperties()) {
                trigger.Init();
            }
        }

    }

}
