using System.Collections;
using UnityEngine;

public class AmbientColorChanger : MonoBehaviour
{
    // Duration for the color transition
    public float transitionDuration = 5f;

    private void Start()
    {
        // Remove Skybox Material 
        RenderSettings.skybox = null; 
        // Remove Sun source 
        RenderSettings.sun = null;
        // Start the coroutine to change the ambient color
        StartCoroutine(ChangeAmbientColor(Color.white, Color.black, transitionDuration));
    }

    private IEnumerator ChangeAmbientColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate the ambient color based on elapsed time
            RenderSettings.ambientLight = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set to the target color
        RenderSettings.ambientLight = endColor;
    }
}
