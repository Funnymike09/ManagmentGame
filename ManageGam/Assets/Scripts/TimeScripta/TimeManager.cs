using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using XCharts.Runtime;
using XCharts;
using Unity.VisualScripting;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; private set; }

    public static int Hour { get; private set; }

    public static int day;

    public float minuteToRealTime = 0.3f;
    private float timer;

    public Stock[] stockList;
    public Color[] colorList = new Color[5];
    public float minStartValue, maxStartValue;
    public float minIncreaseValue, maxIncreaseValue;
    [SerializeField] private float minStockValue;
    [Tooltip("How long news stays active when called")][SerializeField] private int timeNewsActive;
    [SerializeField] private BaseChart chartManager; // the base chart handling all the graphs
    private int xData = 0; // This will go up with each hour, used to display the y graph
    [SerializeField] private int maxXData = 6;
    [SerializeField] private GameObject newsObject;
    private SetNews setNews;
    private int? randomChangeStateType;
    public PriceChangeState priceChangeState;
    private bool finished;
    [SerializeField] private GameObject endOfDemo;
    public VirusManager virusManager;
    [SerializeField] private GameObject dayCard;
    public float playerNetWorth;
    private StockManager stockManager;
    [SerializeField] private TextMeshProUGUI netWorthText;
    [SerializeField] private int startingMin, startingHour;
    [SerializeField] private int /*endingHour, */virusHour;
    [SerializeField] private TextMeshProUGUI endingScoreText;
    [SerializeField] private GameObject virusMail;
    [SerializeField] private GameObject mailNotification;
    [SerializeField] private GameObject emailCloseButton;
    [SerializeField] private GameObject winLetter, loseLetter;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI[] changePercentageTexts = new TextMeshProUGUI[5];
    public bool emailRead;
    public bool paused;
    public bool isFastForward;
    public bool debugging;
    [SerializeField] private Image fadeToBlack;
    private AudioSource audioSource;
    [SerializeField] private AudioClip knock;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private AudioSource emailAudio;
    [SerializeField] private GameObject music;

    public enum PriceChangeState
    {
        bad,
        semiBad,
        neutral,
        semiGood,
        good,
        extremeGamble
    }

    void Start()
    {
        Minute = startingMin;
        Hour = startingHour;
        day = 1;
        timer = minuteToRealTime;
        OnHourChanged += PriceChange;
        OnHourChanged += AddGraphData;
        InitialiseStock();
        AddGraphData();
        setNews = newsObject.GetComponentInChildren<SetNews>();
        newsObject.SetActive(false);
        stockManager = FindObjectOfType<StockManager>();
        audioSource = GetComponent<AudioSource>();
        UpdateNetWorth();
        paused = true;
        dayText.text = "Day " + day.ToString();
    }

    void Update()
    {
        if (!paused && !finished)
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

                timer = minuteToRealTime;
            }
        }
    }

    void InitialiseStock()
    {
        stockList = new Stock[5];
        for (int i = 0; i < stockList.Length; i++) // randomize stock
        {
            stockList[i].myName = "Company " + (i + 1);
            float randomStartValue = Random.Range(minStartValue, maxStartValue);
            randomStartValue = Mathf.Round(randomStartValue * 10.0f) * 0.1f;
            stockList[i].currentPrice = randomStartValue;
            stockList[i].myColor = colorList[i];
            stockList[i].changeText = changePercentageTexts[i];
            stockList[i].greenArrow = stockList[i].changeText.transform.GetChild(0).gameObject;
            stockList[i].redArrow = stockList[i].changeText.transform.GetChild(1).gameObject;
            stockList[i].redArrow.SetActive(false);
            //Debug.Log("i " + stockList[i].redArrow);
        }
    }

    IEnumerator DayChange(float dayCardSeconds)
    {
        //Play day change
        //Show UI of day change
        Debug.Log("Day change rahhhhh");
        Hour = 10;
        day++;
        dayText.text = "Day " + day.ToString();
        switch (day) // This will be where different things will be unlocked depending on what day it is
        {
            case 4:
                {
                    if (stockManager.MetWinCondition())
                    {
                        Debug.Log("WIN");
                        finished = true;
                        UpdateNetWorth();
                        winLetter.SetActive(true);
                        mailNotification.SetActive(true);
                        emailAudio.PlayOneShot(emailAudio.clip);
                        // Hour = 0; // ERROR: THIS NEEDS TO REFERENCE THE TEXT COMPONENT
                        // YOU WIN
                    }
                    else
                    {
                        Debug.Log("LOSE");
                        finished = true;
                        UpdateNetWorth();
                        loseLetter.SetActive(true);
                        mailNotification.SetActive(true);
                        emailAudio.PlayOneShot(emailAudio.clip);
                        //Hour = 0; // ERROR: THIS NEEDS TO REFERENCE THE TEXT COMPONENT
                        // YOU LOSE
                    }
                    break;
                }
            default:
                {
                    TextMeshProUGUI text = dayCard.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = "Day " + day.ToString();
                    dayCard.SetActive(true);
                    yield return new WaitForSeconds(dayCardSeconds);
                    dayCard.SetActive(false);
                    break;
                }
        }
        yield return null;
    }

    void PriceChange() // This also runs every hour
    {
        if (Hour == virusHour && day == 2)
        {
            virusMail.SetActive(true);
            mailNotification.SetActive(true);
        }
        if (Hour == 24)
        {
            StartCoroutine(DayChange(5));
        }
        // if (day == ? && Hour == ?) { OfferVirus(); } // need to decide when to call this
        if (!finished)
        {
            if (Hour == 11 || Hour == 18) // IMPORTANT: THIS SHOULD ONLY BE HERE FOR THE VERTICAL SLICE. FULL GAME THIS SHOULD BE RUNNING ONCE PER DAY, FOR TWO HOURS. UNLESS WE WANT IT AT A SET HOUR EVERY DAY.
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
                setNews.activeCompany = stockList[stockWithNews].myName;
                setNews.UpdateNews();
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
                    if (stockList[i].newsActiveTime == 0)
                    {
                        newsObject.SetActive(true);
                    }
                    if (stockList[i].newsActiveTime >= timeNewsActive) // When the news should no longer be active
                    {
                        Debug.Log("News finished");
                        newsObject.SetActive(false);
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
                        stockList[i].ControlledChange(priceChangeState/*, minIncreaseValue, maxIncreaseValue*/);
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
            UpdateNetWorth();
        }
    }

    public struct Stock
    {
        public float currentPrice;
        public float oldPrice;
        public float priceChange;
        public string myName;
        public int stockOwned;
        public Color myColor;
        public bool newsActive;
        public int newsActiveTime;
        public float priceChangePercentage;
        public TextMeshProUGUI changeText;
        public GameObject greenArrow, redArrow;

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

            priceChangePercentage = (currentPrice - oldPrice) / oldPrice * 100;

            if (priceChangePercentage >= 0)
            {
                changeText.text = "+" + priceChangePercentage.ToString("F2") + "%";
                greenArrow.SetActive(true);
                redArrow.SetActive(false);
            }
            else
            {
                changeText.text = priceChangePercentage.ToString("F2") + "%";
                greenArrow.SetActive(false);
                redArrow.SetActive(true);
            }
        }

        public void ControlledChange(PriceChangeState pCS/*, float minIncreaseValue, float maxIncreaseValue*/) // For companies when news is active
        {
            oldPrice = currentPrice;
            switch (pCS) // These values should be based off the min and maxIncrease values
            {
                case PriceChangeState.bad: // min = 5, max = 25
                    {
                        //priceChange = Random.Range(maxIncreaseValue / 2, maxIncreaseValue / 1.5f) * -1;
                        priceChange = Random.Range(-12.5f, -17f);
                        currentPrice += priceChange;
                        break;
                    }
                case PriceChangeState.semiBad:
                    {
                        //priceChange = Random.Range(maxIncreaseValue / 2, maxIncreaseValue / 3) * -1;
                        priceChange = Random.Range(-12.5f, -8f);
                        currentPrice += priceChange;
                        break;
                    }
                case PriceChangeState.neutral:
                    {
                        priceChange = Random.Range(-2.5f, 5f);
                        currentPrice += priceChange;
                        break;
                    }
                case PriceChangeState.semiGood:
                    {
                        //priceChange = Random.Range(maxIncreaseValue / 2, maxIncreaseValue / 3);
                        priceChange = Random.Range(12.5f, 8f);
                        currentPrice += priceChange;
                        break;
                    }
                case PriceChangeState.good:
                    {
                        priceChange = Random.Range(12.5f, 17f);
                        currentPrice += priceChange;
                        break;
                    }
                case PriceChangeState.extremeGamble:
                    {
                        int goodOrBad = Random.Range(0, 2);
                        if (goodOrBad == 0)
                        {
                            priceChange = -25f;
                        }
                        else
                        {
                            priceChange = 25f;
                        }
                        currentPrice += priceChange;
                        break;
                    }
                default:
                    {
                        // PANIC
                        Debug.Log("Error in ControlledChange() function");
                        break;
                    }
            }

            if (currentPrice < 5)
            {
                currentPrice = 5;
            }

            priceChangePercentage = (currentPrice - oldPrice) / oldPrice * 100;
            Debug.Log("Active news company changed by " + priceChange);
        }
    }

    float RoundDecimals(float number)
    {
        decimal temp = number.ConvertTo<decimal>();
        temp = decimal.Round(temp, 2);
        return temp.ConvertTo<float>();
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

    public void UpdateNetWorth()
    {
        playerNetWorth = 0;
        for (int i = 0; i < stockList.Length; i++)
        {
            playerNetWorth += stockList[i].currentPrice * stockList[i].stockOwned;
        }
        playerNetWorth += stockManager.currentDoubloons;
        netWorthText.text = playerNetWorth.ToString("F2");
    }

    public void PauseAndUnpause(bool wantToPause)
    {
        if (wantToPause && emailRead)
        {
            paused = true;
        }
        else if (!wantToPause && emailRead)
        {
            paused = false;
        }
    }
    public void FastForward(bool isFastForward)
    {
        if (isFastForward && debugging)
        {
            minuteToRealTime = 0.01f;
        }
        else if (isFastForward)
        {
            minuteToRealTime = 0.15f;
        }
        else if (!isFastForward)
        {
            minuteToRealTime = 0.3f;
        }
    }
    public void OnEmailSwitch()
    {
        emailCloseButton.SetActive(true);
    }

    public void OnEmailClose()
    {
        emailRead = true;
        paused = false;
    }

    public void YouLose()
    {
        StartCoroutine(OnLose());
    }

    public IEnumerator OnLose()
    {
        music.SetActive(false);
        fadeToBlack.gameObject.SetActive(true);
        fadeToBlack.transform.SetAsLastSibling();
        Color objectColor = fadeToBlack.color;
        float alpha;
        while (fadeToBlack.color.a < 1)
        {
            alpha = fadeToBlack.color.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(fadeToBlack.color.r, fadeToBlack.color.g, fadeToBlack.color.b, alpha);
            fadeToBlack.color = objectColor;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(knock);
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}
