using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetBundleLoader : MonoBehaviour
{
    [SerializeField]
    private string bundleUrl = "http://mygameserver.com/myassets/spritesbundle";
    [SerializeField]
    private string spriteNameInBundle = "PlayerAvatar";
    [SerializeField]
    private Image uiImageDisplay;

    IEnumerator Start()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(bundleUrl))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = bundleUrl.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                    Sprite loadedSprite = assetBundle.LoadAsset<Sprite>(spriteNameInBundle);

                    if (loadedSprite != null && uiImageDisplay != null)
                    {
                        uiImageDisplay.sprite = loadedSprite;
                        Debug.Log($"Loaded {loadedSprite.name} from Asset Bundle.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load sprite from bundle or UI Image not assigned.");
                    }

                    assetBundle.Unload(false); // Unload the bundle but keep loaded assets in memory
                    break;
            }
        }
        //using (WWW www = new WWW(bundleUrl)) // Old way, but simple for example
        //{
        //    yield return www;

        //    if (www.error != null)
        //    {
        //        Debug.LogError("Error loading bundle: " + www.error);
        //        yield break;
        //    }

        //    AssetBundle bundle = www.assetBundle;
        //    Sprite loadedSprite = bundle.LoadAsset<Sprite>(spriteNameInBundle);

        //    if (loadedSprite != null && uiImageDisplay != null)
        //    {
        //        uiImageDisplay.sprite = loadedSprite;
        //        Debug.Log($"Loaded {loadedSprite.name} from Asset Bundle.");
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to load sprite from bundle or UI Image not assigned.");
        //    }

        //    bundle.Unload(false); // Unload the bundle but keep loaded assets in memory
        //}
    }

}
