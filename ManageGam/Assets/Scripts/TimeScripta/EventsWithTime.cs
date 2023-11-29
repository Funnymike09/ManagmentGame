using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class EventsWithTime : MonoBehaviour
{
    
    public Button Mail;
    private VirusManager virusManager;

    private void Start()
    {
        virusManager = FindObjectOfType<VirusManager>();
    }

    private void OnEnable()
    {
        TimeManager.OnMinuteChanged +=TimeCheck;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= TimeCheck;
    }


    

    private void TimeCheck()
    {
        if (TimeManager.Hour == 10 && TimeManager.Minute == 5) 
            Mail.interactable = true;
            
        /*if (TimeManager.Hour % 2 != 0 && !virusManager.virusActive && TimeManager.Minute == 1)
        {
            Debug.Log("RUN!!!!!");
            virusManager.SpawnFirstWindow();
        }*/
    }

    public LineChart lineChart;

    void bruh()
    {
        
    }
}
