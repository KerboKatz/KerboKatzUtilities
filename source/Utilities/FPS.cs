using UnityEngine;

namespace KerboKatz
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class FPS : MonoBehaviour
  {
    private float lastFPSCheck;
    private float framesSinceLastCheck;

    public static float minFPS { get; private set; }

    public static float currentFPS { get; private set; }

    public static float maxFPS { get; private set; }

    public void Start()
    {
      minFPS = float.MaxValue;
      DontDestroyOnLoad(this);
    }

    public void Update()
    {
      if (lastFPSCheck + 1 > Time.realtimeSinceStartup)
      {
        framesSinceLastCheck++;
      }
      else
      {
        currentFPS = framesSinceLastCheck / (Time.realtimeSinceStartup - lastFPSCheck);
        framesSinceLastCheck = framesSinceLastCheck - currentFPS;
        lastFPSCheck = Time.realtimeSinceStartup;
        if (currentFPS < minFPS && currentFPS > 0)
          minFPS = currentFPS;
        if (currentFPS > maxFPS)
          maxFPS = currentFPS;
      }
    }

    public static void resetMinMax()
    {
      minFPS = float.MaxValue;
      maxFPS = 0;
    }
  }
}