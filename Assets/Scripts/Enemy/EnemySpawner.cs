using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR Foundation namespace
using UnityEngine.XR.ARSubsystems; // Required for trackables

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    [SerializeField] [Tooltip("The plane manager to pull points from each plane")] public ARPlaneManager planeManager;
    [SerializeField] [Tooltip("The ui script")] public UIScript uiScript;

    [SerializeField] [Tooltip("The enemy prefab that will be spawened")] public List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] [Tooltip("The interval between enemy spawns")] public float spawnInterval;
    [SerializeField] [Tooltip("The minimum distance between enemy spawned prefabs")] public float minDistance;
    // [SerializeField] [Tooltip("The maximum number of enemies spawned ")] public int maxNumEnemies;
    [SerializeField] [Tooltip("The current number of enemies spawned")] public int numEnemies = 0;
    [SerializeField] [Tooltip("The number of enemies to be spawned per wave")] public int enemiesPerWave;
    [SerializeField] [Tooltip("The current wave")] public int waveCount = 1;
    [SerializeField] [Tooltip("The list of current gameobjects in the scene")] private List<GameObject> enemiesSpawned = new List<GameObject>();

    private List<Vector3> spawnedPositions = new List<Vector3>(); //the private list containing all the spawn points from the planes

    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }
    public void StartSpawning() {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies() {
        while(waveCount <= 10) {
            while(numEnemies < enemiesPerWave) {
                yield return new WaitForSeconds(spawnInterval);
                SpawnEnemyAtPoint();
                spawnedPositions.Clear();
            }

            // Wait until the numEnemies is 0
            yield return new WaitUntil(() => numEnemies == 0);
            Debug.Log("Wave " + waveCount + " complete");
            waveCount++; // next wave
            uiScript.updateWaveMessage();
        }
    }

    void SpawnEnemyAtPoint() {
        //for each plane in the plane manager's trackables 
        foreach(var plane in planeManager.trackables) {
            //check if the plane is horizontal 
            if(plane.alignment == PlaneAlignment.HorizontalUp && numEnemies < enemiesPerWave) {
                //get the spawn point on each plane
                if(GetSpawnPointOnPlane(plane, out Vector3 spawnPoint)) {
                    GameObject selectedEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                    //spawn enemy
                    GameObject enemy = Instantiate(selectedEnemy, spawnPoint, Quaternion.identity);
                    enemiesSpawned.Add(enemy);
                    numEnemies++;
                    uiScript.updateEnemyMessage();
                }
            }
        }
    }

    bool GetSpawnPointOnPlane(ARPlane plane, out Vector3 spawnPoint) {
        //attempt to find a point multiple times
        for(int attempt = 0; attempt < 10; attempt++) {
            Vector2 randomPoint = Random.insideUnitCircle * Mathf.Sqrt(plane.size.x * plane.size.y) / 2f;
            Vector3 potentialSpawnPoint = plane.transform.TransformPoint(new Vector3(randomPoint.x, .1f, randomPoint.y));

            //check if the potential spawn point is far enough away from other enemies
            bool isFarEnough = true;
            foreach(var pos in spawnedPositions) {
                if(Vector3.Distance(pos, potentialSpawnPoint) < minDistance) {
                    isFarEnough = false;
                    break;
                }
                
            }

            if(isFarEnough) {
                spawnPoint = potentialSpawnPoint;
                spawnedPositions.Add(spawnPoint); //remeber the initial spawn point of the enemey
                return true;
            }
        }

        spawnPoint = Vector3.zero;
        return false; //could not find a good spawn point
    }   

    public void StopSpawning() {
        StopAllCoroutines();  // This will stop the SpawnEnemies coroutine
    }

    public void RemoveAllEnemies() {
        foreach (GameObject enemy in enemiesSpawned) {
            Destroy(enemy);  // Destroy the enemy GameObject
        }
        enemiesSpawned.Clear();  // Clear the list after all enemies are destroyed
        numEnemies = 0;  // Reset the number of enemies to 0
    } 

    public void ResetSpawner() {
        RemoveAllEnemies();
        waveCount = 1;  // Reset wave count here
        numEnemies = 0;  // Ensure numEnemies is also reset
        spawnedPositions.Clear();  // Optionally clear all recorded spawn positions
    } 
}
