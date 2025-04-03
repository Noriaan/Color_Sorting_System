using UnityEngine;

public class PhysicsConveyorBeltWithEdges : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of the conveyor belt")]
    public float speed = 5.0f;
    
    [Tooltip("Direction multiplier (1 = forward, -1 = backward)")]
    public float directionMultiplier = 1.0f;
    
    [Header("Visual Settings")]
    [Tooltip("Should the conveyor material animate?")]
    public bool animateMaterial = true;
    
    [Tooltip("Material scroll speed multiplier")]
    public float materialSpeedMultiplier = 1.0f;
    
    [Header("Edge Detection")]
    [Tooltip("How far from the end objects should start falling off")]
    public float edgeDetectionDistance = 0.2f;
    
    [Tooltip("Should objects be pushed slightly toward the edge when they reach it?")]
    public bool pushOffEdge = true;
    
    [Tooltip("Force to apply when pushing objects off the edge")]
    public float edgePushForce = 1.0f;
    
    private Material conveyorMaterial;
    private Collider conveyorCollider;
    private PhysicMaterial zeroFriction;
    private float conveyorLength;
    
    void Start()
    {
        // Get references
        conveyorCollider = GetComponent<Collider>();
        if (conveyorCollider == null)
        {
            conveyorCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Create a zero friction material
        zeroFriction = new PhysicMaterial("ZeroFriction");
        zeroFriction.dynamicFriction = 0f;
        zeroFriction.staticFriction = 0f;
        zeroFriction.frictionCombine = PhysicMaterialCombine.Minimum;
        conveyorCollider.material = zeroFriction;
        
        // Set up material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && animateMaterial)
        {
            conveyorMaterial = renderer.material;
        }
        
        // Calculate the length of the conveyor in the forward direction
        conveyorLength = transform.localScale.z;
    }
    
    void Update()
    {
        // Animate material
        if (animateMaterial && conveyorMaterial != null)
        {
            Vector2 offset = conveyorMaterial.mainTextureOffset;
            offset.y += speed * materialSpeedMultiplier * directionMultiplier * Time.deltaTime;
            conveyorMaterial.mainTextureOffset = offset;
        }
    }
    
    void FixedUpdate()
    {
        // Calculate movement direction based on conveyor's forward direction
        Vector3 moveDirection = transform.forward * directionMultiplier;
        Vector3 targetVelocity = new Vector3(moveDirection.x * speed, 0, moveDirection.z * speed);
        
        // Find all objects in contact with the conveyor
        Collider[] colliders = Physics.OverlapBox(
            transform.position,
            transform.localScale / 2,
            transform.rotation
        );
        
        foreach (var col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null || rb.isKinematic) continue;
            
            // Check if the object is above the conveyor (y position)
            if (col.bounds.min.y >= transform.position.y)
            {
                // Calculate the position of the object relative to the conveyor
                Vector3 localPos = transform.InverseTransformPoint(col.transform.position);
                
                // Check if object is near the end of the conveyor
                bool nearEnd = (directionMultiplier > 0 && localPos.z > (conveyorLength/2) - edgeDetectionDistance) || 
                               (directionMultiplier < 0 && localPos.z < -(conveyorLength/2) + edgeDetectionDistance);
                
                if (nearEnd)
                {
                    // Object is near the edge, let it fall off naturally
                    if (pushOffEdge)
                    {
                        // Give it a slight push in the forward direction
                        rb.AddForce(moveDirection * edgePushForce, ForceMode.Impulse);
                    }
                    
                    // Don't apply conveyor force anymore
                    continue;
                }
                
                // Get the current horizontal velocity
                Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                
                // Calculate how different the current velocity is from our target
                Vector3 velocityChange = targetVelocity - horizontalVel;
                
                // Apply a force to correct the velocity, but preserve vertical movement
                rb.AddForce(velocityChange, ForceMode.VelocityChange);
                
                // Apply slight downward force to keep objects on the belt
                rb.AddForce(Vector3.down * 3.0f, ForceMode.Acceleration);
            }
        }
    }
    
    // Collision-based movement as a backup method
    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null || rb.isKinematic) return;
        
        // Calculate the position of the object relative to the conveyor
        Vector3 localPos = transform.InverseTransformPoint(collision.transform.position);
        
        // Check if object is near the end of the conveyor
        bool nearEnd = (directionMultiplier > 0 && localPos.z > (conveyorLength/2) - edgeDetectionDistance) || 
                       (directionMultiplier < 0 && localPos.z < -(conveyorLength/2) + edgeDetectionDistance);
        
        if (nearEnd)
        {
            // Object is near the edge, let it fall off naturally
            if (pushOffEdge)
            {
                // Calculate movement direction
                Vector3 moveDirections = transform.forward * directionMultiplier;
                
                // Give it a slight push in the forward direction
                rb.AddForce(moveDirections * edgePushForce, ForceMode.Impulse);
            }
            
            // Don't apply conveyor force anymore
            return;
        }
        
        // Calculate movement direction and target velocity
        Vector3 moveDirection = transform.forward * directionMultiplier;
        Vector3 targetVelocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
        
        // Check if the object is on top of the conveyor
        foreach (ContactPoint contact in collision.contacts)
        {
            // If contact normal is pointing roughly upward, object is on top
            if (Vector3.Dot(contact.normal, transform.up) > 0.5f)
            {
                // Set the velocity directly, preserving vertical component
                rb.velocity = targetVelocity;
                
                // Apply slight downward force to keep objects on the belt
                rb.AddForce(Vector3.down * 3.0f, ForceMode.Acceleration);
                
                break;
            }
        }
    }
    
    // Visual debugging
    void OnDrawGizmosSelected()
    {
        // Draw direction arrow
        Gizmos.color = Color.blue;
        Vector3 start = transform.position;
        Vector3 direction = transform.forward * directionMultiplier;
        Vector3 end = start + direction * 2f;
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(end, end - direction * 0.5f + transform.right * 0.5f);
        Gizmos.DrawLine(end, end - direction * 0.5f - transform.right * 0.5f);
        
        // Draw the conveyor bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // Draw the edge detection zones
        Gizmos.color = Color.red;
        
        // Forward edge
        Vector3 frontPos = transform.position + transform.forward * (transform.localScale.z/2 - edgeDetectionDistance/2);
        Vector3 frontSize = new Vector3(transform.localScale.x, transform.localScale.y, edgeDetectionDistance);
        Gizmos.DrawWireCube(frontPos, frontSize);
        
        // Back edge
        Vector3 backPos = transform.position - transform.forward * (transform.localScale.z/2 - edgeDetectionDistance/2);
        Vector3 backSize = new Vector3(transform.localScale.x, transform.localScale.y, edgeDetectionDistance);
        Gizmos.DrawWireCube(backPos, backSize);
    }
    
    void OnDestroy()
    {
        // Clean up created materials to prevent memory leaks
        if (conveyorMaterial != null)
        {
            Destroy(conveyorMaterial);
        }
        
        if (zeroFriction != null)
        {
            Destroy(zeroFriction);
        }
    }
}