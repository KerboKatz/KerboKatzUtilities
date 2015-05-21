using UnityEngine;

namespace KerboKatz
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class FPS : MonoBehaviour
  {
    private float lastFPSCheck;
    private float framesSinceLastCheck;
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

    public float minFPS { get; private set; }

    public float currentFPS { get; private set; }

    public float maxFPS { get; private set; }

    private void Start()
    {
      minFPS = float.MaxValue;
      DontDestroyOnLoad(this);
      _instance = this;
      _instance.enabled = false;
    }

    private void Update()
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

    public void resetMinMax()
    {
      minFPS = float.MaxValue;
      maxFPS = 0;
    }
  }
}