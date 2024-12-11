using UnityEngine;

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
        }

        // Optionally, press another key (e.g., F) to lock and hide the cursor again
        if (Input.GetKeyDown(KeyCode.F))
        {
            LockCursor();
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
