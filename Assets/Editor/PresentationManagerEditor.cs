using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresentationManager))]
public class PresentationManagerEditor : Editor
{
    private string slideName;

    #region Slides and TimelinesAssets
    SerializedProperty PresentaionSlidesTimeline;
    SerializedProperty PresentationSlides;
    #endregion

    private void OnEnable()
    {
        #region Slides and TimelinesAssets
        PresentaionSlidesTimeline = serializedObject.FindProperty("presentaionSlidesTimeline");
        PresentationSlides = serializedObject.FindProperty("presentationSlides");
        #endregion
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        #region Slides and TimelinesAssets
        EditorGUILayout.PropertyField(PresentaionSlidesTimeline, new GUIContent("PresentaionSlidesTimeline"), true);
        EditorGUILayout.PropertyField(PresentationSlides, new GUIContent("PresentationSlides"), true);
        #endregion
        EditorGUILayout.Space();

        Separator();

        #region Create Slides
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Slide name", GUILayout.Height(40), GUILayout.Width(110));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
        slideName = EditorGUILayout.TextField("", slideName, GUILayout.Height(40));
        GUI.skin.textField.alignment = TextAnchor.UpperLeft;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add new Slide", GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(slideName))
            {
                (target as PresentationManager).CreateSlide(slideName);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please give the slide a name to create\n\nNote : be carful with names for overwrite", "OK");
            }
        }
        EditorGUILayout.EndVertical();
        #endregion
    }

    private void Separator()
    {
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(90));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3);
    }
}
