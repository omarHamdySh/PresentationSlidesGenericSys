using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PresentationSlide))]
public class PresentationSlideEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Remove this slide", GUILayout.Height(40)))
        {
            (target as PresentationSlide).RemoveThisSlide();
        }
    }
}
