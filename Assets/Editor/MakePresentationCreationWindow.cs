using UnityEditor;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MakePresentaionCreationWindow : EditorWindow
{
    private Vector2 scrollPos;
    private PresentationManager presentationManager;
    private PlayableDirector playableDirector;
    private string slideName;

    [MenuItem("Tools/MakePresentation/CreationWindow", false, 0)]
    private static void InitPresentation()
    {
        GetWindowWithRect(typeof(MakePresentaionCreationWindow), new Rect(0, 0, 120, 235));
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Setup", GUILayout.Height(40), GUILayout.Width(110)))
            Setup();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        #region Separator
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(90));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3);
        #endregion

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Slide name", GUILayout.Height(40), GUILayout.Width(110));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
        slideName = EditorGUILayout.TextField("", slideName, GUILayout.Height(40), GUILayout.Width(110));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Slide", GUILayout.Height(40), GUILayout.Width(110)))
        {
            if (!string.IsNullOrEmpty(slideName))
            {
                CreateSlide();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please give the slide a name to create\n\nNote : be carful with names for overwrite", "OK");
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void Setup()
    {
        PresentationManager[] managers = FindObjectsOfType<PresentationManager>();
        if (managers.Length == 0)
        {
            GameObject pm = new GameObject("PresentationManager", typeof(PresentationManager));
            pm.transform.SetAsFirstSibling();
            presentationManager = pm.GetComponent<PresentationManager>();
            Undo.RegisterCreatedObjectUndo(pm, "PresentationManager Setup");
        }
        else if (managers.Length > 1)
        {
            EditorUtility.DisplayDialog("Error", "More than 1 \"PresentationManager\" found, please make sure there's only one.", "OK");
        }
        else if (managers.Length == 1)
        {
            presentationManager = managers[0];
        }

        if (!FindObjectOfType<CinemachineVirtualCamera>())
        {
            GameObject cm = new GameObject("CM vcam", typeof(CinemachineVirtualCamera));
            cm.transform.SetSiblingIndex(1);
            playableDirector = cm.AddComponent<PlayableDirector>();
            playableDirector.playOnAwake = false;
            Undo.RegisterCreatedObjectUndo(cm, "CM vcam Setup");
        }
        else
        {
            playableDirector = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<PlayableDirector>();
        }

        presentationManager.timelineDirector = playableDirector;

        if (Camera.main && !Camera.main.GetComponent<CinemachineBrain>())
        {
            Camera.main.gameObject.AddComponent<CinemachineBrain>();
        }
        else if (!Camera.main)
        {
            EditorUtility.DisplayDialog("Error", "There is no main cammera please create one and press setup again", "OK");
        }

        CreateSlidesFolder();
    }

    private void CreateSlide()
    {
        if (!presentationManager)
        {
            EditorUtility.DisplayDialog("Error", "please press setup button first", "OK");
            return;
        }

        CreateSlidesFolder();

        GameObject s = new GameObject(slideName, typeof(PresentationSlide));
        s.transform.SetParent(presentationManager.transform);
        Undo.RegisterCreatedObjectUndo(s, "PresentationSlide Create");
        AssetDatabase.CreateAsset(TimelineAsset.CreateInstance(typeof(TimelineAsset)), "Assets/SlidesTimeLine/" + slideName + ".playable");

        if (!playableDirector.GetComponent<Animator>())
        {
            playableDirector.gameObject.AddComponent<Animator>();
        }


        playableDirector.playableAsset = AssetDatabase.LoadAssetAtPath("Assets/SlidesTimeLine/" + slideName + ".playable", typeof(TimelineAsset)) as PlayableAsset;

        // Add Animation Track
        TrackAsset t = (playableDirector.playableAsset as TimelineAsset).CreateTrack(typeof(AnimationTrack), null, "");
        playableDirector.SetGenericBinding(t, playableDirector.GetComponent<Animator>());

        // Fetch Slides
        presentationManager.FindAllSlide();

        // Add the new timeline to presentation manager
        presentationManager.AddNewSlideTimeLine(playableDirector.playableAsset);
    }

    private void CreateSlidesFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/SlidesTimeLine"))
        {
            string guid = AssetDatabase.CreateFolder("Assets", "SlidesTimeLine");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.Refresh();
        }
    }
}
