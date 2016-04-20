using UnityEngine;
using UnityEngine.Events;

namespace KerboKatz.UI
{
  public class Drop : MonoBehaviour
  {
    public class TransformEvent : UnityEvent<Transform> { }

    public TransformEvent OnObjectDroped = new TransformEvent();
  }
}