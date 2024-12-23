using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            //  WHY CALCULATE THESE HERE?
            //  WHEN WE MAKE A CHARACTER CREATION MENU, AND SET THE STATS DEPENDING ON THE CLASS, THIS WILL BE CALCULATED THERE
            //  UNTIL THEN HOWEVER, STATS ARE NEVER CALCULATED, SO WE DO IT HERE ON START, IF A SAVE FILE EXISTS THEY WILL BE OVER WRITTEN WHEN LOADING INTO A SCENE
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }

        public void CalculateTotalArmorAbsorption()
        {
            //  RESET ALL VALUES TO 0
            armorPhysicalDamageAbsorption = 0;
            armorMagicDamageAbsorption = 0;
            armorFireDamageAbsorption = 0;
            armorHolyDamageAbsorption = 0;
            armorLightningDamageAbsorption = 0;

            armorRobustness = 0;
            armorVitality = 0;
            armorImmunity = 0;
            armorFocus = 0;

            basePoiseDefense = 0;

            //  HEAD EQUIPMENT
            if (player.playerInventoryManager.headEquipment != null)
            {
                //  DAMAGE RESISTANCE
                armorPhysicalDamageAbsorption += player.playerInventoryManager.headEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.headEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.headEquipment.fireDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.headEquipment.holyDamageAbsorption;
                armorLightningDamageAbsorption += player.playerInventoryManager.headEquipment.lightningDamageAbsorption;

                //  STATUS EFFECT RESISTANCE
                armorRobustness += player.playerInventoryManager.headEquipment.robustness;
                armorVitality += player.playerInventoryManager.headEquipment.vitality;
                armorImmunity += player.playerInventoryManager.headEquipment.immunity;
                armorFocus += player.playerInventoryManager.headEquipment.focus;

                //  POISE
                basePoiseDefense += player.playerInventoryManager.headEquipment.poise;
            }
            //  BODY EQUIPMENT
            if (player.playerInventoryManager.bodyEquipment != null)
            {
                //  DAMAGE RESISTANCE
                armorPhysicalDamageAbsorption += player.playerInventoryManager.bodyEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.bodyEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.bodyEquipment.fireDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.bodyEquipment.holyDamageAbsorption;
                armorLightningDamageAbsorption += player.playerInventoryManager.bodyEquipment.lightningDamageAbsorption;

                //  STATUS EFFECT RESISTANCE
                armorRobustness += player.playerInventoryManager.bodyEquipment.robustness;
                armorVitality += player.playerInventoryManager.bodyEquipment.vitality;
                armorImmunity += player.playerInventoryManager.bodyEquipment.immunity;
                armorFocus += player.playerInventoryManager.bodyEquipment.focus;

                //  POISE
                basePoiseDefense += player.playerInventoryManager.bodyEquipment.poise;
            }
            //  LEG EQUIPMENT
            if (player.playerInventoryManager.legEquipment != null)
            {
                //  DAMAGE RESISTANCE
                armorPhysicalDamageAbsorption += player.playerInventoryManager.legEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.legEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.legEquipment.fireDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.legEquipment.holyDamageAbsorption;
                armorLightningDamageAbsorption += player.playerInventoryManager.legEquipment.lightningDamageAbsorption;

                //  STATUS EFFECT RESISTANCE
                armorRobustness += player.playerInventoryManager.legEquipment.robustness;
                armorVitality += player.playerInventoryManager.legEquipment.vitality;
                armorImmunity += player.playerInventoryManager.legEquipment.immunity;
                armorFocus += player.playerInventoryManager.legEquipment.focus;

                //  POISE
                basePoiseDefense += player.playerInventoryManager.legEquipment.poise;
            }
            //  HAND EQUIPMENT
            if (player.playerInventoryManager.handEquipment != null)
            {
                //  DAMAGE RESISTANCE
                armorPhysicalDamageAbsorption += player.playerInventoryManager.handEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.handEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.handEquipment.fireDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.handEquipment.holyDamageAbsorption;
                armorLightningDamageAbsorption += player.playerInventoryManager.handEquipment.lightningDamageAbsorption;

                //  STATUS EFFECT RESISTANCE
                armorRobustness += player.playerInventoryManager.handEquipment.robustness;
                armorVitality += player.playerInventoryManager.handEquipment.vitality;
                armorImmunity += player.playerInventoryManager.handEquipment.immunity;
                armorFocus += player.playerInventoryManager.handEquipment.focus;

                //  POISE
                basePoiseDefense += player.playerInventoryManager.handEquipment.poise;
            }
        }
    }
}
