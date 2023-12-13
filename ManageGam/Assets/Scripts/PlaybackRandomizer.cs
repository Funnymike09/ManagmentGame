using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlaybackRandomizer : MonoBehaviour
{
    public float playbackSpeed;
    public VideoPlayer videoPlayer;

    

    // Update is called once per frame
    void Update()
    {
        playbackSpeed = Random.Range(0,10);
        videoPlayer.playbackSpeed = playbackSpeed;
    }
}
