using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class CanvasDecalScript : MonoBehaviour
{
    private Material decalMaterial;
    private TextMeshProUGUI titleMesh;

    public Boolean readyForNext;
    public Artwork currentArtwork { get; private set; }
    public DateTime nextImageTime;

    // Start is called before the first frame update
    void Start()
    {
        Material templateMaterial = this.GetComponentInChildren<DecalProjector>().material;
        this.GetComponentInChildren<DecalProjector>().material = new Material(templateMaterial);
        decalMaterial = this.GetComponentInChildren<DecalProjector>().material;

        titleMesh = this.GetComponentInChildren<TextMeshProUGUI>();
        nextImageTime = DateTime.MinValue;
        readyForNext = true;
    }

    public void UpdateTexture(Texture2D image)
    {
        decalMaterial.SetTexture("Base_Map", image);

        if (titleMesh.text != currentArtwork.label)
        {
            titleMesh.SetText(currentArtwork.label);
        }
    }

    public Boolean WantsNewImage()
    {
        return readyForNext && nextImageTime < DateTime.Now;
    }

    public void SetArtwork(Artwork artwork)
    {
        if (currentArtwork == null)
        {
            titleMesh.SetText(artwork.label);
        }
        currentArtwork = artwork;
        Debug.Log(GetInstanceID() + " Got current image: " + currentArtwork.url);
        readyForNext = false;
    }
}
