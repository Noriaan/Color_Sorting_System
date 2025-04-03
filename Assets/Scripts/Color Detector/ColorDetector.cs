using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorDetector : MonoBehaviour
{
    // Dictionary to store named colors and their HSL values
    private Dictionary<string, Vector3> namedColors = new Dictionary<string, Vector3>();
    
    // Dictionary to store VIBGYOR colors and their HSL values
    private Dictionary<string, Vector3> vibgyorColors = new Dictionary<string, Vector3>();
    
    // Dictionary to store grayscale values
    private Dictionary<string, Vector3> grayscaleColors = new Dictionary<string, Vector3>();
    
    // Display the detected color name (for debugging)
    private string detectedColorName = "None";
    private string detectedVibgyorColor = "None";
    private string detectedGrayscaleColor = "None";
    
    // Reference to the rotation controller
    [SerializeField] private ContainerRotationSystem rotationController;

    void Start()
    {
        // Initialize the named colors dictionary with HSL values
        InitializeNamedColors();
        
        // Initialize the VIBGYOR colors
        InitializeVibgyorColors();
        
        // Initialize the grayscale colors
        InitializeGrayscaleColors();
        
        // Make sure rotation controller is assigned
        if (rotationController == null)
        {
            Debug.LogError("ContainerRotationSystem is not assigned to ColorDetector!");
        }
    }

    // Add collision detection methods
    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the "Movable" tag
        if (collision.gameObject.CompareTag("Movable"))
        {
            // Get the color of the colliding object
            DetectObjectColor(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object has the "Movable" tag
        if (other.CompareTag("Movable"))
        {
            // Get the color of the triggering object
            DetectObjectColor(other.gameObject);
        }
    }

    // Method to detect and report object color
    void DetectObjectColor(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Get the main color of the object
            Color objectColor = renderer.material.color;
            
            // Convert RGB to HSL
            Vector3 hslColor = RGBToHSL(objectColor);
            
            // Find the closest named color
            detectedColorName = FindClosestNamedColor(hslColor);
            
            // Find the closest VIBGYOR color
            detectedVibgyorColor = FindClosestVibgyorColor(hslColor);
            
            // Find the closest grayscale color
            detectedGrayscaleColor = FindClosestGrayscaleColor(hslColor);
            
            // Determine if the color is more grayscale or colorful
            bool isMoreGrayscale = IsMoreGrayscaleThanColorful(hslColor);
            string finalColorCategory = isMoreGrayscale ? detectedGrayscaleColor : detectedVibgyorColor;
            
            // Display the detected colors in console
            Debug.Log($"Movable object '{obj.name}' has color: {detectedColorName}, Category: {finalColorCategory} (Saturation: {hslColor.y:F2})");
            
            // Call the rotation controller with the detected color category
            RotateContainerToMatchColor(finalColorCategory);
        }
        else
        {
            Debug.Log($"Movable object '{obj.name}' has no renderer component");
        }
    }
    
    // Check if a color is more grayscale than colorful based on saturation
    bool IsMoreGrayscaleThanColorful(Vector3 hslColor)
    {
        // If saturation is below threshold, consider it grayscale
        float saturationThreshold = 0.15f;
        return hslColor.y < saturationThreshold;
    }
    
    // New method to trigger container rotation based on detected color
    void RotateContainerToMatchColor(string colorCategory)
    {
        if (rotationController != null)
        {
            // Call the rotation controller with the detected color
            rotationController.RotateToColor(colorCategory);
        }
    }

    // Initialize the VIBGYOR colors dictionary with HSL values
    void InitializeVibgyorColors()
    {
        // VIBGYOR colors in HSL (Hue, Saturation, Lightness)
        vibgyorColors.Add("Violet", new Vector3(270f, 1f, 0.5f));
        vibgyorColors.Add("Indigo", new Vector3(275f, 1f, 0.25f));
        vibgyorColors.Add("Blue", new Vector3(240f, 1f, 0.5f));
        vibgyorColors.Add("Green", new Vector3(120f, 1f, 0.5f));
        vibgyorColors.Add("Yellow", new Vector3(60f, 1f, 0.5f));
        vibgyorColors.Add("Orange", new Vector3(30f, 1f, 0.5f));
        vibgyorColors.Add("Red", new Vector3(0f, 1f, 0.5f));
    }
    
    // Initialize the grayscale colors dictionary with HSL values
    void InitializeGrayscaleColors()
    {
        // Grayscale colors in HSL (all with 0 saturation, varying lightness)
        grayscaleColors.Add("Black", new Vector3(0f, 0f, 0.05f));      // Almost black
        grayscaleColors.Add("Dark Gray", new Vector3(0f, 0f, 0.25f));  // Dark gray
        grayscaleColors.Add("Gray", new Vector3(0f, 0f, 0.5f));        // Medium gray
        grayscaleColors.Add("Light Gray", new Vector3(0f, 0f, 0.75f)); // Light gray
        grayscaleColors.Add("White", new Vector3(0f, 0f, 0.95f));      // Almost white
    }

    // Initialize the named colors dictionary with HSL values from the provided list
    void InitializeNamedColors()
    {
        // Basic Colors - HSL values are stored as Vector3(H, S, L) where H is 0-360, S and L are 0-1
        namedColors.Add("Red", new Vector3(0f, 1f, 0.5f));
        namedColors.Add("Orange", new Vector3(30f, 1f, 0.5f));
        namedColors.Add("Yellow", new Vector3(60f, 1f, 0.5f));
        namedColors.Add("Green", new Vector3(120f, 1f, 0.5f));
        namedColors.Add("Cyan", new Vector3(180f, 1f, 0.5f));
        namedColors.Add("Blue", new Vector3(240f, 1f, 0.5f));
        namedColors.Add("Purple", new Vector3(270f, 1f, 0.5f));
        namedColors.Add("Magenta", new Vector3(300f, 1f, 0.5f));
        namedColors.Add("White", new Vector3(0f, 0f, 1f));
        namedColors.Add("Black", new Vector3(0f, 0f, 0f));
        namedColors.Add("Gray", new Vector3(0f, 0f, 0.5f));

        // Extended Colors
        namedColors.Add("Crimson", new Vector3(348f, 0.83f, 0.47f));
        namedColors.Add("Tomato", new Vector3(9f, 1f, 0.64f));
        namedColors.Add("Gold", new Vector3(51f, 1f, 0.5f));
        namedColors.Add("Lime", new Vector3(75f, 1f, 0.5f));
        namedColors.Add("Teal", new Vector3(180f, 1f, 0.25f));
        namedColors.Add("Turquoise", new Vector3(174f, 0.72f, 0.56f));
        namedColors.Add("Royal Blue", new Vector3(225f, 0.73f, 0.57f));
        namedColors.Add("Indigo", new Vector3(275f, 1f, 0.25f));
        namedColors.Add("Violet", new Vector3(282f, 0.76f, 0.53f));
        namedColors.Add("Pink", new Vector3(350f, 1f, 0.88f));
        namedColors.Add("Hot Pink", new Vector3(330f, 1f, 0.71f));
        namedColors.Add("Deep Pink", new Vector3(328f, 1f, 0.54f));
        namedColors.Add("Lavender", new Vector3(240f, 0.67f, 0.85f));
        namedColors.Add("Beige", new Vector3(60f, 0.56f, 0.91f));
        namedColors.Add("Ivory", new Vector3(60f, 1f, 0.97f));
        namedColors.Add("Mint", new Vector3(150f, 0.8f, 0.8f));
        namedColors.Add("Olive", new Vector3(60f, 1f, 0.25f));
        namedColors.Add("Maroon", new Vector3(0f, 1f, 0.25f));
        namedColors.Add("Navy", new Vector3(240f, 1f, 0.25f));
        namedColors.Add("Chocolate", new Vector3(25f, 0.75f, 0.47f));
        namedColors.Add("Salmon", new Vector3(6f, 0.93f, 0.71f));
        namedColors.Add("Coral", new Vector3(16f, 1f, 0.65f));
        namedColors.Add("Peach", new Vector3(28f, 1f, 0.86f));
        namedColors.Add("Periwinkle", new Vector3(231f, 1f, 0.85f));

        // Resene Colors
        namedColors.Add("Pohutukawa", new Vector3(0f, 0.83f, 0.34f));
        namedColors.Add("Saffron", new Vector3(45f, 1f, 0.6f));
        namedColors.Add("Bubbles", new Vector3(186f, 1f, 0.96f));
        namedColors.Add("Deep Koamaru", new Vector3(248f, 0.68f, 0.32f));
        namedColors.Add("Daisy Bush", new Vector3(270f, 0.69f, 0.36f));

        // Crayola Colors
        namedColors.Add("Jazzberry Jam", new Vector3(328f, 0.7f, 0.41f));
        namedColors.Add("Inchworm", new Vector3(96f, 1f, 0.64f));
        namedColors.Add("Midnight Blue", new Vector3(240f, 1f, 0.27f));
        namedColors.Add("Shocking Pink", new Vector3(330f, 1f, 0.66f));
        namedColors.Add("Outrageous Orange", new Vector3(20f, 1f, 0.55f));

        // Pantone Colors
        namedColors.Add("Pantone 186 C", new Vector3(0f, 1f, 0.4f));
        namedColors.Add("Pantone 286 C", new Vector3(230f, 1f, 0.4f));
        namedColors.Add("Pantone 109 C", new Vector3(50f, 1f, 0.5f));
        namedColors.Add("Pantone 347 C", new Vector3(145f, 1f, 0.35f));
        namedColors.Add("Pantone 2587 C", new Vector3(265f, 0.51f, 0.5f));

        // More Named Colors
        namedColors.Add("Alice Blue", new Vector3(208f, 1f, 0.97f));
        namedColors.Add("Antique White", new Vector3(34f, 0.78f, 0.91f));
        namedColors.Add("Aqua", new Vector3(180f, 1f, 0.5f));
        namedColors.Add("Azure", new Vector3(180f, 1f, 0.97f));
        namedColors.Add("Bisque", new Vector3(33f, 1f, 0.88f));
        namedColors.Add("Blanched Almond", new Vector3(36f, 1f, 0.9f));
        namedColors.Add("Burlywood", new Vector3(34f, 0.57f, 0.7f));
        namedColors.Add("Cadet Blue", new Vector3(182f, 0.25f, 0.5f));
        namedColors.Add("Chartreuse", new Vector3(90f, 1f, 0.5f));
        namedColors.Add("Cornflower Blue", new Vector3(219f, 0.79f, 0.66f));
        namedColors.Add("Cornsilk", new Vector3(48f, 1f, 0.93f));
        namedColors.Add("Dark Blue", new Vector3(240f, 1f, 0.27f));
        namedColors.Add("Dark Cyan", new Vector3(180f, 1f, 0.27f));
        namedColors.Add("Dark Goldenrod", new Vector3(43f, 0.89f, 0.38f));
        namedColors.Add("Dark Gray", new Vector3(0f, 0f, 0.41f));
        namedColors.Add("Dark Green", new Vector3(120f, 1f, 0.2f));
        namedColors.Add("Dark Khaki", new Vector3(56f, 0.38f, 0.58f));
        namedColors.Add("Dark Magenta", new Vector3(300f, 1f, 0.27f));
        namedColors.Add("Dark Olive Green", new Vector3(82f, 0.39f, 0.3f));
        namedColors.Add("Dark Orange", new Vector3(33f, 1f, 0.5f));
        namedColors.Add("Dark Orchid", new Vector3(280f, 0.61f, 0.5f));
        namedColors.Add("Dark Red", new Vector3(0f, 1f, 0.27f));
    }

    // Convert RGB to HSL (kept for Unity material colors which are RGB)
    Vector3 RGBToHSL(Color rgbColor)
    {
        float r = rgbColor.r;
        float g = rgbColor.g;
        float b = rgbColor.b;
        
        float max = Mathf.Max(r, Mathf.Max(g, b));
        float min = Mathf.Min(r, Mathf.Min(g, b));
        
        float h = 0;
        float s = 0;
        float l = (max + min) / 2f;
        
        if (max != min)
        {
            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
            
            if (max == r)
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / d + 2f;
            else
                h = (r - g) / d + 4f;
            
            h *= 60f;
        }
        
        return new Vector3(h, s, l);
    }
    
    // Find the closest named color by calculating the distance in HSL space
    string FindClosestNamedColor(Vector3 hslColor)
    {
        string closestColor = "Unknown";
        float minDistance = float.MaxValue;
        
        foreach (var namedColor in namedColors)
        {
            // Calculate distance in HSL space (weighted)
            float hDiff = Mathf.Min(Mathf.Abs(hslColor.x - namedColor.Value.x), 360f - Mathf.Abs(hslColor.x - namedColor.Value.x)) / 180f;
            float sDiff = Mathf.Abs(hslColor.y - namedColor.Value.y);
            float lDiff = Mathf.Abs(hslColor.z - namedColor.Value.z);
            
            // Weight hue more than saturation and lightness
            float distance = hDiff * 2f + sDiff + lDiff;
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = namedColor.Key;
            }
        }
        
        return closestColor;
    }
    
    // Find the closest VIBGYOR color
    string FindClosestVibgyorColor(Vector3 hslColor)
    {
        string closestColor = "Unknown";
        float minDistance = float.MaxValue;
        
        foreach (var vibgyorColor in vibgyorColors)
        {
            // Calculate distance in HSL space (weighted)
            // For VIBGYOR, we mainly care about hue, but we still consider saturation and lightness
            float hDiff = Mathf.Min(Mathf.Abs(hslColor.x - vibgyorColor.Value.x), 360f - Mathf.Abs(hslColor.x - vibgyorColor.Value.x)) / 180f;
            float sDiff = Mathf.Abs(hslColor.y - vibgyorColor.Value.y);
            float lDiff = Mathf.Abs(hslColor.z - vibgyorColor.Value.z);
            
            // Weight hue more than saturation and lightness for VIBGYOR comparison
            float distance = hDiff * 3f + sDiff * 0.5f + lDiff * 0.5f;
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = vibgyorColor.Key;
            }
        }
        
        return closestColor;
    }
    
    // Find the closest grayscale color
    string FindClosestGrayscaleColor(Vector3 hslColor)
    {
        string closestColor = "Unknown";
        float minDistance = float.MaxValue;
        
        foreach (var grayscaleColor in grayscaleColors)
        {
            // For grayscale, we only care about lightness
            float lDiff = Mathf.Abs(hslColor.z - grayscaleColor.Value.z);
            
            if (lDiff < minDistance)
            {
                minDistance = lDiff;
                closestColor = grayscaleColor.Key;
            }
        }
        
        return closestColor;
    }
    
    // Method to test with direct HSL values (can be called from other scripts)
    public string[] TestWithHSL(float h, float s, float l)
    {
        Vector3 hslColor = new Vector3(h, s, l);
        string namedColor = FindClosestNamedColor(hslColor);
        string vibgyorColor = FindClosestVibgyorColor(hslColor);
        string grayscaleColor = FindClosestGrayscaleColor(hslColor);
        
        // Determine if it's more grayscale than colorful
        bool isMoreGrayscale = IsMoreGrayscaleThanColorful(hslColor);
        string finalCategory = isMoreGrayscale ? grayscaleColor : vibgyorColor;
        
        return new string[] { namedColor, finalCategory };
    }
    
    // Method to convert a hex color code to HSL and find the closest colors (can be called from other scripts)
    public string[] TestWithHexColor(string hexColor)
    {
        // Remove # if present
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);
            
        // Parse hex to RGB
        float r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        
        // Convert RGB to HSL
        Vector3 hslColor = RGBToHSL(new Color(r, g, b));
        
        string namedColor = FindClosestNamedColor(hslColor);
        string vibgyorColor = FindClosestVibgyorColor(hslColor);
        string grayscaleColor = FindClosestGrayscaleColor(hslColor);
        
        // Determine if it's more grayscale than colorful
        bool isMoreGrayscale = IsMoreGrayscaleThanColorful(hslColor);
        string finalCategory = isMoreGrayscale ? grayscaleColor : vibgyorColor;
        
        return new string[] { namedColor, finalCategory };
    }
}