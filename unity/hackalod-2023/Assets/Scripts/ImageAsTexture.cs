using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class PaintingScript : MonoBehaviour
{
    public string ImageUrl;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTexture());
    }

    IEnumerator UpdateTexture() {
        // Load Image
        //Material decalMaterial = this.GetComponent<DecalProjector>().material;
        //decalMaterial.SetTexture("Base_Map", image);
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
