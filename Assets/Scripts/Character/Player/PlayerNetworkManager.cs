﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SG
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;

        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Equipment")]
        public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Two Handing")]
        public NetworkVariable<int> currentWeaponBeingTwoHanded = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isTwoHandingWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isTwoHandingRightWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isTwoHandingLeftWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Armor")]
        public NetworkVariable<bool> isMale = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> headEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> bodyEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> legEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> handEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandedAction)
        {
            if (rightHandedAction)
            {
                isUsingLeftHand.Value = false;
                isUsingRightHand.Value = true;
            }
            else
            {
                isUsingRightHand.Value = false;
                isUsingLeftHand.Value = true;
            }
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }

        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }

        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentRightHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadRightWeapon();

            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(newID);
            }
        }

        public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadLeftWeapon();

            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon(newID);
            }
        }

        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

            // CHÚNG TA KHÔNG CẦN CHẠY MÃ NÀY NẾU CHÚNG TA LÀ CHỦ SỞ HỮU BỞI VÌ CHÚNG TA ĐÃ LÀM NHƯ VẬY Ở CỤC BỘ
            if (player.IsOwner)
                return;

            if (player.playerCombatManager.currentWeaponBeingUsed != null)
                player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator);
        }

        public override void OnIsBlockingChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsBlockingChanged(oldStatus, newStatus);

            if (IsOwner)
            {
                player.playerStatsManager.blockingPhysicalAbsorption = player.playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
                player.playerStatsManager.blockingMagicAbsorption = player.playerCombatManager.currentWeaponBeingUsed.magicBaseDamageAbsorption;
                player.playerStatsManager.blockingFireAbsorption = player.playerCombatManager.currentWeaponBeingUsed.fireBaseDamageAbsorption;
                player.playerStatsManager.blockingLightningAbsorption = player.playerCombatManager.currentWeaponBeingUsed.lightningBaseDamageAbsorption;
                player.playerStatsManager.blockingHolyAbsorption = player.playerCombatManager.currentWeaponBeingUsed.holyBaseDamageAbsorption;
                player.playerStatsManager.blockingStability = player.playerCombatManager.currentWeaponBeingUsed.stability;
            }
        }

        public void OnIsTwoHandingWeaponChanged(bool oldStatus, bool newStatus)
        {
            if (!isTwoHandingWeapon.Value)
            {
                if (IsOwner)
                {
                    isTwoHandingLeftWeapon.Value = false;
                    isTwoHandingRightWeapon.Value = false;
                }

                player.playerEquipmentManager.UnTwoHandWeapon();
                player.playerEffectsManager.RemoveStaticEffect(WorldCharacterEffectsManager.instance.twoHandingEffect.staticEffectID);
            }
            else
            {
                StaticCharacterEffect twoHandEffect = Instantiate(WorldCharacterEffectsManager.instance.twoHandingEffect);
                player.playerEffectsManager.AddStaticEffect(twoHandEffect);
            }

            player.animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon.Value);
        }

        public void OnIsTwoHandingRightWeaponChanged(bool oldStatus, bool newStatus)
        {
            if (!isTwoHandingRightWeapon.Value)
                return;

            if (IsOwner)
            {
                currentWeaponBeingTwoHanded.Value = currentRightHandWeaponID.Value;
                isTwoHandingWeapon.Value = true;
            }

            player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentRightHandWeapon;
            player.playerEquipmentManager.TwoHandRightWeapon();
        }

        public void OnIsTwoHandingLeftWeaponChanged(bool oldStatus, bool newStatus)
        {
            if (!isTwoHandingLeftWeapon.Value)
                return;

            if (IsOwner)
            {
                currentWeaponBeingTwoHanded.Value = currentLeftHandWeaponID.Value;
                isTwoHandingWeapon.Value = true;
            }

            player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentLeftHandWeapon;
            player.playerEquipmentManager.TwoHandLeftWeapon();
        }

        public void OnHeadEquipmentChanged(int oldValue, int newValue)
        {
            if (IsOwner)
                return;

            HeadEquipmentItem equipment = WorldItemDatabase.Instance.GetHeadEquipmentByID(headEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadHeadEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadHeadEquipment(null);
            }
        }

        public void OnBodyEquipmentChanged(int oldValue, int newValue)
        {
            // CHÚNG TA ĐÃ CHẠY LOGIC Ở PHÍA CHỦ SỞ HỮU, VÌ VẬY KHÔNG CÓ LỢI ÍCH GÌ KHI CHẠY LẠI NỮA
            if (IsOwner)
                return;

            BodyEquipmentItem equipment = WorldItemDatabase.Instance.GetBodyEquipmentByID(bodyEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadBodyEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadBodyEquipment(null);
            }
        }

        public void OnLegEquipmentChanged(int oldValue, int newValue)
        {
            if (IsOwner)
                return;

            LegEquipmentItem equipment = WorldItemDatabase.Instance.GetLegEquipmentByID(legEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadLegEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadLegEquipment(null);
            }
        }

        public void OnHandEquipmentChanged(int oldValue, int newValue)
        {
            if (IsOwner)
                return;

            HandEquipmentItem equipment = WorldItemDatabase.Instance.GetHandEquipmentByID(headEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadHandEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadHandEquipment(null);
            }
        }

        public void OnIsMaleChanged(bool oldStatus, bool newStatus)
        {
            player.playerBodyManager.ToggleBodyType(isMale.Value);
        }

        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
        {
            if (IsServer)
            {
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }

        [ClientRpc]
        private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformWeaponBasedAction(actionID, weaponID);
            }
        }

        private void PerformWeaponBasedAction(int actionID, int weaponID)
        {
            WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

            if (weaponAction != null)
            {
                weaponAction.AttemptToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(weaponID));
            }
            else
            {
                Debug.LogError("HÀNH ĐỘNG LÀ NULL, KHÔNG THỂ THỰC HIỆN");
            }
        }
    }
}
