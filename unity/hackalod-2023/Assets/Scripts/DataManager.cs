
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    private static DataManager instance = null;

    public HashSet<string> painterQueue;
    public HashSet<string> imageQueue;

    public Dictionary<string, List<Artwork>> painters;
    public Dictionary<string, Texture2D> imageCache;

    private string currentPainter;
    private int currentArtworkIndex;

    private DataManager()
    {
        painters = new Dictionary<string, List<Artwork>>();
        imageCache = new Dictionary<string, Texture2D>();
        painterQueue = new HashSet<string>();
        imageQueue = new HashSet<string>();
        currentArtworkIndex = -1;
    }

    public static DataManager Instance()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject("DataManager");
            instance = gameObject.AddComponent<DataManager>();
        }

        return instance;
    }

    public string GetNextImageUrl()
    {
        currentArtworkIndex++;

        List<Artwork> artworks = painters[currentPainter];

        return artworks[currentArtworkIndex %  artworks.Count].contentUrl;
    }

    public IEnumerator FetchImage(string url)
    {
        if (imageCache.ContainsKey(url))
        {
            Debug.Log("Image already present in cache " + url);
            yield break;
        }
        if (imageQueue.Contains(url))
        {
            Debug.Log("Image already being fetched " + url);
            yield break;
        }

        imageQueue.Add(url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        Debug.Log("Fetching texture " + url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Texture2D image = ((DownloadHandlerTexture)www.downloadHandler).texture;

            imageCache.Add(url, image);
            imageQueue.Remove(url);

            Debug.Log("Fetched texture from " + url);
        }
    }

    public IEnumerator FetchArtworks(string painterId)
    {
        if (painters.ContainsKey(painterId))
        {
            Debug.Log("Painter is already fetched " + painterId);
            yield break;
        }
        if (painterQueue.Contains(painterId))
        {
            Debug.Log("Painter is already being fetched " + painterId);
            yield break;
        }

        painterQueue.Add(painterId);

        using UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/artist/" + painterId);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            Debug.Log("Got response: " + webRequest.downloadHandler.text);
            Debug.Log("parsed " + JsonUtility.FromJson<ArtistResponse>("{\"artworks\":" + webRequest.downloadHandler.text + "}"));
            Debug.Log("parsed list " + JsonUtility.FromJson<ArtistResponse>("{\"artworks\":" + webRequest.downloadHandler.text + "}").artworks);
            painters.Add(painterId, JsonUtility.FromJson<ArtistResponse>("{\"artworks\":"+webRequest.downloadHandler.text+"}").artworks);

            painterQueue.Remove(painterId);

            Debug.Log("Painter size " + painters.Count);
            Debug.Log("Painter " + painters.GetValueOrDefault(painterId));
            Debug.Log("Fetched data for painter " + painterId + ", got " + painters[painterId].Count + " artworks");

            foreach(Artwork artwork in painters[painterId])
            {
                StartCoroutine(FetchImage(artwork.contentUrl));
            }
        }
    }

    public void SetCurrentPainter(string painterId)
    {
        currentPainter = painterId;
        currentArtworkIndex = -1;
    }

    public List<Artwork> GetArtworks(string painterId)
    {
        if (painters.ContainsKey(painterId))
        {
            return painters[painterId];
        }

        return new List<Artwork>();
    }
}
