using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class VirusManager : MonoBehaviour
{
    //private TimeManager timeManager;
    [SerializeField] private float timeBetweenViruses;
    [SerializeField] private GameObject virusPrefab;
    private StockManager stockManager;
    public GameObject startVirusWindow;
    [SerializeField] private float amountToPay;
    private float timer;
    [SerializeField] private int numToSpawn;
    [Tooltip("When spawning, how much time passes between each spawn")][SerializeField] private float timeBetweenSpawns;
    private float minX, minY, maxX, maxY;
    private bool spawning;
    //public bool paused;
    Vector2 pos;
    [SerializeField] private GameObject virusParent;
    public bool virusActive;
    private TimeManager timeManager;
    [SerializeField] private float virusBorderDistance;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        spawning = false;
        timeManager = FindObjectOfType<TimeManager>();
        stockManager = FindObjectOfType<StockManager>();
        SetMinAndMax();
    }

    // Update is called once per frame
    void Update()
    {
        if (virusActive && !timeManager.paused)
        { 
            if (timer < timeBetweenViruses && !spawning)
            {
                timer += Time.deltaTime;
            }
            if (timer >= timeBetweenViruses)
            {
                StartCoroutine(SpawnVirus());
            }
        }
    }

    private void SetMinAndMax()
    {
        Vector2 bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        minX = -bounds.x;
        minY = -bounds.y;
        maxX = bounds.x;
        maxY = bounds.y;
    }

    IEnumerator SpawnVirus()
    {
        spawning = true;
        timer = 0;
        for (int i = 0; i < numToSpawn; i++)
        {
            pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            Debug.Log(pos);
            GameObject virus = Instantiate(virusPrefab, pos, Quaternion.identity);
            virus.transform.parent = virusParent.transform;
            virus.transform.localScale = Vector3.one;
            if (virus.transform.position.x < minX || virus.transform.position.x > maxX || virus.transform.position.y < minY || virus.transform.position.y > maxY)
            {
                virus.transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            }
            virus.transform.SetAsFirstSibling();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        spawning = false;
        yield return null;
    }

    public void SpawnFirstWindow()
    {
        startVirusWindow.SetActive(true);
    }

    public void OnClickYes()
    {
        virusActive = true;
        stockManager.AddDoubloons(amountToPay);
        timeManager.UpdateNetWorth();
        startVirusWindow.SetActive(false);
    }

    public void OnClickNo()
    {
        virusActive = false;
        timeManager.UpdateNetWorth();
        startVirusWindow.SetActive(false);
    }
}
