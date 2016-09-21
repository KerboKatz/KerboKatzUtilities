using System;
using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class UpdateAnimatorStates : MonoBehaviour
  {
    public Toggle toggle;
    public Animator animator;
    public AnimatorState[] states;

    private void Awake()
    {
      toggle.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(bool arg0)
    {
      foreach (var state in states)
      {
        if (state.isBool)
        {
          animator.SetBool(state.variableName, arg0);
        }
        else if (state.isFloat)
        {
          if (arg0)
            animator.SetFloat(state.variableName, state.trueFloatValue);
          else
            animator.SetFloat(state.variableName, state.falseFloatValue);
        }
        else if (state.isString)
        {
          if (arg0)
            animator.SetFloat(state.variableName, state.trueStringValue);
          else
            animator.SetFloat(state.variableName, state.falseStringValue);
        }
      }
    }
  }

  [Serializable]
  public class AnimatorState : ScriptableObject
  {
    public string variableName;
    public bool isBool;
    public bool isFloat;
    public float trueFloatValue;
    public float falseFloatValue;
    public bool isString;
    public float trueStringValue;
    public float falseStringValue;
  }
}