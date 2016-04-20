using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  class RightMouseButtonClick : MonoBehaviour, IPointerClickHandler
  {
    private Button button;

    void Awake()
    {
      button = GetComponent<Button>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
      if(eventData.button == PointerEventData.InputButton.Right)
      {
        button.onClick.Invoke();
      }
    }
  }
}
