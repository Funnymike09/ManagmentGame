using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; private set; }

    public static int Hour { get; private set; }

    private float minuteToRealTIme = 0.5f;
    private float timer;

    private Stock[] stockList;

    // Start is called before the first frame update
    void Start()
    {
        Minute = 59;
        Hour = 10;
        timer = minuteToRealTIme;
        OnHourChanged += PriceChange;
        InitialiseStock();
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

        for (int i = 0; i < stockList.Length; i++)
        {
            stockList[i].myName = "Stock " + i;
            float randomStartValue = Random.Range(20f, 100f);
            randomStartValue = Mathf.Round(randomStartValue * 10.0f) * 0.1f;
            stockList[i].currentPrice = randomStartValue;
            Debug.Log(stockList[i].myName + " current price = " + stockList[i].currentPrice);
        }
    }

    void PriceChange()
    {
        for (int i = 0; i < stockList.Length; i++)
        {
            bool randomBool = Random.value < 0.5f;
            float randomIncrease = Random.Range(1f, 21f);
            randomIncrease = Mathf.Round(randomIncrease * 10.0f) * 0.1f;
            stockList[i].Change(randomBool, randomIncrease);
            Debug.Log(stockList[i].myName + " current price = " + stockList[i].currentPrice);
            Debug.Log(stockList[i].myName + " old price = " + stockList[i].oldPrice);
            Debug.Log(stockList[i].myName + " percentage change = " + stockList[i].priceChange);
        }

    }

    public struct Stock
    {
        public float currentPrice;
        public float oldPrice;
        public float priceChange;
        public string myName;

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
        }
    }
}
