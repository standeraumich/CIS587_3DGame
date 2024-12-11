using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : MonoBehaviour
{
    public GameObject flashPrefab;
    public float duration = 5f; // Duration of the flash effect
    public float flashIntensity = 20f;

    public void TriggerFlash()
    {
        // Instantiate the flash prefab at the current position and rotation
        GameObject flashInstance = Instantiate(flashPrefab, transform.position, transform.rotation);
        Light flash = flashInstance.GetComponent<Light>();
        flash.enabled = true;

        if (flash != null)
        {
            StartCoroutine(FlashEffect(flash));
        }
        else
        {
            Debug.LogError("No Light component found on the flashPrefab!");
        }
    }

    private IEnumerator FlashEffect(Light flash)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            flash.intensity = Mathf.Lerp(flashIntensity, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flash.intensity = 0f;
        flash.enabled = false;

        // Optionally, destroy the flash instance after it fades out
        Destroy(flash.gameObject);
    }
}
