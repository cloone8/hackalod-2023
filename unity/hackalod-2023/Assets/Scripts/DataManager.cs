
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    public string debugPainterId;
    public int SecondsPerImage = 3;

    private HashSet<string> painterQueue;
    private HashSet<string> fetchImageQueue;

    private Dictionary<string, ArtistResponse> painters;
    private Dictionary<string, Texture2D> imageCache;

    private CanvasDecalScript[] canvases;

    private string currentPainter = null;
    private int currentArtworkIndex;

    public DataManager()
    {
        painters = new Dictionary<string, ArtistResponse>();
        imageCache = new Dictionary<string, Texture2D>();
        painterQueue = new HashSet<string>();
        fetchImageQueue = new HashSet<string>();
        currentArtworkIndex = -1;
    }

    public string GetCurrentPainterId()
    {
        return currentPainter;
    }

    public Artwork GetNextArtwork()
    {
        currentArtworkIndex++;

        List<Artwork> artworks = painters[currentPainter].images;

        return artworks[currentArtworkIndex %  artworks.Count];
    }

    public IEnumerator FetchImage(string url)
    {
        if (imageCache.ContainsKey(url))
        {
            Debug.Log("Image already present in cache " + url);
            yield break;
        }
        if (fetchImageQueue.Contains(url))
        {
            Debug.Log("Image already being fetched " + url);
            yield break;
        }

        fetchImageQueue.Add(url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        Debug.Log("Fetching texture " + url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Texture2D image = ((DownloadHandlerTexture) www.downloadHandler).texture;

            imageCache.Add(url, image);
            fetchImageQueue.Remove(url);

            Debug.Log("Fetched texture from " + url);
        }
    }

    public IEnumerator FetchArtist(string path, bool fetchNeighbours, bool fetchImages)
    {
        if (painters.ContainsKey(path))
        {
            Debug.Log("Painter is already fetched " + path);

            if (fetchNeighbours)
            {
                foreach (Link link in painters[path].links)
                {
                    StartCoroutine(FetchArtist(link.type + "/" + link.id, false, false));
                }
            }

            if (fetchImages)
            {
                foreach (Artwork artwork in painters[path].images)
                {
                    StartCoroutine(FetchImage(artwork.url));
                }
            }

            yield break;
        }
        if (painterQueue.Contains(path))
        {
            Debug.Log("Painter is already being fetched " + path);
            yield break;
        }

        painterQueue.Add(path);

        using UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/entity/" + path);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            painters.Add(path, JsonUtility.FromJson<ArtistResponse>(webRequest.downloadHandler.text));

            painterQueue.Remove(path);

            if (fetchNeighbours)
            {
                foreach (Link link in painters[path].links)
                {
                    StartCoroutine(FetchArtist(link.type + "/" + link.id, false, false));
                }
            }

            if (fetchImages)
            {
                foreach (Artwork artwork in painters[path].images)
                {
                    StartCoroutine(FetchImage(artwork.url));
                }
            }
        }
    }

    // Check if painterQueue is empty before calling this
    public List<Tuple<string, string, int>> GetLinksOfArtist(string path)
    {
        if (!painters.ContainsKey(path))
        {
            return new List<Tuple<string, string, int>>();
        }

        var links = painters[path].links;
        var result = new List<Tuple<string, string, int>>();
        foreach (var link in links)
        {
            int num = -1;
            if (painters.ContainsKey(link.type + "/" + link.id))
            {
                num = painters[link.type + "/" + link.id].links.Count;
            }
            result.Add(new Tuple<string, string, int>(link.type + "/" + link.id, link.label, num));
        }
        return result;
    }

    // Wouter you should call this method to start fetching stuff
    public void SetCurrentPainter(string painterId)
    {
        Debug.Log("Setting current painter to: " + painterId);

        currentPainter = painterId;

        currentArtworkIndex = -1;
        StartCoroutine(FetchArtist(painterId, true, true));
    }

    void Start()
    {
        if (currentPainter == null && !String.IsNullOrWhiteSpace(debugPainterId))
        {
            SetCurrentPainter(debugPainterId);
        }

        canvases = FindObjectsByType<CanvasDecalScript>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (currentPainter == null || painterQueue.Contains(currentPainter))
        {
            return;
        }

        foreach (CanvasDecalScript canvas in canvases)
        {
            if (canvas.WantsNewImage())
            {
                canvas.SetArtwork(GetNextArtwork());
            }

            if (!canvas.readyForNext && !fetchImageQueue.Contains(canvas.currentArtwork.url))
            {
                canvas.UpdateTexture(imageCache[canvas.currentArtwork.url]);
                canvas.nextImageTime = DateTime.Now.AddSeconds(SecondsPerImage);
                canvas.readyForNext = true;
            }
        }
    }
}
