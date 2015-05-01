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
  }
}