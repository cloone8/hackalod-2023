using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EntranceManager : MonoBehaviour
{
    public Room startingRoom;
    void Start() {
        RoomManager.Instance().InitFirstRoom(startingRoom);
    }
}
