using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class EventsWithTime : MonoBehaviour
{
    
    public Button Mail;
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
            

    }

    public LineChart lineChart;

    void bruh()
    {
        
    }
}
