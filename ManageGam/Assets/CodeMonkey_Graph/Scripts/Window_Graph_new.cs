using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Window_Graph_new : MonoBehaviour
{
    private bool isBarChart = true;
    [SerializeField] private Sprite dotSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;

    // Cached values
    private List<int> valueList;
    private IGraphVisual lineGraphVisual;
    private IGraphVisual barChartVisual;
    private int minVisAmount;
    private int maxVisAmount;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;

    public void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();

        gameObjectList = new List<GameObject>();
        getAxisLabelX = (int _i) => "Trial " + (_i + 1);
        getAxisLabelY = (float _f) => Mathf.RoundToInt(_f) + "%";

        valueList = new List<int>() { 5, 98, 23, 44, 68, 34, 52, 54, 63, 64, 55, 50 };

        lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, 0.5f));
        barChartVisual = new BarChartVisual(graphContainer, Color.green, 0.8f);

        ShowGraph(this.valueList, barChartVisual, 0, valueList.Count, this.getAxisLabelX, this.getAxisLabelY);
    }

    public void SetGraph(bool isBarChart)
    {
        if (isBarChart)
        {
            this.isBarChart = isBarChart;
            ShowGraph(this.valueList, barChartVisual, this.minVisAmount, this.maxVisAmount, this.getAxisLabelX, this.getAxisLabelY);
        }
        else
        {
            this.isBarChart = isBarChart;
            ShowGraph(this.valueList, lineGraphVisual, this.minVisAmount, this.maxVisAmount, this.getAxisLabelX, this.getAxisLabelY);
        }
    }

    public void DecreaseStartValue()
    {
        if (minVisAmount > 0)
        {
            minVisAmount--;
            SetGraph(isBarChart);
        }
    }

    public void IncreaseStartValue()
    {
        if (minVisAmount <= maxVisAmount)
        {
            minVisAmount++;
            SetGraph(isBarChart);
        }
    }

    public void DecreaseEndValue()
    {
        if (maxVisAmount > minVisAmount + 1)
        {
            maxVisAmount--;
            SetGraph(isBarChart);
        }
    }

    public void IncreaseEndValue()
    {
        if (maxVisAmount < valueList.Count)
        {
            maxVisAmount++;
            SetGraph(isBarChart);
        }
    }

    private void CleanGraph()
    {
        // Clean up old graph before drawing new one
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
    }

    private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int minVisAmount = 0, int maxVisAmount = 5, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        this.valueList = valueList;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        // Set default label values
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        // if value is invalid display whole graph
        if (minVisAmount < 0)
        {
            minVisAmount = 0;
        }
        else if (minVisAmount >= maxVisAmount)
        {
            minVisAmount = maxVisAmount - 1;
        }
        if (maxVisAmount <= 0 || maxVisAmount > valueList.Count)
        {
            maxVisAmount = valueList.Count;
        }
        else if (maxVisAmount < minVisAmount)
        {
            maxVisAmount = minVisAmount + 1;
        }
        this.minVisAmount = minVisAmount;
        this.maxVisAmount = maxVisAmount;

        CleanGraph();

        // Set boundaries
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMax = valueList[0];
        float yMin = valueList[0];

        // Cycle through all our values and find the highest value
        for (int i = minVisAmount; i < maxVisAmount; i++)
        {
            int value = valueList[i];
            if (value > yMax)
            {
                yMax = value;
            }
            if (value < yMin)
            {
                yMin = value;
            }
        }

        float yDiff = yMax - yMin;
        if (yMax - yMin <= 0)
        {
            yDiff = 5f;
        }
        if (yMax + yDiff * 0.2f <= 100f) // Scale the graph without letting it go above 100%
        {
            yMax += yDiff * 0.2f;
        }
        else
        {
            yMax = 100f;
        }
        if (yMin - yDiff * 0.2f >= 0f) // Scale the graph without letting it go below 0%
        {
            yMin -= yDiff * 0.2f;
        }
        else
        {
            yMin = 0f;
        }

        // Plot the horizontal points and label them
        float xSize = graphWidth / ((maxVisAmount - minVisAmount) + 1);
        int xIndex = 0;

        for (int i = minVisAmount; i < maxVisAmount; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight; // Normalized Y position

            gameObjectList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize));

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.transform.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -7f);
            labelX.GetComponent<TMP_Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -7f);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
        }

        // Connect the points and create the vertical labels
        int sepCount = 10;
        for (int i = 0; i <= sepCount; i++)
        {
            float normValue = i * 1f / sepCount;

            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.transform.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-9f, normValue * graphHeight);
            labelY.GetComponent<TMP_Text>().text = getAxisLabelY(yMin + (normValue * (yMax - yMin)));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    private interface IGraphVisual
    {
        List<GameObject> AddGraphVisual(Vector2 graphPos, float graphPosWidth);
    }

    private class BarChartVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPos, float graphPosWidth)
        {
            GameObject barGameObject = CreateBar(graphPos, graphPosWidth);

            return new List<GameObject>() { barGameObject };
        }

        private GameObject CreateBar(Vector2 graphPos, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPos.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPos.y);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);

            return gameObject;
        }
    }

    private class LineGraphVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite dotSprite;
        private GameObject lastDotGameObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastDotGameObject = null;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPos, float graphPosWidth)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPos);
            gameObjectList.Add(dotGameObject);

            if (lastDotGameObject != null && dotGameObject.GetComponent<RectTransform>().anchoredPosition.x > lastDotGameObject.GetComponent<RectTransform>().anchoredPosition.x)
            {
                GameObject dotConnection = CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnection);
            }
            lastDotGameObject = dotGameObject;

            return gameObjectList;
        }

        private GameObject CreateDot(Vector2 anchorPos)
        {
            GameObject gameObject = new GameObject("dot", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchorPos;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);

            return gameObject;
        }

        private GameObject CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPosB - dotPosA).normalized;
            float distance = Vector2.Distance(dotPosA, dotPosB);

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchoredPosition = dotPosA + dir * distance * 0.5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));

            return gameObject;
        }
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
