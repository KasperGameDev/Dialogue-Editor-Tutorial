using KasperDev.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class PlayVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private GameEventSO _videoEndEvent;

    private void Awake()
    {
        _videoPlayer.loopPointReached += VideoDone;
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    private void VideoDone(VideoPlayer vp)
    {
        _videoEndEvent.Raise();
    }
}
