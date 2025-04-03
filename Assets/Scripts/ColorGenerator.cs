using UnityEngine;
using System.Collections.Generic;

public class ColorGenerator : MonoBehaviour
{
    // Keep track of colors we've generated that were too dark
    private List<Color> darkColorHistory = new List<Color>();
    private const int MAX_HISTORY_SIZE = 20;
    
    // Brightness threshold below which a color is considered "too dark"
    private const float DARKNESS_THRESHOLD = 0.2f;
    
    // Completely random color with full randomness
    public Color GenerateFullRandomColor()
    {
        Color color = Random.ColorHSV();
        return HandleDarkColors(color);
    }

    // Generate a random color with more controlled parameters
    public Color GenerateControlledRandomColor()
    {
        // Adjusted brightness range to avoid dark colors
        Color color = Random.ColorHSV(
            0f, 1f,       // Hue range (full color wheel)
            0.5f, 1f,     // Saturation range (more saturated colors)
            0.5f, 1f      // Value/Brightness range (no dark colors)
        );
        return HandleDarkColors(color);
    }

    // Generate a color from a specific color palette
    public Color GeneratePaletteColor()
    {
        // Define an array of preset colors
        Color[] colorPalette = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            new Color(1f, 0.5f, 0f),  // Orange
            new Color(0.5f, 0f, 0.5f) // Purple
        };

        // Return a random color from the palette
        return colorPalette[Random.Range(0, colorPalette.Length)];
    }

    // Generate a pastel color
    public Color GeneratePastelColor()
    {
        // Pastel colors are already bright, so they shouldn't appear black
        return Random.ColorHSV(
            0f, 1f,       // Full hue range
            0.1f, 0.3f,   // Low saturation for pastel effect
            0.8f, 1f      // High brightness
        );
    }

    // Generate a color with alpha (transparency)
    public Color GenerateColorWithAlpha()
    {
        // Increased minimum brightness to avoid dark colors
        Color color = Random.ColorHSV(
            0f, 1f,       // Hue
            0.5f, 1f,     // Saturation
            0.5f, 1f,     // Brightness - minimum increased
            0.5f, 1f      // Alpha (transparency)
        );
        return HandleDarkColors(color);
    }

    // Method to get a random color within a specific hue range
    public Color GenerateColorInRange(float minHue, float maxHue)
    {
        // Increased minimum brightness
        Color color = Random.ColorHSV(
            minHue, maxHue,   // Specific hue range
            0.5f, 1f,         // Saturation
            0.5f, 1f          // Brightness - minimum increased
        );
        return HandleDarkColors(color);
    }

    // Generate a complementary color
    public Color GenerateComplementaryColor(Color baseColor)
    {
        // Convert RGB to HSV
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        
        // Shift hue by 180 degrees (complementary color)
        float complementaryHue = (h + 0.5f) % 1f;
        
        // Ensure the brightness is above our threshold
        v = Mathf.Max(v, DARKNESS_THRESHOLD + 0.1f);
        
        Color color = Color.HSVToRGB(complementaryHue, s, v);
        return HandleDarkColors(color);
    }

    // Handle colors that are too dark
    private Color HandleDarkColors(Color color)
    {
        // Convert to HSV to check brightness
        Color.RGBToHSV(color, out float h, out float s, out float v);
        
        // If the color is too dark (low value/brightness)
        if (v < DARKNESS_THRESHOLD)
        {
            // Add to our history of dark colors
            AddToDarkHistory(color);
            
            // Return pure black instead
            return Color.black;
        }
        
        // Check if this color is similar to colors in our dark history
        if (IsSimilarToDarkHistory(color))
        {
            // If similar to previously dark colors, increase brightness
            return Color.HSVToRGB(h, s, Mathf.Lerp(v, 1f, 0.5f));
        }
        
        return color;
    }
    
    // Add a color to our history of dark colors
    private void AddToDarkHistory(Color color)
    {
        darkColorHistory.Add(color);
        
        // Keep the history at a reasonable size
        if (darkColorHistory.Count > MAX_HISTORY_SIZE)
        {
            darkColorHistory.RemoveAt(0);
        }
    }
    
    // Check if a color is similar to colors in our dark history
    private bool IsSimilarToDarkHistory(Color color)
    {
        Color.RGBToHSV(color, out float colorH, out float colorS, out float colorV);
        
        foreach (Color darkColor in darkColorHistory)
        {
            Color.RGBToHSV(darkColor, out float darkH, out float darkS, out float darkV);
            
            // Check if the hue and saturation are similar
            float hueDifference = Mathf.Abs(colorH - darkH);
            if (hueDifference > 0.5f) hueDifference = 1f - hueDifference; // Handle wrapping
            
            if (hueDifference < 0.1f && Mathf.Abs(colorS - darkS) < 0.2f)
            {
                return true;
            }
        }
        
        return false;
    }

    // Example of how to use these methods
    void Start()
    {
        // Demonstration of different color generation methods
        Renderer objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null)
        {
            // Option 1: Completely random color
            objectRenderer.material.color = GenerateFullRandomColor();

            // Option 2: Controlled random color
            // objectRenderer.material.color = GenerateControlledRandomColor();

            // Option 3: Palette color
            // objectRenderer.material.color = GeneratePaletteColor();

            // Option 4: Pastel color
            // objectRenderer.material.color = GeneratePastelColor();

            // Option 5: Color with alpha
            // objectRenderer.material.color = GenerateColorWithAlpha();
        }
    }
}