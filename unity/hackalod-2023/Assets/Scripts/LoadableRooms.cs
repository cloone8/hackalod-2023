using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Data", menuName = "Room Config/Loadable Rooms", order = 1)]
public class LoadableRooms : ScriptableObject
{
    public string transitionRoom;

    public List<string> roomNames;
}
