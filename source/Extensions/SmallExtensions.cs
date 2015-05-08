using KerboKatz.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz.Extensions
{
  public static partial class SmallExtensions
  {
    public static void setToZero(this RectOffset offset)
    {
      offset.left = 0;
      offset.right = 0;
      offset.bottom = 0;
      offset.top = 0;
    }

    /**
     * since .net 3.5 doesn't have has flag and i needed something like that i implemented this
     */
    public static bool HasFlag(this ApplicationLauncher.AppScenes source, ApplicationLauncher.AppScenes flag)
    {
      return (source & flag) == flag;
    }
    public static bool HasFlag(this Utilities.LogMode source, Utilities.LogMode flag)
    {
      return (source & flag) == flag;
    }

    public static Rect moveToCursor(this Rect rect, float offsetX = 10, float offsetY = 10)
    {
      rect.x = Input.mousePosition.x + offsetX;
      rect.y = Screen.height - Input.mousePosition.y + offsetY;
      return rect;
    }

    public static Vector3 clampTo(this Vector3 current, Vector3 min, Vector3 max)
    {
      if (current.x > max.x)
      {
        current.x = max.x;
      }
      else if (current.x < min.x)
      {
        current.x = min.x;
      }

      if (current.y > max.y)
      {
        current.y = max.y;
      }
      else if (current.y < min.y)
      {
        current.y = min.y;
      }

      if (current.z > max.z)
      {
        current.z = max.z;
      }
      else if (current.z < min.z)
      {
        current.z = min.z;
      }
      return current;
    }

    private static Dictionary<KeyBinding, KeyBindingStorage> KeyBindingStorage = new Dictionary<KeyBinding, KeyBindingStorage>();

    public static void saveDefault(this KeyBinding KeyBinding)
    {
      if (!KeyBindingStorage.ContainsKey(KeyBinding))
        KeyBindingStorage.Add(KeyBinding, new KeyBindingStorage(KeyBinding.primary, KeyBinding.secondary));
    }

    public static void setNone(this KeyBinding KeyBinding)
    {
      KeyBinding.primary = KeyCode.None;
      KeyBinding.secondary = KeyCode.None;
    }

    public static void reset(this KeyBinding KeyBinding)
    {
      if (KeyBindingStorage.ContainsKey(KeyBinding))
      {
        KeyBinding.primary = KeyBindingStorage[KeyBinding].primary;
        KeyBinding.secondary = KeyBindingStorage[KeyBinding].secondary;
      }
    }

    public static KeyCode getDefaultPrimary(this KeyBinding KeyBinding)
    {
      if (KeyBindingStorage.ContainsKey(KeyBinding))
      {
        return KeyBindingStorage[KeyBinding].primary;
      }
      return KeyCode.None;
    }

    private static Dictionary<AxisBinding, AxisBindingStorage> AxisBindingStorage = new Dictionary<AxisBinding, AxisBindingStorage>();
    public static void saveDefault(this AxisBinding AxisBinding)
    {
      if (!AxisBindingStorage.ContainsKey(AxisBinding))
        AxisBindingStorage.Add(AxisBinding, new AxisBindingStorage(AxisBinding.primary.scale, AxisBinding.secondary.scale));
    }

    public static void setZero(this AxisBinding AxisBinding)
    {
      AxisBinding.primary.scale = 0;
      AxisBinding.secondary.scale = 0;
    }

    public static void reset(this AxisBinding AxisBinding)
    {
      if (AxisBindingStorage.ContainsKey(AxisBinding))
      {
        AxisBinding.primary.scale = AxisBindingStorage[AxisBinding].primaryScale;
        AxisBinding.secondary.scale = AxisBindingStorage[AxisBinding].secondaryScale;
      }
    }
  }
}