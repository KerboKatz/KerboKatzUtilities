using KerboKatz.Classes;
using KerboKatz.ConfigNodeExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class RecoverAll
    {
      private static double maxKSCDistance;
      public static void addVesselInfo(Vessel vessel,ref Dictionary<string, int> experimentCount, ref List<vesselInfo> vesselInfoList,bool destroyAll  = false)
      {
        if (maxKSCDistance == 0)
          maxKSCDistance = SpaceCenter.Instance.cb.Radius * Math.PI;
        float totalCost = 0, totalScience = 0, partCost, science, crewCount = 0;
        float dryCost, fuelCost;
        //var parts = new List<partInfo>();
        var parts = new Dictionary<string,partInfo>();
        var scienceExperiments = new List<scienceInfo>();
        var crew = new List<ProtoCrewMember>();
        var resourceInfo = new Dictionary<string,resourceInfo>();
        float distanceModifier  = 1;
        if(!destroyAll)
          distanceModifier = 1 - Utilities.toFloat(SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(vessel.protoVessel.latitude, vessel.protoVessel.longitude)) / maxKSCDistance);
        foreach (var part in vessel.protoVessel.protoPartSnapshots)
        {
          totalCost += partCost = ShipConstruction.GetPartCosts(part, part.partInfo, out dryCost, out fuelCost);
          var identifier = part.partInfo.name + dryCost;
          if (parts.ContainsKey(identifier))
          {
            parts[identifier].amount++;
          }else{
            parts.Add(identifier, new partInfo(part.partInfo.title, dryCost));
          }
          foreach (var resource in part.resources)
          {
            if (resourceInfo.ContainsKey(resource.resourceName))
            {
              resourceInfo[resource.resourceName].amount += resource.resourceValues.getFloatValue("amount");//.GetValue("amount");//resource.resourceRef.amount;
            }
            else
            {
              //resource.resourceValues.
              resourceInfo.Add(resource.resourceName, new resourceInfo(resource.resourceName, PartResourceLibrary.Instance.GetDefinition(resource.resourceName).unitCost, resource.resourceValues.getFloatValue("amount")));//resource.resourceRef.amount));
            }
          }
          foreach (ProtoPartModuleSnapshot partModule in part.modules)
          {
            if (partModule.moduleValues.HasNode("ScienceData"))
            {
              foreach (ConfigNode subjectNode in partModule.moduleValues.GetNodes("ScienceData"))
              {
                var scienceSubject = ResearchAndDevelopment.GetSubjectByID(subjectNode.GetValue("subjectID"));

                var experiment = ResearchAndDevelopment.GetExperiment(scienceSubject.id.Split('@')[0]);
                totalScience += science = Utilities.Science.getScienceValue(experimentCount, experiment, scienceSubject);
                Science.addToExperimentCount(ref experimentCount, scienceSubject);

                scienceExperiments.Add(new scienceInfo(scienceSubject, science, scienceSubject.title, Utilities.toFloat(subjectNode.GetValue("data"))));
              }
            }
          }
        }
        foreach (ProtoCrewMember protoCrew in vessel.protoVessel.GetVesselCrew())
        {
          crew.Add(protoCrew);
          crewCount++;
        }
        List<alignedTooltip> tooltipList = new List<alignedTooltip>();
        var currentVessel                = new vesselInfo(parts, resourceInfo, scienceExperiments, crew, new importantInfo(vessel, vessel.vesselName, totalCost, totalScience, crewCount, distanceModifier, true));
        float maxSize                    = 0;
        //creating the tooltips and caching them puts less strain on the gc on the cost of memory
        //but it would all sit in memory anyways so this seems to be the smart move :)
        currentVessel.partTooltip        = Utilities.RecoverAll.createPartInfo(currentVessel, ref tooltipList, ref maxSize);
        currentVessel.scienceTooltip     = Utilities.RecoverAll.createScienceInfo(currentVessel, ref tooltipList, ref maxSize);
        currentVessel.crewTooltip        = Utilities.RecoverAll.createCrewString(currentVessel);
        vesselInfoList.Add(currentVessel);
      }
      public static string createPartInfo(vesselInfo currentVessel, ref List<alignedTooltip> tooltipList, ref float maxSize)
      {
        foreach (var currentPart in currentVessel.partInfo.Values)
        {
          maxSize = Utilities.UI.addToTooltipList(ref tooltipList, maxSize, currentPart.amount.ToString("N0") + " x " + currentPart.name, currentPart.cost * currentPart.amount * currentVessel.importantInfo.distanceModifier);
        }
        foreach (var currentResource in currentVessel.resourceInfo.Values)
        {
          maxSize = Utilities.UI.addToTooltipList(ref tooltipList, maxSize, currentResource.amount.ToString("N0") + " x " + currentResource.name, currentResource.cost * currentResource.amount);
        }
        var partString = Utilities.UI.createAlignedOutString(tooltipList, maxSize);
        maxSize = 0;
        tooltipList.Clear();
        return partString;
      }

      public static string createScienceInfo(vesselInfo currentVessel, ref List<alignedTooltip> tooltipList, ref float maxSize)
      {
        foreach (var currentScience in currentVessel.scienceInfo)
        {
          maxSize = Utilities.UI.addToTooltipList(ref tooltipList, maxSize, currentScience.title, currentScience.science);
        }
        var scienceString = Utilities.UI.createAlignedOutString(tooltipList, maxSize);
        maxSize = 0;
        tooltipList.Clear();
        return scienceString;
      }

      public static string createCrewString(vesselInfo currentVessel)
      {
        string crewString = "";
        foreach (var currentCrew in currentVessel.crewInfo)
        {
          if (crewString != "")
            crewString += "\n";
          crewString += currentCrew.name;
        }
        return crewString;
      }
    }
  }
}
