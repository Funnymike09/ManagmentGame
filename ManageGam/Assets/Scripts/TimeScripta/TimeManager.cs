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
    [Tooltip("How long news stays active when called")][SerializeField] private int timeNewsActive;
    [SerializeField] private BaseChart chartManager; // the base chart handling all the graphs
    private int xData = 0; // This will go up with each hour, used to display the y graph
    [SerializeField] private int maxXData = 6;
    [SerializeField] private GameObject newsPrefab;
    private int? randomChangeStateType;
    [SerializeField] private PriceChangeState priceChangeState;

    public enum PriceChangeState
    {
        bad,
        semiBad,
        neutral,
        semiGood,
        good,
        extremeGamble
    }

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
            stockList[i].myColor = colorList[i];
        }
    }

    void PriceChange()
    {
        if (Hour == 12) // IMPORTANT: THIS SHOULD ONLY BE HERE FOR THE VERTICAL SLICE. FULL GAME THIS SHOULD BE RUNNING ONCE PER DAY, FOR TWO HOURS. UNLESS WE WANT IT AT A SET HOUR EVERY DAY.
        {
            int stockWithNews = Random.Range(0, 5);
            randomChangeStateType = Random.Range(0, 6);
            switch (randomChangeStateType) // Setting the state of the PriceChangeState according to a random selection
            {
                case 0:
                    {
                        priceChangeState = PriceChangeState.bad;
                        break;
                    }
                case 1:
                    {
                        priceChangeState = PriceChangeState.semiBad;
                        break;
                    }
                case 2:
                    {
                        priceChangeState = PriceChangeState.neutral;
                        break;
                    }
                case 3:
                    {
                        priceChangeState = PriceChangeState.semiGood;
                        break;
                    }
                case 4:
                    {
                        priceChangeState = PriceChangeState.good;
                        break;
                    }
                case 5:
                    {
                        priceChangeState = PriceChangeState.extremeGamble;
                        break;
                    }
                default:
                    {
                        // PANIC
                        break;
                    }
            }
            stockList[stockWithNews].newsActive = true;
        }
        for (int i = 0; i < stockList.Length; i++)
        {
            if (!stockList[i].newsActive) // Non-news related increases will be completely random
            {
                bool randomBool = Random.value < 0.5f;
                float randomIncrease = Random.Range(minIncreaseValue, maxIncreaseValue);
                randomIncrease = Mathf.Round(randomIncrease * 10.0f) * 0.1f;
                stockList[i].Change(randomBool, randomIncrease);
            }
            else // If the news is still active for given stock
            {
                if (stockList[i].newsActiveTime >= timeNewsActive) // When the news should no longer be active
                {
                    stockList[i].newsActive = false;
                    stockList[i].newsActiveTime = 0;
                    bool randomBool = Random.value < 0.5f;
                    float randomIncrease = Random.Range(minIncreaseValue, maxIncreaseValue);
                    randomIncrease = Mathf.Round(randomIncrease * 10.0f) * 0.1f;
                    stockList[i].Change(randomBool, randomIncrease);
                    randomChangeStateType = null;
                }
                else
                {
                    stockList[i].ControlledChange(priceChangeState);
                    stockList[i].newsActiveTime++;
                }
                Debug.Log("News active!");
                // INSTANTIATE PREFAB
            }
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
        //public PriceChangeState priceChangeState;
        public bool newsActive;
        public int newsActiveTime;

        public void Change(bool positiveIncrease, float increasePercent) // this will only be used for the 1st price change
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

        public void ControlledChange(PriceChangeState pCS) // For companies when news is active
        {
            switch (pCS) // These values should be based off the min and maxIncrease values
            {
                case PriceChangeState.bad:
                    {
                        priceChange = 1;
                        break;
                    }
                case PriceChangeState.semiBad:
                    {

                        break;
                    }
                case PriceChangeState.neutral:
                    {

                        break;
                    }
                case PriceChangeState.semiGood:
                    {

                        break;
                    }
                case PriceChangeState.good:
                    {

                        break;
                    }
                case PriceChangeState.extremeGamble:
                    {

                        break;
                    }
                default:
                    {
                        // PANIC
                        Debug.Log("Error in ControlledChange() function");
                        break;
                    }
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
