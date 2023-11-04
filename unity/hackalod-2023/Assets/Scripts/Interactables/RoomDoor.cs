using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDoor : Interactable
{
    private string destination = "Unknown";

    private string scene = "";

    void Start() {
        scene = RoomManager.Instance().GetRandomRoom();
    }

    public void SetDestination(string sceneName, string prompt) {
        destination = prompt;
        scene = sceneName;
    }

    public override void Interact() {
        Debug.Log("Going to " + destination + "!");
        RoomManager.Instance().EnterHallway(scene);
    }
}
