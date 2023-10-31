using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using XCharts.Runtime;
using XCharts;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; private set; }

    public static int Hour { get; private set; }

    private float minuteToRealTIme = 0.5f;
    private float timer;

    public Stock[] stockList;
    public Color[] colorList = new Color[5];
    [SerializeField] private float minStartValue, maxStartValue;
    [SerializeField] private float minIncreaseValue, maxIncreaseValue;
    [SerializeField] private float minStockValue;
    [SerializeField] private BaseChart chartManager; // the base chart handling all the graphs
    private int xData = 0; // This will go up with each hour, used to display the y graph
    [SerializeField] private int maxXData = 6;
    //public Serie[] companySeries;

    // Start is called before the first frame update
    void Start()
    {
        Minute = 58;
        Hour = 10;
        timer = minuteToRealTIme;
        OnHourChanged += PriceChange;
        OnHourChanged += AddGraphData;
        InitialiseStock();
        AddGraphData();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();
            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;
                OnHourChanged?.Invoke();
            }

            timer = minuteToRealTIme;
        }
    }

    void InitialiseStock()
    {
        stockList = new Stock[5];
        for (int i = 0; i < stockList.Length; i++) // randomize stock
        {
            stockList[i].myName = "Company " + i;
            float randomStartValue = Random.Range(minStartValue, maxStartValue);
            randomStartValue = Mathf.Round(randomStartValue * 10.0f) * 0.1f;
            stockList[i].currentPrice = randomStartValue;
            //Debug.Log(stockList[i].myName + " current price = " + stockList[i].currentPrice);
            stockList[i].myColor = colorList[i];
        }
    }

    void PriceChange()
    {
        for (int i = 0; i < stockList.Length; i++)
        {
            bool randomBool = Random.value < 0.5f;
            float randomIncrease = Random.Range(minIncreaseValue, maxIncreaseValue);
            randomIncrease = Mathf.Round(randomIncrease * 10.0f) * 0.1f;
            stockList[i].Change(randomBool, randomIncrease);
            /*Debug.Log(stockList[i].myName + " current price = " + stockList[i].currentPrice);
            Debug.Log(stockList[i].myName + " old price = " + stockList[i].oldPrice);
            Debug.Log(stockList[i].myName + " percentage change = " + stockList[i].priceChange);*/
        }
        xData++; // Hour passed
    }

    public struct Stock
    {
        public float currentPrice;
        public float oldPrice; // do we need this?
        public float priceChange;
        public string myName;
        public int stockOwned;
        public Color myColor;

        public void Change(bool positiveIncrease, float increasePercent)
        {
            oldPrice = currentPrice;
            int increaseMultiplier;
            if (positiveIncrease)
            {
                increaseMultiplier = 1;
            }
            else
            {
                increaseMultiplier = -1;
            }
            priceChange = increasePercent * increaseMultiplier;
            currentPrice += priceChange;

            if (currentPrice < 5)
            {
                currentPrice = 5;
            }
        }
    }

    void AddGraphData()
    {
        int i = 0;

        if (xData < maxXData) // Only applies if there are less than maxXData (6) amount of seriedatas in serie
        {
            foreach (Serie serie in chartManager.series) // Getting each individual graph from the chart (Serie 0-4)
            {
                SerieData serieData = serie.AddXYData(xData, stockList[i].currentPrice);
                i++;
            }
        }

        else // applies past 6 data points
        {
            int ii = 0;
            foreach (Serie serie in chartManager.series)
            {
                if (ii == 0)
                {
                    // Delete oldest serie?
                }
                //Create new serie
            }
        }
    }

    public void SetCompanyActive(int serieIndex)
    {
        if (chartManager.series[serieIndex].show == false)
        {
            chartManager.series[serieIndex].show = true;
        }
        else
        {
            chartManager.series[serieIndex].show = false;
        }
    } 
}
