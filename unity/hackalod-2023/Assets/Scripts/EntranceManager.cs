using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EntranceManager : MonoBehaviour
{
    public Room startingRoom;
    void Start()
    {
        Debug.Log("EntranceManager.Start()");
        Debug.Log(startingRoom.scene);
        Debug.Log(startingRoom.painter);
        Debug.Log(startingRoom.prompt);
        RoomManager.Instance().InitFirstRoom(startingRoom);
    }
}
