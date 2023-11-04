
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    public int SecondsPerImage = 3;

    private Dictionary<string, ArtistResponse> painters = new Dictionary<string, ArtistResponse>();
    private Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

    private CanvasDecalScript[] canvases;

    private string currentPainter = null;
    private int currentArtworkIndex = 0;

    private bool loading = false;

    private float lastCanvasUpdate = 0;

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
        string compressedUrl = "http://localhost:3000/image/" + HttpUtility.UrlEncode(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(compressedUrl);
        Debug.Log("Fetching texture " + compressedUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching image at " + url + ": " + www.error);
            yield break;
        }

        Texture2D image = ((DownloadHandlerTexture) www.downloadHandler).texture;

        images.Add(url, image);

        Debug.Log("Successfully texture from " + url);
    }

    private IEnumerator FetchNeighbourArtist(string path) {
        using UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/entity/" + path);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
            yield break;
        }

        ArtistResponse artist = JsonUtility.FromJson<ArtistResponse>(webRequest.downloadHandler.text);

        painters.Add(path, artist);
    }
    private IEnumerator FetchMainArtist(string path)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/entity/" + path);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
            yield break;
        }

        ArtistResponse mainArtist = JsonUtility.FromJson<ArtistResponse>(webRequest.downloadHandler.text);

        painters.Add(path, mainArtist);

        List<Coroutine> ops = new List<Coroutine>();

        foreach (Artwork artwork in mainArtist.images)
        {
            ops.Add(StartCoroutine(FetchImage(artwork.url)));
        }

        foreach (Link link in mainArtist.links)
        {
            ops.Add(StartCoroutine(FetchNeighbourArtist(link.type + "/" + link.id)));
        }

        foreach (Coroutine op in ops)
        {
            // Wait for all neighbours to be fetched
            yield return op;
        }
    }

    // Wouter you should call this method to start fetching stuff
    public IEnumerator SetCurrentPainter(string painterId, Action<List<DoorLink>> callback)
    {
        Debug.Log("Setting current painter to: " + painterId);

        loading = true;
        lastCanvasUpdate = 0;
        currentPainter = painterId;
        currentArtworkIndex = 0;

        yield return StartCoroutine(FetchMainArtist(painterId));

        loading = false;

        var links = painters[painterId].links;
        var result = new List<DoorLink>();
        foreach (var link in links)
        {
            if (painters.ContainsKey(link.type + "/" + link.id))
            {
                int num = painters[link.type + "/" + link.id].links.Count;
                result.Add(new DoorLink(link.type + "/" + link.id, link.label, num));
            } else {
                Debug.LogError("Missing neighbour " + link.type + "/" + link.id);
            }
        }

        callback(result);
    }

    void Start()
    {
        canvases = FindObjectsByType<CanvasDecalScript>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (currentPainter == null || loading) {
            return;
        }

        if(lastCanvasUpdate + SecondsPerImage < Time.time)
        {
            lastCanvasUpdate = Time.time;

            foreach (CanvasDecalScript canvas in canvases)
            {
                Artwork nextArtwork = GetNextArtwork();
                canvas.SetArtwork(images[nextArtwork.url], nextArtwork.label);
            }
        }
    }
}
