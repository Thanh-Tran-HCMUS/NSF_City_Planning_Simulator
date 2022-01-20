using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
#endif

#if UNITY_EDITOR
namespace Michsky.UI.Shift
{
    [CustomEditor(typeof(UIManager))]
    [System.Serializable]
    public class UIManagerEditor : Editor
    {
        Texture2D muipLogo;
        protected static bool showBackground = false;
        protected static bool showColors = false;
        protected static bool showFonts = false;
        protected static bool showLogo = false;
        protected static bool showParticle = false;
        protected static bool showSounds = false;

        void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true)
                muipLogo = Resources.Load<Texture2D>("Editor\\Shift Editor Dark");

            else
                muipLogo = Resources.Load<Texture2D>("Editor\\Shift Editor Light");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.fontSize = 12;

            // Editor Logo
            GUILayout.Label(muipLogo, GUILayout.Width(250), GUILayout.Height(40));

            GUILayout.Space(6);

            // Background
            var backgroundType = serializedObject.FindProperty("backgroundType");
            var backgroundImage = serializedObject.FindProperty("backgroundImage");
            var backgroundPreserveAspect = serializedObject.FindProperty("backgroundPreserveAspect");
            var backgroundVideo = serializedObject.FindProperty("backgroundVideo");
            var backgroundSpeed = serializedObject.FindProperty("backgroundSpeed");
            var backgroundColorTint = serializedObject.FindProperty("backgroundColorTint");
            showBackground = EditorGUILayout.Foldout(showBackground, "Background", foldoutStyle);

            if (showBackground && backgroundType.enumValueIndex == 0)
            {
                GUILayout.Space(2);
                EditorGUILayout.PropertyField(backgroundType, new GUIContent("Background Type"));
                EditorGUILayout.PropertyField(backgroundImage, new GUIContent("Background Image"));
                EditorGUILayout.PropertyField(backgroundColorTint, new GUIContent("Color Tint"));
                EditorGUILayout.PropertyField(backgroundPreserveAspect, new GUIContent("Preserve Aspect"));
            }

            if (showBackground && backgroundType.enumValueIndex == 1)
            {
                GUILayout.Space(2);
                EditorGUILayout.PropertyField(backgroundType, new GUIContent("Background Type"));
                EditorGUILayout.PropertyField(backgroundVideo, new GUIContent("Background Video"));
                EditorGUILayout.PropertyField(backgroundColorTint, new GUIContent("Color Tint"));
                EditorGUILayout.PropertyField(backgroundSpeed, new GUIContent("Animation Speed"));
                EditorGUILayout.HelpBox("Video Player will be used for background on Advanced mode.", MessageType.Info);
            }

            GUILayout.Space(6);

            // Colors       
            var primaryColor = serializedObject.FindProperty("primaryColor");
            var secondaryColor = serializedObject.FindProperty("secondaryColor");
            var primaryReversed = serializedObject.FindProperty("primaryReversed");
            var negativeColor = serializedObject.FindProperty("negativeColor");
            var backgroundColor = serializedObject.FindProperty("backgroundColor");
            showColors = EditorGUILayout.Foldout(showColors, "Colors", foldoutStyle);

            if (showColors)
            {
                GUILayout.Space(2);
                EditorGUILayout.PropertyField(primaryColor, new GUIContent("Primary"));
                EditorGUILayout.PropertyField(secondaryColor, new GUIContent("Secondary"));
                EditorGUILayout.PropertyField(primaryReversed, new GUIContent("Primary Reversed"));
                EditorGUILayout.PropertyField(negativeColor, new GUIContent("Negative"));
                EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Background"));
            }

            GUILayout.Space(6);

            // Fonts
            var lightFont = serializedObject.FindProperty("lightFont");
            var regularFont = serializedObject.FindProperty("regularFont");
            var mediumFont = serializedObject.FindProperty("mediumFont");
            var semiBoldFont = serializedObject.FindProperty("semiBoldFont");
            var boldFont = serializedObject.FindProperty("boldFont");
            showFonts = EditorGUILayout.Foldout(showFonts, "Fonts", foldoutStyle);

            if (showFonts)
            {
                GUILayout.Space(2);
                EditorGUILayout.PropertyField(lightFont, new GUIContent("Light"));
                EditorGUILayout.PropertyField(regularFont, new GUIContent("Regular"));
                EditorGUILayout.PropertyField(mediumFont, new GUIContent("Medium"));
                EditorGUILayout.PropertyField(semiBoldFont, new GUIContent("Semibold"));
                EditorGUILayout.PropertyField(boldFont, new GUIContent("Bold"));
                EditorGUILayout.HelpBox("Supports only TextMesh Pro fonts. You can create them from Window > TextMesh Pro > Font Asset Creator.", MessageType.Info);
            }

            GUILayout.Space(6);

            // Logo
            var gameLogo = serializedObject.FindProperty("gameLogo");
            var logoColor = serializedObject.FindProperty("logoColor");
            showLogo = EditorGUILayout.Foldout(showLogo, "Logo", foldoutStyle);

            if (showLogo)
            {
                GUILayout.Space(4);
                EditorGUILayout.PropertyField(gameLogo, new GUIContent("Game Logo"));
                EditorGUILayout.PropertyField(logoColor, new GUIContent("Logo Color"));
            }

            GUILayout.Space(6);

            // Particles
            var particleColor = serializedObject.FindProperty("particleColor");
            showParticle = EditorGUILayout.Foldout(showParticle, "Particles", foldoutStyle);

            if (showParticle)
            {
                GUILayout.Space(4);
                EditorGUILayout.PropertyField(particleColor, new GUIContent("Color"));
            }

            GUILayout.Space(6);

            // Sounds
            var backgroundMusic = serializedObject.FindProperty("backgroundMusic");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");
            showSounds = EditorGUILayout.Foldout(showSounds, "Sounds", foldoutStyle);

            if (showSounds)
            {
                GUILayout.Space(4);
                EditorGUILayout.PropertyField(backgroundMusic, new GUIContent("Background Music"));
                EditorGUILayout.PropertyField(hoverSound, new GUIContent("Hover SFX"));
                EditorGUILayout.PropertyField(clickSound, new GUIContent("Click SFX"));
            }

            GUILayout.Space(7);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Space(6);

            var enableDynamicUpdate = serializedObject.FindProperty("enableDynamicUpdate");
            EditorGUILayout.PropertyField(enableDynamicUpdate, new GUIContent("Update Values"));

            if (enableDynamicUpdate.boolValue == false)
            {
                EditorGUILayout.HelpBox("'Update Values' option is disabled. Note that you must turn on this feature to update the UI values.", MessageType.Warning);
            }

            GUILayout.Space(7);

            var enableExtendedColorPicker = serializedObject.FindProperty("enableExtendedColorPicker");
            EditorGUILayout.PropertyField(enableExtendedColorPicker, new GUIContent("Extended Color Picker"));

            if (enableExtendedColorPicker.boolValue == true)
                EditorPrefs.SetInt("ShiftUIManager.EnableExtendedColorPicker", 1);

            else
                EditorPrefs.SetInt("ShiftUIManager.EnableExtendedColorPicker", 0);

            GUILayout.Space(7);

            var editorHints = serializedObject.FindProperty("editorHints");
            EditorGUILayout.PropertyField(editorHints, new GUIContent("UI Manager Hints"));

            if (editorHints.boolValue == true)
            {
                EditorGUILayout.HelpBox("These values are universal and will affect any object that contains 'UI Manager' component.", MessageType.Info);
                EditorGUILayout.HelpBox("Remove 'UI Manager' component from the object if you want unique values.", MessageType.Info);
				EditorGUILayout.HelpBox("Press 'Tools > Shift UI > Show UI Manager' to open UI Manager quickly.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(7);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Reset to defaults")))
                ResetToDefaults();

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Space(18);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
            GUILayout.Label("Need help? Contact me via:");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Discord")))
                Discord();

            if (GUILayout.Button(new GUIContent("E-mail")))
                Email();

            if (GUILayout.Button(new GUIContent("YouTube")))
                YouTube();

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Website")))
                Website();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(18);
            GUILayout.Label("Loved the package and wanna support us?");

            if (GUILayout.Button(new GUIContent("Write a review")))
                Review();
        }

        void Discord()
        {
            Application.OpenURL("https://discord.gg/VXpHyUt");
        }

        void Email()
        {
            Application.OpenURL("mailto:isa.steam@outlook.com?subject=Contact");
        }

        void YouTube()
        {
            Application.OpenURL("https://www.youtube.com/c/michsky");
        }

        void Website()
        {
            Application.OpenURL("https://www.michsky.com/");
        }

        void Review()
        {
            Application.OpenURL("https://assetstore.unity.com/account/assets");
        }

        void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset to defaults", "Are you sure you want to reset UI Manager values to default?", "Yes", "Cancel"))
            {
                try
                {
                    Preset defaultPreset = Resources.Load<Preset>("Shift UI Presets/Default");
                    defaultPreset.ApplyTo(Resources.Load("Shift UI Manager"));
                    Selection.activeObject = null;
                    Debug.Log("UI Manager - Resetting is successful.");
                }

                catch
                {
                    Debug.LogWarning("UI Manager - Resetting is failed.");
                }
            }
        }
    }
}
#endif