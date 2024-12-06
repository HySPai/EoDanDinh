using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

        private void Update()
        {
            if(processEffect)
            {
                processEffect = false;
                TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
                effect.staminaDamage = 55;
                ProcessInstantEffect(effect);
            }
        }
    }

}
