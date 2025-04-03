using UnityEngine;

public class AdvancedSpawner : MonoBehaviour
{
    // Prefab to spawn
    public GameObject prefabToSpawn;

    // Number of objects to spawn per interval
    public int numberOfObjects = 5;

    // Spawn area
    public float spawnRadiusX = 5f;
    public float spawnRadiusZ = 5f;

    // Spawn interval in seconds
    public float spawnInterval = 3f;

    // Timer to track spawn intervals
    private float spawnTimer;

    public bool spawn;

    void Start()
    {
        SpawnObjects();
    }
    void Update()
    {
        // Increment timer
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn
        if (spawn)
        {
            if(spawnTimer >= spawnInterval)
            {
                // Spawn objects
                SpawnObjects();

                // Reset the timer
                spawnTimer = 0f;
            }
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generate random spawn position
            Vector3 spawnPosition = new Vector3(
                transform.position.x + Random.Range(-spawnRadiusX, spawnRadiusX),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRadiusZ, spawnRadiusZ)
            );

            // Spawn the object
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    // Visualize spawn area in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRadiusX * 2, 0.1f, spawnRadiusZ * 2));
    }
}