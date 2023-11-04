using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDoor : Interactable
{
    private Room destination;
    private string prompt = "";
    private TextMeshProUGUI titleMesh;

    void Start() {
        if(destination.scene == null) {
            destination.scene = RoomManager.Instance().GetRandomRoom();
        }

        titleMesh = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetDestination(Room room) {
        destination = room;
    }

    public void SetPrompt(string prompt) {
        this.prompt = prompt;

        titleMesh.SetText(prompt);
    }

    public override void Interact() {
        Debug.Log("Entering door to " + destination.scene + " with painter " + destination.painter);
        RoomManager.Instance().EnterHallway(destination);
    }
}
