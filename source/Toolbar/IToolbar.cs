using System.Collections.Generic;
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