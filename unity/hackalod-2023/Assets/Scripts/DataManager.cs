
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    public int SecondsPerImage = 5;

    private List<Tuple<Link, ArtistResponse>> neighbours = new();

    private CanvasDecalScript[] canvases;

    private string currentPainter = null;
    private ArtistResponse mainArtist = null;

    private int currentArtworkIndex = 0;

    private bool loading = false;

    private float lastCanvasUpdate = 0;

    public string GetCurrentPainterId()
    {
        return currentPainter;
    }

    public Artwork GetNextArtwork()
    {
        if(mainArtist == null) {
            return null;
        }

        currentArtworkIndex++;

        List<Artwork> artworks = mainArtist.images;

        return artworks[currentArtworkIndex % artworks.Count];
    }

    public IEnumerator FetchImage(string url, Action<Texture2D> callback)
    {
        string compressedUrl = "http://localhost:3000/image/" + HttpUtility.UrlEncode(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(compressedUrl);
        Debug.Log("Fetching texture " + compressedUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching image at " + url + ": " + www.error);
            callback(null);
            yield break;
        }

        Texture2D image = ((DownloadHandlerTexture) www.downloadHandler).texture;
        Debug.Log("Successfully texture from " + url);

        callback(image);
    }

    private IEnumerator FetchNeighbourArtist(Link link) {
        using UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/entity/" + link.type + "/" + link.id);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
            yield break;
        }

        ArtistResponse artist = JsonUtility.FromJson<ArtistResponse>(webRequest.downloadHandler.text);

        if(artist.links.Count < 2) {
            Debug.Log("Artist " + artist.metadata.name + " has less than 2 links, skipping");
            yield break;
        }

        if(artist.images.Count == 0 && artist.links.Count < RoomManager.Instance().GetMaxDoors()) {
            Debug.Log("Artist " + artist.metadata.name + " has no images, skipping");
            yield break;
        }

        neighbours.Add(new(link, artist));
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

        mainArtist = JsonUtility.FromJson<ArtistResponse>(webRequest.downloadHandler.text);

        int linksNeeded = Math.Min(mainArtist.links.Count, RoomManager.Instance().GetMaxDoors());
        Debug.Log("Fetching " + linksNeeded + " links");
        foreach (Link link in mainArtist.links)
        {
            yield return StartCoroutine(FetchNeighbourArtist(link));
            Debug.Log("Fetched link " + link.id);
            Debug.Log("Got " + neighbours.Count + " links");

            if (neighbours.Count >= linksNeeded)
            {
                break;
            }
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

        var result = new List<DoorLink>();

        foreach (var neighbour in neighbours)
        {
            Link link = neighbour.Item1;
            ArtistResponse artist = neighbour.Item2;

            int num = artist.links.Count;
            result.Add(new DoorLink(link.type + "/" + link.id, link.label, num));
        }

        callback(result);
    }

    void Start()
    {
        canvases = FindObjectsByType<CanvasDecalScript>(FindObjectsSortMode.None);
    }

    void UpdateCanvas(CanvasDecalScript canvas) {
        Artwork nextArtwork = GetNextArtwork();

        StartCoroutine(FetchImage(nextArtwork.url, (image) => {
            if(image == null) {
                // Try again
                UpdateCanvas(canvas);
            } else {
                canvas.SetArtwork(image, nextArtwork.label, mainArtist.metadata);
            }
        }));
    }

    void Update()
    {
        if (currentPainter == null || loading || mainArtist == null) {
            return;
        }

        if(lastCanvasUpdate + SecondsPerImage < Time.time)
        {
            lastCanvasUpdate = Time.time;

            foreach (CanvasDecalScript canvas in canvases)
            {
                UpdateCanvas(canvas);
            }
        }
    }
}
