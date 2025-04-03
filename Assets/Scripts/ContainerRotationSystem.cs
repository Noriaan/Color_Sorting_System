using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerRotationSystem : MonoBehaviour
{
    [Header("Container Settings")]
    [SerializeField] private Transform containerAssembly; // The parent object of all containers
    [SerializeField] private float rotationSpeed = 45f; // Degrees per second
    [SerializeField] private float positioningAccuracy = 0.5f; // How close to target angle is considered "aligned"
    
    [Header("VIBGYOR Color Positions")]
    [SerializeField] private float violetAngle = 0f;
    [SerializeField] private float indigoAngle = 45f;
    [SerializeField] private float blueAngle = 90f;
    [SerializeField] private float greenAngle = 135f;
    [SerializeField] private float yellowAngle = 180f;
    [SerializeField] private float orangeAngle = 225f;
    [SerializeField] private float redAngle = 270f;
    [SerializeField] private float grayAngle = 315f; // For non-VIBGYOR colors
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // Dictionary to map alternative color names to standard colors
    private Dictionary<string, string> colorNameMapping;
    private float targetAngle;
    private bool isRotating = false;
    private string lastDetectedColor = "";
    
    void Start()
    {
        // Make sure container assembly is assigned
        if (containerAssembly == null)
        {
            Debug.LogError("Container Assembly is not assigned in ContainerRotationSystem!");
            enabled = false; // Disable this script if the container assembly is not assigned
            return;
        }
        
        // Initialize color mapping dictionary for common color variants
        InitializeColorMapping();
        
        if (enableDebugLogs)
        {
            Debug.Log("ContainerRotationSystem initialized with the following color angles:");
            Debug.Log($"Violet: {violetAngle}°, Indigo: {indigoAngle}°, Blue: {blueAngle}°");
            Debug.Log($"Green: {greenAngle}°, Yellow: {yellowAngle}°, Orange: {orangeAngle}°");
            Debug.Log($"Red: {redAngle}°, Gray: {grayAngle}°");
        }
    }
    
    // Initialize color mapping for common variants
    private void InitializeColorMapping()
    {
        colorNameMapping = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
        {
            // Standard VIBGYOR colors
            {"violet", "violet"},
            {"indigo", "indigo"},
            {"blue", "blue"},
            {"green", "green"},
            {"yellow", "yellow"},
            {"orange", "orange"},
            {"red", "red"},
            
            // Common variants
            {"purple", "violet"},
            {"dark blue", "indigo"},
            {"light blue", "blue"},
            {"lime", "green"},
            {"gold", "yellow"},
            {"amber", "orange"},
            {"crimson", "red"},
            
            // RGB values (if your detector outputs these)
            {"(128, 0, 128)", "violet"},
            {"(75, 0, 130)", "indigo"},
            {"(0, 0, 255)", "blue"},
            {"(0, 255, 0)", "green"},
            {"(255, 255, 0)", "yellow"},
            {"(255, 165, 0)", "orange"},
            {"(255, 0, 0)", "red"},
            
            // Default case for unrecognized colors
            {"unknown", "gray"},
            {"gray", "gray"},
            {"grey", "gray"}
        };
    }
    
    // This method is called from the ColorDetector script
    public void RotateToColor(string colorName)
    {
        if (string.IsNullOrEmpty(colorName))
        {
            Debug.LogWarning("Received empty color name!");
            return;
        }
        
        lastDetectedColor = colorName.ToLower();
        
        if (enableDebugLogs)
        {
            Debug.Log($"Received color: \"{colorName}\"");
        }
        
        // Map to standard color if variant is detected
        string standardColor = MapToStandardColor(lastDetectedColor);
        
        if (enableDebugLogs && lastDetectedColor != standardColor)
        {
            Debug.Log($"Mapped \"{lastDetectedColor}\" to standard color: \"{standardColor}\"");
        }
        
        // Set target angle based on detected color
        switch (standardColor)
        {
            case "violet":
                targetAngle = violetAngle;
                break;
            case "indigo":
                targetAngle = indigoAngle;
                break;
            case "blue":
                targetAngle = blueAngle;
                break;
            case "green":
                targetAngle = greenAngle;
                break;
            case "yellow":
                targetAngle = yellowAngle;
                break;
            case "orange":
                targetAngle = orangeAngle;
                break;
            case "red":
                targetAngle = redAngle;
                break;
            default:
                // For any color not in VIBGYOR, use gray or another default
                targetAngle = grayAngle;
                Debug.Log($"Using default container (gray) for unrecognized color: \"{colorName}\"");
                break;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"Rotating container to {standardColor} at angle: {targetAngle}°");
        }
        
        // Start rotation
        isRotating = true;
    }
    
    // Map detected color to standard color
    private string MapToStandardColor(string detectedColor)
    {
        if (colorNameMapping.TryGetValue(detectedColor, out string standardColor))
        {
            return standardColor;
        }
        
        // Check for partial matches (e.g., "light red" or "dark green")
        foreach (var key in colorNameMapping.Keys)
        {
            if (detectedColor.Contains(key))
            {
                return colorNameMapping[key];
            }
        }
        
        // If no match found
        return "gray";
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            // Get current y rotation
            float currentAngle = containerAssembly.rotation.eulerAngles.y;
            
            // Calculate the rotation needed in clockwise direction
            float clockwiseDistance;
            
            if (currentAngle <= targetAngle)
            {
                clockwiseDistance = targetAngle - currentAngle;
            }
            else
            {
                clockwiseDistance = 360f - currentAngle + targetAngle;
            }
            
            // Check if we've reached the target (with accuracy threshold)
            if (clockwiseDistance < positioningAccuracy || clockwiseDistance > (360f - positioningAccuracy))
            {
                // Set exact angle and stop rotating
                containerAssembly.rotation = Quaternion.Euler(containerAssembly.rotation.eulerAngles.x, targetAngle, containerAssembly.rotation.eulerAngles.z);
                isRotating = false;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"Container rotation completed. Position: {targetAngle}° for color: {lastDetectedColor}");
                }
                return;
            }
            
            // Apply clockwise rotation (negative Y rotation)
            float rotationAmount = rotationSpeed * Time.deltaTime;
            
            // Make sure we don't overshoot
            if (rotationAmount > clockwiseDistance)
            {
                rotationAmount = clockwiseDistance;
            }
            
            // Apply rotation in clockwise direction
            containerAssembly.Rotate(0, -rotationAmount, 0); // Negative Y rotation is clockwise
        }
    }
    
    // Optional: Public method to manually set container position (for testing)
    public void SetContainerPosition(float angle)
    {
        containerAssembly.rotation = Quaternion.Euler(containerAssembly.rotation.eulerAngles.x, angle, containerAssembly.rotation.eulerAngles.z);
    }
    
    // Optional: Public method to rotate to a specific container index
    public void RotateToContainerIndex(int index)
    {
        // Assuming 8 containers (0-7) based on the VIBGYOR + gray
        float anglePerContainer = 360f / 8;
        targetAngle = index * anglePerContainer;
        isRotating = true;
    }
    
    // For debugging: Show last detected color
    public string GetLastDetectedColor()
    {
        return lastDetectedColor;
    }
}