using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class ExpandingInputFieldSyncMaster : UIBehaviour
  {
    private Dictionary<ExpandingInputField, float> syncWith = new Dictionary<ExpandingInputField, float>();
    private float oldSize = 0;

    protected override void OnEnable()
    {
      UpdateSizes();
    }

    public void Add(ExpandingInputField newSyncTarget)
    {
      syncWith.Add(newSyncTarget, 0);
      newSyncTarget.onResize.AddListener(OnResize);
      UpdateSizes();
    }

    private void OnResize(ExpandingInputField arg0, float newSize)
    {
      var oSize = syncWith[arg0];
      syncWith[arg0] = newSize;
      if (oldSize == oSize || newSize > oldSize)
      {
        UpdateSizes();
      }
    }

    private void UpdateSizes()
    {
      float max = 0;
      foreach (var pair in syncWith)
      {
        if (pair.Value > max)
        {
          max = pair.Value;
        }
      }
      foreach (var pair in syncWith)
      {
        pair.Key.SetSizeTo(max);
      }
      oldSize = max;
    }

    public void Remove(ExpandingInputField oldSyncTarget)
    {
      syncWith.Remove(oldSyncTarget);
      oldSyncTarget.onResize.RemoveListener(OnResize);
      UpdateSizes();
    }
  }
}