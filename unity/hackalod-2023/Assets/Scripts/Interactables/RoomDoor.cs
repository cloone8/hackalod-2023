using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDoor : Interactable
{
    private Room destination;

    public TextMeshProUGUI titleMesh;

    void Start() {
        if(destination.scene == null) {
            destination.scene = RoomManager.Instance().GetRandomRoom();
        }
    }

    public void SetDestination(Room room) {
        destination = room;
        if(titleMesh != null) {
            titleMesh.SetText(room.prompt);
        }
    }

    public override void Interact() {
        Debug.Log("Entering door to " + destination.scene + " with painter " + destination.painter);
        RoomManager.Instance().EnterHallway(destination);
    }
}
