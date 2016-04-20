using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KerboKatz.Extensions;
using UnityEngine;
using System.Collections;

namespace KerboKatz.Assets
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class AssetLoader : KerboKatzBase<EmptySettings>
  {
    private static Dictionary<string, List<LoaderInfo>> pathsToLoad = new Dictionary<string, List<LoaderInfo>>();
    private static Dictionary<string, AssetBundle> cachedAssetBundles = new Dictionary<string, AssetBundle>();
    HashSet<string> isRuning = new HashSet<string>();
    public AssetLoader()
    {
      modName = "KerboKatzAssetLoader";
      Log("Init done!");
    }
    public override void OnAwake()
    {
      DontDestroyOnLoad(this);
    }
    public static LoaderInfo Add(string path, string objectName, Action<GameObject> onAssetLoaded)
    {
      path = "file://" + KSPUtil.ApplicationRootPath + "GameData/KerboKatz/" + path + ".KerboKatzAsset";
      var loaderInfo = new LoaderInfo();
      loaderInfo.path = path;
      loaderInfo.objectName = objectName;
      loaderInfo.onAssetLoaded = onAssetLoaded;
      AssetBundle bundle;

      if (!CheckBundle(path, out bundle))
      {
        var list = pathsToLoad.TryGetOrAdd(path);
        list.Add(loaderInfo);
      }
      else
      {
        SetLoaderReady(loaderInfo, bundle);
      }
      return loaderInfo;
    }
    public static T GetAsset<T>(string name, string pathInBundle = "", string dataType = "png") where T : UnityEngine.Object
    {//now you are probably wondering why im going through all of this for simple icons/textures right ?
     //well kerbals assetloading takes its sweet time during startup and lots of small icons slow it down by alot! assetbundles on the other hand don't have that issue.
      StringBuilder texturePath = GetAssetPath(name, pathInBundle, dataType);

      AssetBundle bundle = GetMainBundle();

      return bundle.LoadAsset<T>(texturePath.ToString());
    }

    private static StringBuilder GetAssetPath(string name, string pathInBundle, string dataType)
    {
      var texturePath = new StringBuilder();
      texturePath.Append("assets/kerbokatz/textures/");
      texturePath.Append(pathInBundle);
      texturePath.Append("/");
      texturePath.Append(name);
      texturePath.Append(".");
      texturePath.Append(dataType);
      return texturePath;
    }

    private static AssetBundle GetMainBundle()
    {
      var path = KSPUtil.ApplicationRootPath + "GameData/KerboKatz/kerbokatz.KerboKatzAsset";
      var wwwPath = "file://" + path;

      AssetBundle bundle;
      if (!CheckBundle(wwwPath, out bundle))
      {//when kerbal upgrades to unity 5.3 or higher this will be replaced by AssetBundle.LoadFromFile which can load compressed assetbundles as well
        bundle = AssetBundle.CreateFromFile(path);
        cachedAssetBundles.Add(wwwPath, bundle);
      }

      return bundle;
    }

    private static bool CheckBundle(string wwwPath, out AssetBundle bundle)
    {
      var foundBundle = cachedAssetBundles.TryGetValue(wwwPath, out bundle);
      if (!foundBundle || bundle == null)
      {
        if (foundBundle)
        {
          cachedAssetBundles.Remove(wwwPath);
          Debug.LogError("[KerboKatzAssetLoader] bundle is null!");
        }
        return false;
      }
      return true;
    }

    void Update()
    {
      foreach (var list in pathsToLoad)
      {
        if (!isRuning.Contains(list.Key))
        {
          StartCoroutine(LoadBundle(list.Key));
          isRuning.Add(list.Key);
        }
      }
    }

    public IEnumerator LoadBundle(string path)
    {
      using (WWW www = new WWW(path))
      {
        yield return www;
        AssetBundle assetBundle;
        if (!CheckBundle(path, out assetBundle))
        {
          assetBundle = www.assetBundle;

          if (assetBundle == null)
          {
            isRuning.Remove(path);
            Debug.LogError("[" + modName + "] bundle loaded as null!:" + path);
            yield break;
          }
          cachedAssetBundles.Add(path, assetBundle);
        }
        var list = pathsToLoad.TryGetOrAdd(path);
        foreach (var loaderInfo in list)
        {
          SetLoaderReady(loaderInfo, assetBundle);
        }
        pathsToLoad.Remove(path);
        isRuning.Remove(path);
        //list.Clear();
      }
    }

    private static void SetLoaderReady(LoaderInfo loaderInfo, AssetBundle assetBundle)
    {

      var loadedAsset = assetBundle.LoadAsset<GameObject>(loaderInfo.objectName);
      if (loadedAsset == null)
      {
        var sb = new StringBuilder();
        foreach (var asset in assetBundle.GetAllAssetNames())
        {
          sb.AppendLine(asset);
        }
        Debug.LogError("[KerboKatzAssetLoader] Failed to load asset. Does the asset(" + loaderInfo.objectName + ") exist in the specified bundle(" + loaderInfo.path + ") ? But found:" + sb.ToString());
        return;
      }
      loaderInfo.onAssetLoaded(loadedAsset);
      loaderInfo.isReady = true;
    }
  }
}
