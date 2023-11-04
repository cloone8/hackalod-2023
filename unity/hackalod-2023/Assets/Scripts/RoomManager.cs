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

    private string prevRoom = "Entrance";

    public string GetRandomRoom() {
        return rooms.roomNames[Random.Range(0, rooms.roomNames.Count)];
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

    public void EnterHallway(string destinationRoomName) {
        Debug.Log("Entering hallway to " + destinationRoomName);

        if(loadOp == null) {
            Debug.LogError("No load operation to activate");
            return;
        }

        prevRoom = SceneManager.GetActiveScene().name;

        loadOp.allowSceneActivation = true;

        loadOp = SceneManager.LoadSceneAsync(destinationRoomName, LoadSceneMode.Single);
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("Scene loaded callback: " + scene.name);

        if(scene.name == rooms.transitionRoom || scene.name == rooms.entranceRoom) {
            Debug.Log("Transition or entrance room loaded, not doing anything");
            return;
        }

        GameObject entranceObject = GameObject.FindGameObjectWithTag("Entrance");
        RoomDoor entrance = entranceObject != null ? entranceObject.GetComponent<RoomDoor>() : null;

        if(prevRoom == rooms.entranceRoom) {
            Debug.Log("Previous door was the entrance, deleting door");
            Destroy(entrance.gameObject);
        } else {
            Debug.Log("Previous door was not the entrance, setting up door");
            entrance.SetDestination(prevRoom, "Back");
        }

        List<RoomDoor> exits = GameObject.FindGameObjectsWithTag("Exit").ToList().ConvertAll(door => door.GetComponent<RoomDoor>());
        // TODO: Get new edges and set up doors
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
