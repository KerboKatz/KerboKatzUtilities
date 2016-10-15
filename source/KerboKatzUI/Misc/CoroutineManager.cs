using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerboKatz.UI
{
  public class CoroutineManager : MonoBehaviour
  {
    private static CoroutineManager _instance;
    public static CoroutineManager instance
    {
      get
      {
        if (_instance == null)
        {
          var go = new GameObject();
          _instance = go.AddComponent<CoroutineManager>();
        }
        return _instance;
      }
    }
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
      return base.StartCoroutine(routine);
    }
    public new void StopCoroutine(Coroutine routine)
    {
      base.StopCoroutine(routine);
    }
  }
}
