using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class CanvasDecalScript : MonoBehaviour
{
    private Material decalMaterial;

    public Boolean readyForNext;
    public string currentImageUrl { get; private set; }
    public DateTime nextImageTime;

    // Start is called before the first frame update
    void Start()
    {
        Material templateMaterial = this.GetComponentInChildren<DecalProjector>().material;
        this.GetComponentInChildren<DecalProjector>().material = new Material(templateMaterial);
        decalMaterial = this.GetComponentInChildren<DecalProjector>().material;
        nextImageTime = DateTime.MinValue;
        readyForNext = true;
    }

    public void UpdateTexture(Texture2D image)
    {
        //image.width
        decalMaterial.SetTexture("Base_Map", image);
    }

    public Boolean WantsNewImage()
    {
        return readyForNext && nextImageTime < DateTime.Now;
    }

    public void SetImageUrl(string url)
    {
        currentImageUrl = url;
        Debug.Log(GetInstanceID() + " Got current image: " + currentImageUrl);
        readyForNext = false;
    }
}
