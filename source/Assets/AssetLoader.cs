using KerboKatz.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KerboKatz.Assets
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class AssetLoader : KerboKatzBase<EmptySettings>
  {
    private static Dictionary<string, List<LoaderInfo>> pathsToLoad = new Dictionary<string, List<LoaderInfo>>();
    private static Dictionary<string, AssetBundle> cachedAssetBundles = new Dictionary<string, AssetBundle>();
    private static HashSet<string> isRuning = new HashSet<string>();
    private static EmptySettings staticSettings;
    private static AssetLoader instance;

    public AssetLoader()
    {
      modName = "KerboKatzAssetLoader";
      Log("Init done!");
    }

    public override void OnAwake()
    {
      instance = this;
      DontDestroyOnLoad(this);
      LoadSettings("", "AssetLoadSettings");
      staticSettings = settings;
    }

    public static T GetAsset<T>(string name, string pathInBundle = "", string bundlePath = "kerbokatz", string dataType = "png") where T : UnityEngine.Object
    {//now you are probably wondering why im going through all of this for simple icons/textures right ?
     //well kerbals assetloading takes its sweet time during startup and lots of small icons slow it down by alot! assetbundles on the other hand don't have that issue.

      StringBuilder texturePath = GetAssetPath(name, pathInBundle, dataType);

      AssetBundle bundle = GetBundle(bundlePath);
      Log("Bundle loaded. Accessing file at: ", texturePath.ToString());
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

    private static AssetBundle GetBundle(string bundlePath)
    {
      var dataPath = GetPath(bundlePath);
      Log("Path is: ", dataPath, " File exists: ", File.Exists(dataPath), " Path is: ", dataPath);
      AssetBundle bundle;
      if (!CheckBundle(dataPath, out bundle))
      {//when kerbal upgrades to unity 5.3 or higher this will be replaced by AssetBundle.LoadFromFile which can load compressed assetbundles as well
        Log("Couldn't find bundle. Loading from file!", dataPath);
        bundle = AssetBundle.LoadFromFile(dataPath);
        if (bundle == null)
        {
          Log("GetBundle returns null. ", dataPath);
        }
        cachedAssetBundles.Add(dataPath, bundle);
      }

      return bundle;
    }

    private static string GetPath(string bundlePath)
    {
      var path = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/KerboKatz/");
      path = Path.Combine(path, bundlePath + ".KerboKatzAsset");
      path = Path.GetFullPath(path);
      var directory = Path.GetDirectoryName(path);
      var fileName = Path.GetFileNameWithoutExtension(path).ToLower();
      path = Path.Combine(directory, fileName + ".KerboKatzAsset");
      return path;
    }

    private static bool CheckBundle(string dataPath, out AssetBundle bundle)
    {
      var foundBundle = cachedAssetBundles.TryGetValue(dataPath, out bundle);
      Log("FoundBundle: ", foundBundle, " is null: ", bundle == null, " ", dataPath);
      if (!foundBundle || bundle == null)
      {
        if (foundBundle)
        {
          cachedAssetBundles.Remove(dataPath);
          Log("Bundle is null!Removing!");
        }
        Log("Cache miss!");
        return false;
      }
      Log("Cache hit!");
      return true;
    }
    public static void Add(string path, string objectName, Action<GameObject> onAssetLoaded)
    {
      GetBundle("kerbokatz");
      var dataPath = GetPath(path);
      var loaderInfo = new LoaderInfo();
      loaderInfo.path = path;
      loaderInfo.objectName = objectName;
      loaderInfo.onAssetLoaded = onAssetLoaded;
      AssetBundle bundle;

      if (!CheckBundle(dataPath, out bundle))
      {
        if (instance != null)
        {
          instance.StartCoroutineForBundle(path);
          pathsToLoad.TryGetOrAdd(path).Add(loaderInfo);
          return;
        }
        else
        {
          bundle = GetBundle(path);
        }
      }
      SetLoaderReady(loaderInfo, bundle);
    }

    private void StartCoroutineForBundle(string dataPath)
    {
      if (isRuning.Contains(dataPath))
        return;
      StartCoroutine(LoadBundle(dataPath));
      isRuning.Add(dataPath);
    }

    public IEnumerator LoadBundle(string path)
    {
      var fromCache = false;
      var dataPath = GetPath(path);

      Log("Path is: ", dataPath, " File exists: ", File.Exists(dataPath), " Path is: ", dataPath);

      AssetBundle bundle;
      if (!CheckBundle(dataPath, out bundle))
      {
        Log("Couldn't find bundle. Loading from file!", dataPath);
        var bundleLoader = AssetBundle.LoadFromFileAsync(dataPath);
        yield return bundleLoader;

        isRuning.Remove(path);

        bundle = bundleLoader.assetBundle;

        if (bundle == null)
        {
          Log("LoadFromFileAsync returns null. ", dataPath);
          if (!CheckBundle(dataPath, out bundle))
          {
            Log("Checked bundle. It is nulll!", dataPath);
            yield break;
          }
          Log("Cache hit! Must have loaded it with magic!", dataPath);
          fromCache = true;
        }
        Log("Loaded bundle at ", dataPath);

        if(!fromCache)
          cachedAssetBundles.Add(dataPath, bundle);
      }
      var list = pathsToLoad.TryGetOrAdd(path);
      foreach (var loaderInfo in list)
      {
        SetLoaderReady(loaderInfo, bundle);
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
        Log("Failed to load asset. Does the asset(", loaderInfo.objectName, ") exist in the specified bundle(", loaderInfo.path, ") ? But found:", sb.ToString());
        return;
      }
      loaderInfo.onAssetLoaded(loadedAsset);
      loaderInfo.isReady = true;
    }

    private static new void Log(params object[] debugStrings)
    {
      if (staticSettings == null || staticSettings.debug)
        Utilities.Debug("KerboKatzAssetLoader", debugStrings);
    }
  }
}