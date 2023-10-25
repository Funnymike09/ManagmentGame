using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public float startingDoubloons;
    public float currentDoubloons;
    private TimeManager timeManager;

    // Start is called before the first frame update
    void Start()
    {
        currentDoubloons = startingDoubloons;
        timeManager = FindObjectOfType<TimeManager>();
    }

    public void BuyStock(float buyPrice, int companyStockIndex)
    {
        currentDoubloons -= buyPrice;
        timeManager.stockList[companyStockIndex].stockOwned++;
    }

    public void SellStock(float sellPrice, int companyStockIndex)
    {
        if (timeManager.stockList[companyStockIndex].stockOwned > 0)
        currentDoubloons += sellPrice;
        timeManager.stockList[companyStockIndex].stockOwned++;
    }
}
