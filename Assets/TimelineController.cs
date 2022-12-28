using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public static TimelineController instance;

    public PlayableDirector endingTimeline;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        endingTimeline.gameObject.SetActive(false);
    }

    public void ShowEndingTimeline()
    {
        endingTimeline.gameObject.SetActive(true);
        endingTimeline.Play();
    }

    public void ReadyToGoMainMenuScene()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        pc.isEnding = true;
    }
}
