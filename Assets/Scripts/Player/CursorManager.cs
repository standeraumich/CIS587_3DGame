using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    // Variable to keep track of the cursor state
    private bool isCursorLocked = true;

    void Start()
    {
        // Lock and hide the cursor at the start
        LockCursor();
    }

    void Update()
    {
        // Press Escape to unlock and show the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
            SceneManager.LoadScene(0);
        }

    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }
}
