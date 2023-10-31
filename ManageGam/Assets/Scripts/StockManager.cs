using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public float startingDoubloons;
    public float currentDoubloons;
    private TimeManager timeManager;
    [SerializeField] private TextMeshProUGUI doubloonsText;

    // Start is called before the first frame update
    void Start()
    {
        currentDoubloons = startingDoubloons;
        timeManager = FindObjectOfType<TimeManager>();
        doubloonsText.text = currentDoubloons.ToString();
    }

    public void BuyStock(/*float buyPrice,*/ int companyStockIndex)
    {
        //Debug.Log(timeManager.stockList[companyStockIndex].currentPrice);
        for (int i = 0; i < timeManager.stockList.Length; i++)
        {
            if (timeManager.stockList[i].myName.Contains(companyStockIndex.ToString()))
            {
                Debug.Log("Buying from " + timeManager.stockList[i].myName + " at price " + timeManager.stockList[i].currentPrice);
                if (currentDoubloons > timeManager.stockList[i].currentPrice)
                {
                    currentDoubloons -= timeManager.stockList[i].currentPrice;
                    timeManager.stockList[i].stockOwned++;
                    doubloonsText.text = currentDoubloons.ToString();
                }
                else
                {
                    // INVALID FUNDS
                    Debug.Log("Invalid funds");
                }
                i = timeManager.stockList.Length;
            }
        }

    }

    public void SellStock(/*float sellPrice,*/ int companyStockIndex)
    {
        for (int i = 0; i < timeManager.stockList.Length; i++)
        {
            if (timeManager.stockList[i].myName.Contains(companyStockIndex.ToString()))
            {
                Debug.Log("Selling from " + timeManager.stockList[i].myName + " at price " + timeManager.stockList[i].currentPrice);
                if (timeManager.stockList[i].stockOwned > 0)
                {
                    currentDoubloons += timeManager.stockList[i].currentPrice;
                    timeManager.stockList[i].stockOwned--;
                    doubloonsText.text = currentDoubloons.ToString();
                }
                else
                {
                    // INVALID STOCK AMOUNT
                    Debug.Log("Invalid stock amount");
                }
                i = timeManager.stockList.Length;
            }
        }
    }
}
