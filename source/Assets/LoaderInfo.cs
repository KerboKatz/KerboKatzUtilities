using System;
using UnityEngine;

namespace KerboKatz.Assets
{
  public class LoaderInfo
  {
    public bool isReady = false;
    internal string objectName;
    internal Action<GameObject> onAssetLoaded;
    internal string path;
  }
}