using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class PresentationSlide : MonoBehaviour
{
    public void RemoveThisSlide()
    {
        PresentationManager presentationManager = GetComponentInParent<PresentationManager>();
        if (presentationManager)
        {
            presentationManager.RemoveSlide(gameObject.name);

            AssetDatabase.DeleteAsset("Assets/SlidesTimeLine/" + gameObject.name + ".playable");
            AssetDatabase.Refresh();

            DestroyImmediate(gameObject);
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "no presentation manager Found", "OK");
        }
    }
}
