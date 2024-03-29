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
    [SerializeField] private TextMeshProUGUI[] stockOwnedText;
    public bool canBuyAndSell;
    public float targetGoal;

    // Start is called before the first frame update
    void Start()
    {
        currentDoubloons = startingDoubloons;
        timeManager = FindObjectOfType<TimeManager>();
        doubloonsText.text = currentDoubloons.ToString("F2");
        timeManager.UpdateNetWorth();
        canBuyAndSell = true;
    }

    public void BuyStock(/*float buyPrice,*/ int companyStockIndex)
    {
        if (canBuyAndSell)
        {
            int tempIndex = companyStockIndex + 1;
            for (int i = 0; i < timeManager.stockList.Length; i++)
            {
                if (timeManager.stockList[i].myIndex.Contains(tempIndex.ToString()))
                {
                    Debug.Log("Buying from " + timeManager.stockList[i].myIndex + " at price " + timeManager.stockList[i].currentPrice);
                    if (currentDoubloons > timeManager.stockList[i].currentPrice)
                    {
                        currentDoubloons -= timeManager.stockList[i].currentPrice;
                        timeManager.stockList[i].stockOwned++;
                        doubloonsText.text = currentDoubloons.ToString("F2"); // This needs to be to max 2 decimal points
                        stockOwnedText[companyStockIndex].text = timeManager.stockList[i].stockOwned.ToString();
                    }
                    else
                    {
                        // INVALID FUNDS
                        Debug.Log("Invalid funds");
                    }
                    i = timeManager.stockList.Length;
                }
            }
            timeManager.UpdateNetWorth();
        }
    }

    public void SellStock(/*float sellPrice,*/ int companyStockIndex)
    {
        if (canBuyAndSell)
        {
            int tempIndex = companyStockIndex + 1;
            for (int i = 0; i < timeManager.stockList.Length; i++)
            {
                if (timeManager.stockList[i].myIndex.Contains(tempIndex.ToString()))
                {
                    Debug.Log("Selling from " + timeManager.stockList[i].myIndex + " at price " + timeManager.stockList[i].currentPrice);
                    if (timeManager.stockList[i].stockOwned > 0)
                    {
                        currentDoubloons += timeManager.stockList[i].currentPrice;
                        timeManager.stockList[i].stockOwned--;
                        doubloonsText.text = currentDoubloons.ToString("F2"); // This needs to be to max 2 decimal points
                        stockOwnedText[companyStockIndex].text = "(" + timeManager.stockList[i].stockOwned.ToString() + ")";
                    }
                    else
                    {
                        // INVALID STOCK AMOUNT
                        Debug.Log("Invalid stock amount");
                    }
                    i = timeManager.stockList.Length;
                }
            }
            timeManager.UpdateNetWorth();
        }
    }

    public void AddDoubloons(float doubloonsToAdd)
    {
        currentDoubloons += doubloonsToAdd;
        doubloonsText.text = currentDoubloons.ToString("F2");
        timeManager.UpdateNetWorth();
    }

    public bool MetWinCondition()
    {
        if (timeManager.playerNetWorth >= targetGoal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
