using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : MonoBehaviour
{
    public Light flash;
    public float duration = 5f; // Duration of the flash effect
    public float flashIntensity = 20f;

    private void Start()
    {
        flash.intensity = 0f;
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        flash.enabled = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            flash.intensity = Mathf.Lerp(flashIntensity, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flash.intensity = 0f;
        flash.enabled = false;
    }
}
