using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        if(instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
