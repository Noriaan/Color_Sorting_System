using UnityEngine;
using System.Collections.Generic;

public class Rotating_conveyor : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField] private float speed = 2.0f; // Speed of the conveyor belt
    [SerializeField] private Vector3 direction = Vector3.forward; // Direction of movement
    [SerializeField] private bool isActive = true; // Whether the conveyor is running
    
    [Header("Visual Settings")]
    [SerializeField] private Material conveyorMaterial; // Material with scrolling texture
    [SerializeField] private string textureOffsetProperty = "_MainTex"; // Property name for texture offset
    
    // List to track objects on the conveyor
    private List<Rigidbody> objectsOnConveyor = new List<Rigidbody>();
    
    // Cached transform for efficiency
    private Transform cachedTransform;
    private float textureOffset = 0f;
    
    private void Awake()
    {
        cachedTransform = transform;
    }
    
    private void Update()
    {
        if (isActive && conveyorMaterial != null)
        {
            // Scroll the conveyor texture based on speed
            textureOffset += speed * Time.deltaTime;
            if (textureOffset > 1f)
                textureOffset -= 1f;
                
            // Update material offset
            conveyorMaterial.SetTextureOffset(textureOffsetProperty, new Vector2(0, textureOffset));
        }
    }
    
    private void FixedUpdate()
    {
        if (!isActive) return;
        
        // Apply force to all objects on the conveyor
        foreach (Rigidbody rb in objectsOnConveyor)
        {
            if (rb != null)
            {
                // Convert direction from local to world space
                Vector3 worldDirection = cachedTransform.TransformDirection(direction.normalized);
                
                // Apply velocity to move the object
                Vector3 targetVelocity = worldDirection * speed;
                
                // Calculate velocity difference and apply it as a force
                Vector3 velocityDifference = targetVelocity - rb.velocity;
                rb.AddForce(velocityDifference, ForceMode.VelocityChange);
                
                // Optional: Rotate objects to face the direction of movement
                if (rb.gameObject.CompareTag("ConveyorObject"))
                {
                    Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
                    rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
                }
            }
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // Check if the object has a rigidbody
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        
        if (rb != null && !objectsOnConveyor.Contains(rb))
        {
            // Add the object to the list of objects on the conveyor
            objectsOnConveyor.Add(rb);
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        // Remove the object from the list when it leaves the conveyor
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        
        if (rb != null && objectsOnConveyor.Contains(rb))
        {
            objectsOnConveyor.Remove(rb);
        }
    }
    
    // Public methods to control the conveyor
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    public void ToggleConveyor(bool active)
    {
        isActive = active;
    }
}