using System.Collections;
using UnityEngine;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    public TextMeshProUGUI introText; // Reference to the TMP text component
    public float displayDuration = 2f; // Duration to show the intro (in seconds)

    private void Start()
    {
        StartCoroutine(DisplayIntro());
    }

    private IEnumerator DisplayIntro()
    {
        introText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        introText.gameObject.SetActive(false);
    }
}
