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

    public void SetArtwork(Texture2D image, string label, MetaData metaData)
    {
        string text = label;

        if (metaData != null)
        {
            if (!string.IsNullOrWhiteSpace(metaData.name))
            {
                text += "\n Origin: " + metaData.name;
            }
            if (!string.IsNullOrWhiteSpace(metaData.pob))
            {
                text += "\n Place of Birth: " + metaData.pob;
            }
            if (!string.IsNullOrWhiteSpace(metaData.dob))
            {
                text += "\n Date of Birth: " + metaData.dob;
            }
            if (!string.IsNullOrWhiteSpace(metaData.spouse))
            {
                text += "\n Spouse: " + metaData.spouse;
            }
            if (!string.IsNullOrWhiteSpace(metaData.movements))
            {
                text += "\n Movement: " + metaData.movements;
            }

        }
        titleMesh.SetText(text);
        decalMaterial.SetTexture("Base_Map", image);
    }
}
