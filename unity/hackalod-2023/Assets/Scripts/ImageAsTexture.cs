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
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(ImageUrl);
        Debug.Log("Fetching texture " + ImageUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError(www.error);
        }
        else {
            Texture2D image = ((DownloadHandlerTexture) www.downloadHandler).texture;

            Material decalMaterial = this.GetComponent<DecalProjector>().material;
            decalMaterial.SetTexture("Base_Map", image);

            Debug.Log("Set texture from " + ImageUrl);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
