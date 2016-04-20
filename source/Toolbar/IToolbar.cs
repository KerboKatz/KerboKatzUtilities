using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace KerboKatz.Toolbar
{
  public interface IToolbar
  {
    string modName { get; }
    string displayName { get; }
    string tooltip { get; }
    List<GameScenes> activeScences { get; }
    Sprite icon { get; }
    UnityAction onClick { get; }
  }
}
