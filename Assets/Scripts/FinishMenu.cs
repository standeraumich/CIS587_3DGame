using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishMenu : MonoBehaviour
{
    void Start()
    {
         // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        print("QUIT");
        Application.Quit();
    }
}
