using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        [Header("Blocking Absorptions")]
        public float blockingPhysicalAbsorption;
        public float blockingFireAbsorption;
        public float blockingMagicAbsorption;
        public float blockingLightningAbsorption;
        public float blockingHolyAbsorption;
        public float blockingStability;

        [Header("Armor Absorption")]
        public float armorPhysicalDamageAbsorption;
        public float armorMagicDamageAbsorption;
        public float armorFireDamageAbsorption;
        public float armorHolyDamageAbsorption;
        public float armorLightningDamageAbsorption;

        [Header("Armor Resistances")]
        public float armorImmunity;      // RESISTANCE TO ROT AND POISON
        public float armorRobustness;    // RESISTANCE TO BLEED AND FROST
        public float armorFocus;         // RESISTANCE TO MADNESS AND SLEEP
        public float armorVitality;      // RESISTANCE TO DEATH CURSE

        [Header("Poise")]
        public float totalPoiseDamage; // Chúng ta đã phải chịu bao nhiêu sát thương poise
        public float offensePoiseBonus; // Phần thưởng poise nhận được khi sử dụng vũ khí (vũ khí hạng nặng có phần thưởng lớn hơn nhiều)
        public float basePoiseDefense; // Phần thưởng poise nhận được từ áo giáp/bùa hộ mệnh, v.v.
        public float defaultPoiseResetTime = 8; // Thời gian cần thiết để sát thương poise được thiết lập lại (không được đánh trong thời gian đó nếu không nó sẽ được thiết lập lại)
        public float poiseResetTimer = 0; // Bộ đếm thời gian hiện tại để thiết lập lại poise

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            HandlePoiseResetTimer();
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            //  ONLY OWNERS CAN EDIT THEIR NETWORK VARAIBLES
            if (!character.IsOwner)
                return;

            //  WE DO NOT WANT TO REGENERATE STAMINA IF WE ARE USING IT
            if (character.characterNetworkManager.isSprinting.Value)
                return;

            if (character.isPerformingAction)
                return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            //  WE ONLY WANT TO RESET THE REGENERATION IF THE ACTION USED STAMINA
            //  WE DONT WANT TO RESET THE REGENERATION IF WE ARE ALREADY REGENERATING STAMINA
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }

        protected virtual void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else
            {
                totalPoiseDamage = 0;
            }
        }
    }
}
