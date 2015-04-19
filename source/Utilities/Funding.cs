using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class Funding
    {
      public static void addFunds(double add, TransactionReasons reason = TransactionReasons.None)
      {
        if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
          return;
        global::Funding.Instance.AddFunds(add, reason);
      }
    }
    #region deprecated
    [Obsolete("Use Utilities.Science instead")]
    public static void addFunds(float add, TransactionReasons reason)
    {
      Funding.addFunds(add, reason);
    }
    #endregion
  }
}
