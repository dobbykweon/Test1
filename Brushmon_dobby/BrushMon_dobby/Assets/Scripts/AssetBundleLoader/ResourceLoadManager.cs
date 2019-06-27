using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;
using UnityEngine.UI;

public enum ResourceType
{
    stacker,
    accessory
}

public class ResourceLoadManager : GlobalMonoSingleton<ResourceLoadManager>
{

    public const string AssetBundlesOutputPath = "/AssetBundles/";
    private readonly string assetBundleName = "resources_sticker_pack";
    private readonly string assetName;
    private AssetBundleLoadManifestOperation assetBundleLoadManifest;

    public IEnumerator Initialize()
    {
        if (assetBundleLoadManifest == null)
        {
            LogManager.Log("ResourceLoadManager Initialize");

            //AssetBundleManager.SetSourceAssetBundleURL ("http://redis.brushmon.com/static" + AssetBundlesOutputPath);
            //AssetBundleManager.SetSourceAssetBundleURL("http://assets.brushmon.com/hanwha_160" + AssetBundlesOutputPath);
            AssetBundleManager.SetSourceAssetBundleURL("http://assets.brushmon.com/sticker_pack" + AssetBundlesOutputPath);

            assetBundleLoadManifest = AssetBundleManager.Initialize();
        }

        if (assetBundleLoadManifest != null)
        {
            LogManager.Log("Load AssetBundle");
            yield return StartCoroutine(assetBundleLoadManifest);
        }
    }

    public void CoInstantiateGameSpriteAsync(string assetName, Action<Sprite> callback)
    {
#if OFFLINE
        Sprite sprite = Resources.Load<Sprite>("Textures/sticker/" + assetName);
        callback(sprite);
#else
        StartCoroutine(InstantiateGameSpriteAsync(assetName, callback));
#endif
    }

    public IEnumerator InstantiateGameSpriteAsync(string assetName, Action<Sprite> callback)
    {
        if (assetBundleLoadManifest == null)
        {
            yield return StartCoroutine(Initialize());
        }
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(Sprite));
        // LoadedAssetBundle assetBundle;
        // assetBundle.
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        Sprite prefab = request.GetAsset<Sprite>();
        callback(prefab);

        // Calculate and display the elapsed time.
        //float elapsedTime = Time.realtimeSinceStartup - startTime;
        //LogManager.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }


    public void CoInstantiateGameSpriteAsync(string assetName, Action<Sprite> callback, ResourceType resourceType)
    {
#if OFFLINE
        Sprite sprite = Resources.Load<Sprite>("Textures/" + resourceType.ToString() + "/" + assetName);
        callback(sprite);
#else
        StartCoroutine(InstantiateGameSpriteAsync(assetName, callback, resourceType));
#endif
    }

    public IEnumerator InstantiateGameSpriteAsync(string assetName, Action<Sprite> callback, ResourceType resourceType)
    {
        if (assetBundleLoadManifest == null)
        {
            yield return StartCoroutine(Initialize());
        }
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync("resources_" + resourceType.ToString() + "_pack", assetName, typeof(Sprite));
        // LoadedAssetBundle assetBundle;
        // assetBundle.
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        Sprite prefab = request.GetAsset<Sprite>();
        callback(prefab);

        // Calculate and display the elapsed time.
        //float elapsedTime = Time.realtimeSinceStartup - startTime;
        //LogManager.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }
}