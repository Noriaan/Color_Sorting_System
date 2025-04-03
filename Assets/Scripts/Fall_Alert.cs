using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Alert : MonoBehaviour
{
    [SerializeField]private PhysicsConveyorBelt physicsConveyorBelt;
    [SerializeField]private AdvancedSpawner advancedSpawner;
    public string tag_Name;
    public float restartDelay = 2.0f; // Delay in seconds before restarting the conveyor
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(tag_Name))
        {
            Debug.Log("Fall");
            physicsConveyorBelt.speed = 0;
            advancedSpawner.spawn = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(tag_Name))
        {
            // Start the coroutine to delay the speed reset
            StartCoroutine(RestartConveyorAfterDelay());
        }
    }
    
    // Coroutine to handle the delay
    private IEnumerator RestartConveyorAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(restartDelay);
        
        // After waiting, set the speed back to 4
        physicsConveyorBelt.speed = 4;
        advancedSpawner.spawn = true;
    }
}