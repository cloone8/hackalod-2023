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

    private Room prevRoom = new("Entrance", null);

    private string nextPainterId = null;


    public string GetRandomRoom() {
        return rooms.roomNames[UnityEngine.Random.Range(0, rooms.roomNames.Count)];
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

        prevRoom = LDM != null ? new Room(curScene, LDM.GetCurrentPainterId()) : new Room(curScene, null);

        nextPainterId = destination.painter;

        loadOp.allowSceneActivation = true;

        loadOp = SceneManager.LoadSceneAsync(destination.scene, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
    }

    private IEnumerator AsyncHandler(AsyncOperation operation, string sceneName = null) {
        string scene = sceneName ?? "unknown";

        Debug.Log("Starting async operation " + scene);

        while(!operation.isDone) {
            yield return null;
        }

        Debug.Log("Async operation " + scene + " completed!");
    }

    public void InitFirstRoom() {
        string firstRoom = GetRandomRoom();

        Debug.Log("Loading first room " + firstRoom);

        loadOp = SceneManager.LoadSceneAsync(firstRoom, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;

        StartCoroutine(AsyncHandler(loadOp, firstRoom));
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

        List<RoomDoor> exits = GameObject.FindGameObjectsWithTag("Exit").ToList().ConvertAll(door => door.GetComponent<RoomDoor>());
        // TODO: Get new edges and set up doors

        DataManager LDM = FindCurrentDataManager();

        if(LDM != null) {
            Debug.Log("Setting up LDM");

            StartCoroutine(LDM.SetCurrentPainter(nextPainterId, (links) => {
                Debug.Log("Got links of artist");

                for(int i = 0; i < Math.Min(links.Count, exits.Count); i++) {
                    Debug.Log("Setting up door " + i);

                    int roomNum = Math.Clamp(links[i].numLinks, rooms.minDoors, rooms.maxDoors);

                    exits[i].SetPrompt(links[i].label);
                    exits[i].SetDestination(new Room("Room" + i, links[i].id));
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
