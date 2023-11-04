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

    // Start is called before the first frame update
    void Start()
    {
        Material templateMaterial = this.GetComponentInChildren<DecalProjector>().material;
        this.GetComponentInChildren<DecalProjector>().material = new Material(templateMaterial);
        decalMaterial = this.GetComponentInChildren<DecalProjector>().material;
        titleMesh = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetArtwork(Texture2D image, string label)
    {
        titleMesh.SetText(label);
        decalMaterial.SetTexture("Base_Map", image);
    }
}
