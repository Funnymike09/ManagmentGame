using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyScript : MonoBehaviour
{
    private TimeManager timeManager;

    private void Start()
    {
        timeManager = GetComponent<TimeManager>();
    }

    public void OnClick()
    {
        Application.OpenURL("https://forms.office.com/e/Bwv5htBStt");
    }
}
