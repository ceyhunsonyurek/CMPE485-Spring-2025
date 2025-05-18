using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;

    [Header("Scoring")]
    public static int currentScore = 0;
    public static int highScore = 0;
    public GameObject gameOverPanel;
    public Text scoreText;
    public Text finalScoreText;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public bool isPoolingActive;

    [Header("Player Reference")]
    public GameObject player;

    [Header("Level Generation")]
    public float segmentLength = 96f;
    public int initialSegments = 4;
    public int maxActiveSegments = 6;
    public GameObject segment1;
    public GameObject segment2;

    [Header("Object Pooling")]
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private int spawnCount = 1;
    private string spawnTag;
    private List<GameObject> activeSegments = new List<GameObject>();
    private float playerLastZ = 0f;
    private bool hasInitializedLevel = false;

    private void Start()
    {
        if (isPoolingActive)
        {
            InitializePools();

            playerLastZ = player.transform.position.z;
            SpawnInitialSegments();
        }
    }

    private void Update()
    {
        if (isGameOver)
        {
            finalScoreText.text = "Your score is: " + currentScore;
            gameOverPanel.SetActive(true);
        }
        if (isPoolingActive)
        {
            float playerCurrentZ = player.transform.position.z;
            currentScore = Mathf.RoundToInt(playerCurrentZ);
            scoreText.text = "Score: " + currentScore;
            float distanceMoved = playerCurrentZ - playerLastZ;

            if (distanceMoved >= segmentLength)
            {
                SpawnNextSegment();
                playerLastZ = playerCurrentZ;

                CleanupOldSegments();
            }
        }
        else
        {
            if (hasInitializedLevel)
            {
                float playerCurrentZ = player.transform.position.z;
                currentScore = Mathf.RoundToInt(playerCurrentZ);
                scoreText.text = "Score: " + currentScore;
                float distanceMoved = playerCurrentZ - playerLastZ;

                if (distanceMoved >= 24)
                {
                    playerLastZ = playerCurrentZ;
                    int randomNumber = Random.Range(0, 2);
                    if (randomNumber == 0)
                    {
                        Vector3 spawnPosition = new Vector3(0, 0, segmentLength * spawnCount);
                        spawnCount++;
                        SpawnWithoutPool(segment1, spawnPosition, Quaternion.identity);
                    }
                    else
                    {
                        Vector3 spawnPosition = new Vector3(0, 0, segmentLength * spawnCount);
                        spawnCount++;
                        SpawnWithoutPool(segment2, spawnPosition, Quaternion.identity);
                    }
                }
            }
            else
            {
                InitializeWithoutPool();
            }
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private void SpawnInitialSegments()
    {
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnNextSegment();
        }
        
        hasInitializedLevel = true;
    }

    private void SpawnNextSegment()
    {
        int randomNumber = Random.Range(0, 2);
        if (randomNumber == 0)
        {
            spawnTag = "Segment_1";
        } 
        else
        {
            spawnTag = "Segment_2";
        }

        Vector3 spawnPosition = new Vector3(0, 0, segmentLength * spawnCount);
        spawnCount++;
        
        GameObject segment = SpawnFromPool(spawnTag, spawnPosition, Quaternion.identity);
        
        activeSegments.Add(segment);
    }

    private void CleanupOldSegments()
    {
        while (activeSegments.Count > maxActiveSegments)
        {
            GameObject oldestSegment = activeSegments[0];
            activeSegments.RemoveAt(0);
            
            oldestSegment.SetActive(false);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        poolDictionary[tag].Enqueue(objectToSpawn);
        
        return objectToSpawn;
    }

    public void SpawnWithoutPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Instantiate(prefab, position, rotation);
    }

    public void InitializeWithoutPool()
    {
        for (int k = 0; k < 4; k++)
        {
            int randomNumber = Random.Range(0, 2);
            if (randomNumber == 0)
            {
                Vector3 spawnPosition = new Vector3(0, 0, segmentLength * spawnCount);
                spawnCount++;
                Instantiate(segment1, spawnPosition, Quaternion.identity);
            }
            else
            {
                Vector3 spawnPosition = new Vector3(0, 0, segmentLength * spawnCount);
                spawnCount++;
                Instantiate(segment2, spawnPosition, Quaternion.identity);
            }
        }
        hasInitializedLevel = true;
    }

    public void PlayAgain()
    {
        isGameOver = false;
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}