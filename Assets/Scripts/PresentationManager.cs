using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PresentationManager : MonoBehaviour
{
    [SerializeField] private List<PlayableAsset> presentaionSlidesTimeline = new List<PlayableAsset>();
    [SerializeField] private List<PresentationSlide> presentationSlides = new List<PresentationSlide>();

    public PlayableDirector timelineDirector;

    public int slideIndex = 0;

    private bool isPlaying;

    private void Update()
    {
        #region Input from user
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Move to next slide
                Turn_On_Off_Slide(presentationSlides[slideIndex], false);

                slideIndex++;
                slideIndex %= presentaionSlidesTimeline.Count;

                Turn_On_Off_Slide(presentationSlides[slideIndex], true);

                PlaySelectedTimelineByIndex(slideIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Move to the previous
                Turn_On_Off_Slide(presentationSlides[slideIndex], false);

                slideIndex--;
                slideIndex = (slideIndex == -1) ? 0 : slideIndex;

                Turn_On_Off_Slide(presentationSlides[slideIndex], true);

                PlaySelectedTimelineByIndex(slideIndex);
            }
        }
        #endregion
    }

    /// <summary>
    /// This method for close or open slide
    /// </summary>
    /// <param name="slide">the selected slide to turn on or off</param>
    /// <param name="isOpen">the state of the slide</param>
    private void Turn_On_Off_Slide(PresentationSlide slide, bool isOpen)
    {
        slide.gameObject.SetActive(isOpen);
    }

    private void PlaySelectedTimelineByIndex(int timelineIndex)
    {
        if (presentaionSlidesTimeline[timelineIndex])
        {
            timelineDirector.playableAsset = presentaionSlidesTimeline[timelineIndex];
            timelineDirector.Play();
        }
    }

    /// <summary>
    /// This method for detect all slide down this gameobject
    /// </summary>
    public void FindAllSlide()
    {
        presentationSlides.Clear();
        foreach (PresentationSlide slide in GetComponentsInChildren<PresentationSlide>())
        {
            presentationSlides.Add(slide);
        }
    }

    /// <summary>
    /// Turn all slide off
    /// </summary>
    private void TurnAllSlideOff()
    {
        foreach (PresentationSlide slide in presentationSlides)
        {
            slide.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Add new slide Timeline 
    /// </summary>
    /// <param name="slideTimeLine"></param>
    public void AddNewSlideTimeLine(PlayableAsset slideTimeLine)
    {
        if (!presentaionSlidesTimeline.Contains(slideTimeLine))
        {
            presentaionSlidesTimeline.Add(slideTimeLine);
        }
    }

    /// <summary>
    /// Stop the Show Timeline and responding to input from user
    /// </summary>
    public void StopShow()
    {
        timelineDirector.Stop();
        isPlaying = false;
    }

    /// <summary>
    /// Resume the show and respond to input from user
    /// </summary>
    public void ResumeShow()
    {
        timelineDirector.Play();
        isPlaying = true;
    }

    /// <summary>
    /// Start the show from the begaining
    /// </summary>
    public void StartShow()
    {
        if (presentationSlides.Count > 0)
        {
            TurnAllSlideOff();
            slideIndex = 0;
            Turn_On_Off_Slide(presentationSlides[slideIndex], true);
            PlaySelectedTimelineByIndex(slideIndex);
            timelineDirector.Play();
            isPlaying = true;
        }
    }

    /// <summary>
    /// Restart the show
    /// </summary>
    public void RestartShow()
    {
        if (presentationSlides.Count > 0)
        {
            Turn_On_Off_Slide(presentationSlides[slideIndex], false);
            slideIndex = 0;
            Turn_On_Off_Slide(presentationSlides[slideIndex], true);
            PlaySelectedTimelineByIndex(slideIndex);
            timelineDirector.Play();
            timelineDirector.Pause();
        }
    }

    public void CreateSlide(string slideName)
    {
        CreateSlidesFolder();

        GameObject s = new GameObject(slideName, typeof(PresentationSlide));
        s.transform.SetParent(transform);
        Undo.RegisterCreatedObjectUndo(s, "PresentationSlide Create");
        AssetDatabase.CreateAsset(TimelineAsset.CreateInstance(typeof(TimelineAsset)), "Assets/SlidesTimeLine/" + slideName + ".playable");

        if (!timelineDirector.GetComponent<Animator>())
        {
            timelineDirector.gameObject.AddComponent<Animator>();
        }


        timelineDirector.playableAsset = AssetDatabase.LoadAssetAtPath("Assets/SlidesTimeLine/" + slideName + ".playable", typeof(TimelineAsset)) as PlayableAsset;

        // Add Animation Track
        TrackAsset t = (timelineDirector.playableAsset as TimelineAsset).CreateTrack(typeof(AnimationTrack), null, "");
        timelineDirector.SetGenericBinding(t, timelineDirector.GetComponent<Animator>());

        // Fetch Slides
        FindAllSlide();

        // Add the new timeline to presentation manager
        AddNewSlideTimeLine(timelineDirector.playableAsset);
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

    public void RemoveSlide(string slidName)
    {
        int index = presentationSlides.FindIndex(x => x.gameObject.name.Equals(slidName));
        if (index != -1)
        {
            presentationSlides.RemoveAt(index);
        }

        index = presentaionSlidesTimeline.FindIndex(x => x.name.Equals(slidName));
        if (index != -1)
        {
            presentaionSlidesTimeline.RemoveAt(index);
        }
    }
}
