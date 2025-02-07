using UnityEditor;
using UnityEngine;
using Simple.SoundSystem.Core;

namespace Simple.SoundSystem.Core
{
    [CustomEditor(typeof(Sound))]
    public class SoundEditor : UnityEditor.Editor
    {
        Sound sound;
        private void OnEnable()
        {
            sound = (Sound)target;
        }
        public override void OnInspectorGUI()
        {
            var centerLabel = new GUIStyle(EditorStyles.boldLabel);
            centerLabel.alignment = TextAnchor.MiddleCenter;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myLibrary"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customName"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Single Clip", !sound.UseMultipleClipVariants ? centerLabel : new GUIStyle("minibuttonleft")))
            {
                serializedObject.FindProperty("useMultipleClipVariants").boolValue = false;
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("Multiple Clips", sound.UseMultipleClipVariants ? centerLabel : new GUIStyle("minibuttonright")))
            {
                serializedObject.FindProperty("useMultipleClipVariants").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();

            var clipProp = serializedObject.FindProperty("clip" + (sound.UseMultipleClipVariants ? "s" : ""));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(clipProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                sound.Rename();
            }

            if (GUILayout.Button(new GUIContent($"Preview Clip{(sound.UseMultipleClipVariants ? "s" : "")}", EditorGUIUtility.FindTexture("Animation.Play"))))
            {
                var toPlay = (clipProp.isArray ? clipProp.GetArrayElementAtIndex(UnityEngine.Random.Range(0, clipProp.arraySize)) : clipProp).objectReferenceValue.GetInstanceID();
                AssetDatabase.OpenAsset(toPlay);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            var randomizeProp = serializedObject.FindProperty("randomizePitch");
            var before = randomizeProp.boolValue;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pitch");
            if (before)
            {
                var minLimit = 0;
                var maxLimit = 3;
                var minValueProp = serializedObject.FindProperty("pitchMin");
                var maxValueProp = serializedObject.FindProperty("pitchMax");
                var minValue = minValueProp.floatValue;
                var maxValue = maxValueProp.floatValue;

                var minValueBefore = minValue;
                var maxValueBefore = maxValue;

                GUILayoutOption limitedWidth = GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.5f);

                EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);

                if (minValue != minValueBefore || maxValue != maxValueBefore)
                {
                    minValueProp.floatValue = minValue;
                    maxValueProp.floatValue = maxValue;
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(minValueProp, GUIContent.none, limitedWidth);
                EditorGUILayout.PropertyField(maxValueProp, GUIContent.none, limitedWidth);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pitch"), GUIContent.none);
            }
            GUILayout.EndHorizontal();

            var after = GUILayout.Toggle(before, "Randomize?");
            if (after != before)
            {
                randomizeProp.boolValue = after;
                serializedObject.ApplyModifiedProperties();
            }

            var validationProp = serializedObject.FindProperty("needsValidation");
            var beforeV = validationProp.boolValue;
            var afterV = GUILayout.Toggle(beforeV, "Needs Validation");
            if (afterV != beforeV)
            {
                validationProp.boolValue = afterV;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}