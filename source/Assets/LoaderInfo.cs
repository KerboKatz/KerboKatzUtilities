using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
