using System;
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

      public static float GetScienceValue(Dictionary<string, int> experimentCount, ScienceExperiment experiment, ScienceSubject currentScienceSubject, Func<ScienceExperiment, ScienceSubject, float> GetScienceValue = null, Func<ScienceExperiment, ScienceSubject, float> GetNextScienceValue = null)
      {
        float currentScienceValue;
        if (experimentCount.ContainsKey(currentScienceSubject.id))
        {
          if (GetNextScienceValue == null)
            GetNextScienceValue = Science.GetNextScienceValue;
          currentScienceValue = GetNextScienceValue(experiment, currentScienceSubject);
          if (experimentCount[currentScienceSubject.id] >= 2)//taken from scienceAlert to get somewhat accurate science values after the second experiment
            currentScienceValue = currentScienceValue / Mathf.Pow(4f, experimentCount[currentScienceSubject.id] - 1);
        }
        else
        {
          if (GetScienceValue == null)
            GetScienceValue = Science.GetScienceValue;
          currentScienceValue = GetScienceValue(experiment, currentScienceSubject);
        }
        return currentScienceValue;
      }

      private static float GetScienceValue(ScienceExperiment experiment, ScienceSubject currentScienceSubject)
      {
        return ResearchAndDevelopment.GetScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
      }

      private static float GetNextScienceValue(ScienceExperiment experiment, ScienceSubject currentScienceSubject)
      {
        return ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
      }
    }
  }
}