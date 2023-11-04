using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    private string destination = "Narnia";

    private string scene = "Room5";

    public void SetDestination(string sceneName, string prompt) {
        destination = prompt;
        scene = sceneName;
    }

    public override void Interact() {
        Debug.Log("Going to " + destination + "!");
    }
}
