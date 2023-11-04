using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    private static RoomManager instance;

    public static RoomManager Instance() {
        return instance;
    }

    public LoadableRooms rooms;

    private AsyncOperation loadOp;

    private Room prevRoom = new("Entrance", null, "");

    private string nextPainterId = null;

    public string GetRandomRoom() {
        return rooms.roomNames[UnityEngine.Random.Range(0, rooms.roomNames.Count)];
    }

    public int GetMaxDoors() {
        return rooms.maxDoors;
    }

    public int GetMinDoors() {
        return rooms.minDoors;
    }

    public void EnterRoom() {
        Debug.Log("Entering room");

        if(loadOp == null) {
            Debug.LogError("No load operation to activate");
            return;
        }

        loadOp.allowSceneActivation = true;

        loadOp = SceneManager.LoadSceneAsync(rooms.transitionRoom, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
    }

    public void EnterHallway(Room destination) {
        Debug.Log("Entering hallway to " + destination.scene + " with painter " + destination.painter);

        if(loadOp == null) {
            Debug.LogError("No load operation to activate");
            return;
        }

        string curScene = SceneManager.GetActiveScene().name;

        DataManager LDM = FindCurrentDataManager();

        prevRoom = LDM != null ? new Room(curScene, LDM.GetCurrentPainterId(), "Go Back") : new Room(curScene, null, "Go Back");

        nextPainterId = destination.painter;

        loadOp.allowSceneActivation = true;

        loadOp = SceneManager.LoadSceneAsync(destination.scene, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
    }

    private IEnumerator AsyncHandler(AsyncOperation operation, string sceneName = null) {
        while(!operation.isDone) {
            yield return null;
        }
    }

    public void InitFirstRoom(Room firstRoom) {
        Debug.Log("Loading first room " + firstRoom);

        loadOp = SceneManager.LoadSceneAsync(firstRoom.scene, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
        nextPainterId = firstRoom.painter;

        Debug.Log("init next painter set to " + nextPainterId);

        StartCoroutine(AsyncHandler(loadOp, firstRoom.painter));
    }

    private DataManager FindCurrentDataManager() {
        GameObject LDMObject = GameObject.FindGameObjectWithTag("Linked Data Manager");
        return LDMObject != null ? LDMObject.GetComponent<DataManager>() : null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("Scene loaded callback: " + scene.name);

        if(scene.name == rooms.transitionRoom || scene.name == rooms.entranceRoom) {
            Debug.Log("Transition or entrance room loaded, not doing anything");
            return;
        }

        GameObject entranceObject = GameObject.FindGameObjectWithTag("Entrance");
        RoomDoor entrance = entranceObject != null ? entranceObject.GetComponent<RoomDoor>() : null;

        if(prevRoom.scene == rooms.entranceRoom) {
            Debug.Log("Previous door was the entrance, deleting door");
            Destroy(entrance.gameObject);
        } else {
            Debug.Log("Previous door was not the entrance, setting up door");
            entrance.SetDestination(prevRoom);
        }

        List<RoomDoor> exits = GameObject.FindGameObjectsWithTag("Exit").ToList().ConvertAll(door => {
            Debug.Log("Component: " + door.name);
            return door.GetComponent<RoomDoor>();
        });

        DataManager LDM = FindCurrentDataManager();

        if(LDM != null) {
            Debug.Log("Setting up LDM");

            StartCoroutine(LDM.SetCurrentPainter(nextPainterId, (links) => {
                Debug.Log("Got " + links.Count + " links of artist");

                for(int i = 0; i < Math.Min(links.Count, exits.Count); i++) {
                    Debug.Log("Setting up door " + i);
                    DoorLink link = links[i];
                    RoomDoor exitDoor = exits[i];

                    Debug.Log("Setting up " + link.id + " for door " + exitDoor.name);

                    int roomNum = Math.Clamp(link.numLinks, rooms.minDoors, rooms.maxDoors);

                    exitDoor.SetDestination(new Room("Room" + roomNum, link.id, link.label));
                }
            }));
        } else {
            Debug.Log("No LDM found");
        }
    }

    void Awake()
    {
        if(instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }
}
