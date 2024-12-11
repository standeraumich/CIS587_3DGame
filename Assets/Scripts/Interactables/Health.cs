using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Interactable
{
    private GameObject player;
    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Find the player object at runtime
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(30); // Restore health by 10 (or any other amount)
            }
        }
        else
        {
            Debug.LogWarning("Player object not found!");
        }

        gameObject.SetActive(false);
    }
}
