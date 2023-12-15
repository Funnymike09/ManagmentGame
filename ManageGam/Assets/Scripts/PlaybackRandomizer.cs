using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlaybackRandomizer : MonoBehaviour
{
    public float playbackSpeed;
    public VideoPlayer videoPlayer;
    private bool delayBool;


    

    // Update is called once per frame
    void Update()
    {

        StartCoroutine(delay());
    }

    public void Randomiser()
    {
        playbackSpeed = Random.Range(1, 10);
        videoPlayer.playbackSpeed = playbackSpeed;
    }
    IEnumerator delay()
    {
        if (!delayBool)
        {
            Randomiser();
            delayBool = true;
            yield return new WaitForSeconds(1);
            delayBool = false;
            
        }
    }

    
}
