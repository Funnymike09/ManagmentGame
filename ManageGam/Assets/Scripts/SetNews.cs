using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetNews : MonoBehaviour
{
    private TimeManager timeManager;
    [SerializeField] private TextMeshProUGUI newsText;
    public string activeCompany;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        newsText = GetComponent<TextMeshProUGUI>();

        UpdateNews();
    }

    public void UpdateNews()
    {
        switch (timeManager.priceChangeState)
        {
            case TimeManager.PriceChangeState.bad:
                {
                    newsText.text = activeCompany + " shares drop after a reportedly cancelled order from China. " + activeCompany + " sales reports show a huge drop in sales.";
                    break;
                }
            case TimeManager.PriceChangeState.semiBad:
                {
                    newsText.text = "Due to rising price in labor, " + activeCompany + " has laid off a large amount of employees. Analytics say that " + activeCompany + " shares will be down if they don't do something about it.";
                    break;
                }
            case TimeManager.PriceChangeState.neutral:
                {
                    newsText.text = "Reports show a stall in company sales due to recent changes in international law. " + activeCompany + " shares have seemed to plateau and don't seem to be going down.";
                    break;
                }
            case TimeManager.PriceChangeState.semiGood:
                {
                    newsText.text = "The Prime Minister of Portugal was confirmed to be using " + activeCompany + " products. " + activeCompany + " confirmed to be partnering with local companies in their production.";
                    break;
                }
            case TimeManager.PriceChangeState.good:
                {
                    newsText.text = activeCompany + " has released a long awaited product that people stand in line for. Reports say that " + activeCompany + " has a big government order to complete.";
                    break;
                }
            case TimeManager.PriceChangeState.extremeGamble:
                {
                    newsText.text = activeCompany + " CEO has shared their opinion on current world situations... Rumors are that " + activeCompany + " will be doing a rebrand. Consumers are unsure how this will affect stock prices.";
                    break;
                }
            default:
                {
                    // PANIC
                    Debug.Log("PriceChangeState on TimeManager is null! :(");
                    break;
                }
        }
    }
}
