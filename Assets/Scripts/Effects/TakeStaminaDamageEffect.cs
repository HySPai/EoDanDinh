using SG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effect/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;
        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            if (character.IsOwner)
            {
                Debug.Log("Character is taking " + staminaDamage + " stamina damage");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}