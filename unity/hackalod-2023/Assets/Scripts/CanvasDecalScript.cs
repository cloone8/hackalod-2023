using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class CanvasDecalScript : MonoBehaviour
{
    public string painterId;
    public int SecondsPerImage = 3;

    private Material decalMaterial;

    private Boolean readyForNext;
    private string currentImageUrl;
    private DateTime nextImageTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DataManager.Instance().FetchArtworks(painterId));
        decalMaterial = this.GetComponent<DecalProjector>().material;
        nextImageTime = DateTime.MinValue;
        DataManager.Instance().SetCurrentPainter(painterId);
        readyForNext = true;
    }

    void UpdateTexture(Texture2D image)
    {
        decalMaterial.SetTexture("Base_Map", image);
    }

    // Update is called once per frame
    void Update()
    {
        // Still loading painter
        if (DataManager.Instance().painterQueue.Contains(painterId))
        {
            return;
        }

        if (readyForNext && nextImageTime < DateTime.Now)
        {
            currentImageUrl = DataManager.Instance().GetNextImageUrl();
            Debug.Log("Got current image: " + currentImageUrl);
            readyForNext = false;
        }

        if (!readyForNext && !DataManager.Instance().imageQueue.Contains(currentImageUrl))
        {
            UpdateTexture(DataManager.Instance().imageCache[currentImageUrl]);

            nextImageTime = DateTime.Now.AddSeconds(SecondsPerImage);
            readyForNext = true;
        }
    }
}
