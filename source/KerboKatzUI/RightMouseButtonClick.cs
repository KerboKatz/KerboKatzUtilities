using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  internal class RightMouseButtonClick : MonoBehaviour, IPointerClickHandler
  {
    private Button button;

    private void Awake()
    {
      button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Right)
      {
        button.onClick.Invoke();
      }
    }
  }
}