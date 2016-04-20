using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace KerboKatz
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class FPS : MonoBehaviour
  {
    private static FPS _instance;

    public static FPS instance
    {
      get
      {
        if (!_instance.enabled)
        {
          _instance.enabled = true;
        }
        return _instance;
      }
    }


    private Queue<Stopwatch> frames = new Queue<Stopwatch>();
    public static int currentFPS { get; private set; }
    public static int maxFPS { get; private set; }
    public static int minFPS { get; private set; }
    private void Update()
    {
      var frameSW = new Stopwatch();
      frameSW.Start();
      frames.Enqueue(frameSW);
      PeekAndRemove();
      if (currentFPS < minFPS && currentFPS > 0)
        minFPS = currentFPS;
      if (currentFPS > maxFPS)
        maxFPS = currentFPS;

      currentFPS = frames.Count;
    }

    private void PeekAndRemove()
    {
      if (frames.Count == 0)
        return;
      if (frames.Peek().Elapsed.TotalMilliseconds >= 1000)
      {
        frames.Dequeue();
        PeekAndRemove();
      }
    }

    private void Start()
    {
      minFPS = int.MaxValue;
      DontDestroyOnLoad(this);
      _instance = this;
    }
    public void ResetMinMax()
    {
      minFPS = int.MaxValue;
      maxFPS = 0;
    }
  }
}