using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class Science
    {
      public static void addToExperimentCount(ref Dictionary<string, int> experimentCount, ScienceSubject scienceSubject)
      {
        if (experimentCount.ContainsKey(scienceSubject.id))
        {
          experimentCount[scienceSubject.id] = experimentCount[scienceSubject.id] + 1;
        }
        else
        {
          experimentCount.Add(scienceSubject.id, 1);
        }
      }

      public static float GetScienceValue(Dictionary<string, int> experimentCount, ScienceExperiment experiment, ScienceSubject currentScienceSubject)
      {
        float currentScienceValue;
        if (experimentCount.ContainsKey(currentScienceSubject.id))
        {
          currentScienceValue = ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
          if (experimentCount[currentScienceSubject.id] >= 2)//taken from scienceAlert to get somewhat accurate science values after the second experiment
            currentScienceValue = currentScienceValue / Mathf.Pow(4f, experimentCount[currentScienceSubject.id] - 1);
        }
        else
        {
          currentScienceValue = ResearchAndDevelopment.GetScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
        }
        return currentScienceValue;
      }
    }
  }
}