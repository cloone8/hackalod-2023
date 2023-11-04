using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDoor : Interactable
{
    private Room destination;

    void Start() {
        if(destination.scene == null) {
            destination.scene = RoomManager.Instance().GetRandomRoom();
        }
    }

    public void SetDestination(Room room) {
        destination = room;
    }

    public override void Interact() {
        Debug.Log("Entering door to " + destination.scene + " with painter " + destination.painter);
        RoomManager.Instance().EnterHallway(destination);
    }
}
