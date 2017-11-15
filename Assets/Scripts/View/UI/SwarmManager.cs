﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SwarmManager
{
    public static bool IsActive;

    public static void CheckActivation()
    {
        //Debug.Log(!IsActive + " " + (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase)).ToString() + " " + (Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).GetType() == typeof(Players.HumanPlayer)).ToString());
        if (!IsActive && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase) && Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).GetType() == typeof(Players.HumanPlayer))
        {
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.A)))
            {
                Activate();
            }
        }
    }

    private static void Activate()
    {
        IsActive = true;

        Triggers.RegisterTrigger(new Trigger {
            Name = "Swarm manager",
            TriggerType = TriggerTypes.OnAbilityDirect,
            TriggerOwner = Phases.CurrentSubPhase.RequiredPlayer,
            EventHandler = ShowSwarmManagerWindow
        });

        Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate { });
    }

    private static void ShowSwarmManagerWindow(object sender, EventArgs e)
    {
        DirectionsMenu.ShowForAll(AssignManeuverToAllShips, AnyShipHasManeuver);
    }

    private static void AssignManeuverToAllShips(string maneuverCode)
    {
        foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships)
        {
            if (shipHolder.Value.HasManeuver(maneuverCode))
            {
                shipHolder.Value.SetAssignedManeuver(ShipMovementScript.MovementFromString(maneuverCode, shipHolder.Value));
                Roster.HighlightShipOff(shipHolder.Value);
            }
        }

        if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
        {
            UI.ShowNextButton();
            UI.HighlightNextButton();
        }

        IsActive = false;
        Triggers.FinishTrigger();
    }

    private static bool AnyShipHasManeuver(string maneuverCode)
    {
        bool result = false;

        foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships)
        {
            if (shipHolder.Value.HasManeuver(maneuverCode))
            {
                result = true;
                break;
            }
        }

        return result;
    }

}