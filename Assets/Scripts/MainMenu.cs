using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public TMP_Dropdown mazeSizeDropdown;
    // Start is called before the first frame update
    void Start()
    {
         // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        difficultyDropdown.value = PlayerPrefs.GetInt("Difficulty",0);
        mazeSizeDropdown.value = PlayerPrefs.GetInt("Size", 0);

        difficultyDropdown.onValueChanged.AddListener(delegate 
        {
            DropdownValueChanged(difficultyDropdown, "Difficulty");
        });

        mazeSizeDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(mazeSizeDropdown, "Size");
        });
    }
    void DropdownValueChanged(TMP_Dropdown change, string key)
    {
        // Save the selected difficulty
        PlayerPrefs.SetInt(key, change.value);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit(); // This will exit the game when built
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // This will stop play mode in the editor
        #endif

    }
}
