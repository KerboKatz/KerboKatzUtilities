using UnityEngine;

namespace KerboKatz.Classes
{
  public class KeyBindingStorage
  {
    public KeyCode primary;
    public KeyCode secondary;

    public KeyBindingStorage(KeyCode primary, KeyCode secondary)
    {
      this.primary = primary;
      this.secondary = secondary;
    }
  }

  public class AxisBindingStorage
  {
    public float primaryScale;
    public float secondaryScale;

    public AxisBindingStorage(float scale, float sensitivity)
    {
      this.primaryScale = scale;
      this.secondaryScale = sensitivity;
    }
  }
}