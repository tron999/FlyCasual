using System;
using System.Linq;
using ActionsList;
using RuleSets;
using Ship;

namespace Ship
{
    namespace TIEBomber
    {
        public class Deathfire : TIEBomber, ISecondEditionPilot
        {
            public Deathfire() : base()
            {
                PilotName = "\"Deathfire\"";
                PilotSkill = 3;
                Cost = 17;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.DeathfireAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 32;

                PilotAbilities.RemoveAll(a => a is Abilities.DeathfireAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.DeathfireAbilitySE());

                SEImageNumber = 110;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeathfireAbilitySE : GenericAbility
    {
        private bool DestructionWasPreventedByAbility;

        public override void ActivateAbility()
        {
            HostShip.OnReadyToBeDestroyed += UseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnReadyToBeDestroyed -= UseAbility;
        }

        private void UseAbility(GenericShip ship)
        {
            if (DestructionWasPreventedByAbility) return;

            DestructionWasPreventedByAbility = true;
            HostShip.PreventDestruction = true;

            if (Combat.AttackStep != CombatStep.None)
            {
                HostShip.OnAttackFinish += RegisterBothAbilities;
            }
        }

        private void RegisterBothAbilities(GenericShip ship)
        {
            Messages.ShowInfo("\"Deathfire\": You may preform an attack and drop or launch a device.");

            HostShip.PreventDestruction = false;

            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerfromAttack, name:"Perform an attack");
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, DropOrLaunchDevice, name:"Drop or launch a device");
        }

        private void PerfromAttack(object sender, System.EventArgs e)
        {
            //TODO

            Triggers.FinishTrigger();
        }

        private void DropOrLaunchDevice(object sender, System.EventArgs e)
        {
            //TODO

            Triggers.FinishTrigger();
        }
    }
}

namespace Abilities
{
    public class DeathfireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckDeathfireAbility;
            HostShip.OnActionIsPerformed += CheckDeathfireAbility;            
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckDeathfireAbility;
            HostShip.OnActionIsPerformed -= CheckDeathfireAbility;
        }

        private void CheckDeathfireAbility(GenericAction action)
        {
            if (!IsAbilityUsed)
            {
                SetIsAbilityIsUsed(HostShip);
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, DeathfireEffect);
            }
        }

        private void CheckDeathfireAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                SetIsAbilityIsUsed(HostShip);
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, DeathfireEffect);
            }
        }

        private void DeathfireEffect(object sender, EventArgs e)
        {
            var actions = HostShip.GetAvailableActions()
                .Where(action => action is BombDropAction)
                .ToList();

            HostShip.AskPerformFreeAction(actions, () =>
            {
                ClearIsAbilityUsedFlag();
                Triggers.FinishTrigger();                
            });
        }        
    }
}
