using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EntranceManager : MonoBehaviour
{
   void Start() {
    RoomManager.Instance().InitFirstRoom();
   }
}
