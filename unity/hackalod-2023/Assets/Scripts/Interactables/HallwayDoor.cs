using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HallwayDoor : Interactable
{
    public string prompt = "Narnia";

    public override void Interact()
    {
        Debug.Log("Going to " + prompt + "!");
        RoomManager.Instance().EnterRoom();
    }
}
