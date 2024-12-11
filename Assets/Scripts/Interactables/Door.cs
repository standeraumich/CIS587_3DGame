using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    private bool doorOpen;
    //this function is where we will design our interaction using code
    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        doorOpen = !doorOpen;
        gameObject.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}
