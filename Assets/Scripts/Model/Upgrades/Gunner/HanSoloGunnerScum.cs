using Upgrade;
using Tokens;
using SquadBuilderNS;
using Abilities.SecondEdition;
using System.Collections.Generic;
using RuleSets;

namespace UpgradesList
{
    public class HanSoloGunnerScum : GenericUpgrade, ISecondEditionUpgrade
    {
        public HanSoloGunnerScum() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Han Solo (Scum)";
            Cost = 4;
            isUnique = true;
            
            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new HanSoloGunnerAbilityScum());

            SEImageNumber = 163;
        }
    public void AdaptUpgradeToSecondEdition()
        {

                //Nothing to do, already second edition upgrade
        }
        
    public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloGunnerAbilityScum : GenericAbility
    {
        // Before you engage, you may perform a red (Focus) action.
            
        public override void ActivateAbility() 
            {
                HostShip.OnCombatActivationGlobal += CheckAbility;
                Phases.Events.OnRoundEnd += RemoveHanSoloGunnerScumAbility;
            }

        public override void DeactivateAbility()
            {
  
            }
            
        private void CheckAbility(HostShip.OnCombatActivationGlobal)
            {
                if (HostShip.Tokens.HasToken(typeof(StressToken))) return;
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskAbility);
            }
        private void AskAbility(object sender, System.EventArgs e)
            {
                Messages.ShowInfo("Do you want to use Han Solo Gunner (Scum) Ability?");
                AskToUseAbility(UseAbility, DontUseAbility);
            }
        private void UseAbility(object sender, System.EventArgs e)
            {
                if (!HostShip.Tokens.HasToken(typeof(StressToken)))
                    {
                        HostShip.Tokens.AssignToken(typeof(StressToken), AssignConditionToActivatedShip);
                        HostShip.Tokens.Assigntoken(typeof(FocusToken), AssignConditionToActivatedShip);
                    }
                else
                    {
                        Messages.ShowErrorToHuman("Han Solo Gunner (Scum): Cannot use ability - already has stress");
                        Triggers.FinishTrigger();
                    }
            }
        private void DontUseAbility(object sender, System.EventArgs e)
            {          
                DecisionSubPhase.ConfirmDecision();
            }
    }
}