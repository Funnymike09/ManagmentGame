using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class VirusManager : MonoBehaviour
{
    private TimeManager timeManager;
    [SerializeField] private float timeBetweenViruses;
    [SerializeField] private GameObject virusPrefab;
    private float timer;
    [SerializeField] private int numToSpawn;
    [Tooltip("When spawning, how much time passes between each spawn")][SerializeField] private float timeBetweenSpawns;
    private float minX, minY, maxX, maxY;
    private bool spawning;
    Vector2 pos;
    [SerializeField] private GameObject virusParent;
    // Start is called before the first frame update
    void Start()
    {
        timeManager = GetComponent<TimeManager>();
        timeManager.virusManager = this;
        this.enabled = false;
        timer = 0;
        spawning = false;
    }

    // Update is called once per frame
    void Update()
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
        for (int i = 0; i < numToSpawn; i++)
        {
            pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            GameObject virus = Instantiate(virusPrefab, pos, Quaternion.identity);
            virus.transform.parent = GameObject.FindGameObjectWithTag("VirusParent").transform; // This is not set currently!
            virus.transform.localScale = Vector3.one;
            if (virus.transform.position.x < minX || virus.transform.position.x > maxX || virus.transform.position.y < minY || virus.transform.position.y > maxY)
            {
                virus.transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            }
            virus.transform.SetAsFirstSibling();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        yield return null;
    }
}
