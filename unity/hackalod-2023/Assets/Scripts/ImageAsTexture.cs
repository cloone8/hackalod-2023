using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError(www.error);
        }
        else {
            Texture2D image = ((DownloadHandlerTexture) www.downloadHandler).texture;

            Renderer renderer = GetComponent<Renderer>();
            // renderer.material.SetTexture();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
