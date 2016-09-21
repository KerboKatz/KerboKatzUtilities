using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class UpdateAnimatorState : MonoBehaviour
  {
    public Toggle toggle;
    public Animator animator;
    public AnimatorState state;

    private void Awake()
    {
      toggle.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(bool arg0)
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