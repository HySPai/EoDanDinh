﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        [HideInInspector] public PlayerInteractionManager playerInteractionManager;
        [HideInInspector] public PlayerEffectsManager playerEffectsManager;
        [HideInInspector] public PlayerBodyManager playerBodyManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerInteractionManager = GetComponent<PlayerInteractionManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            playerBodyManager = GetComponent<PlayerBodyManager>();
        }

        protected override void Update()
        {
            base.Update();

            // NẾU CHÚNG TA KHÔNG SỞ HỮU GAMEOBJECT NÀY, CHÚNG TA KHÔNG KIỂM SOÁT HOẶC CHỈNH SỬA NÓ
            if (!IsOwner)
                return;

            // XỬ LÝ CHUYỂN ĐỘNG
            playerLocomotionManager.HandleAllMovement();

            // SỨC BỀN TĨNH PHỤC
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;

            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            // NẾU ĐÂY LÀ ĐỐI TƯỢNG NGƯỜI CHƠI DO KHÁCH HÀNG NÀY SỞ HỮU
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // CẬP NHẬT TỔNG SỐ LƯỢNG MÁU HOẶC SỨC BỀN KHI THỐNG KÊ LIÊN KẾT VỚI MỘT TRONG HAI THAY ĐỔI
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // CẬP NHẬT THANH THỐNG KÊ GIAO DIỆN KHI THỐNG KÊ THAY ĐỔI (SỨC BỀN HOẶC SỨC BỀN)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;

                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            // CHỈ CẬP NHẬT THANH HP NỔI NẾU NHÂN VẬT NÀY KHÔNG PHẢI NHÂN VẬT CỦA NGƯỜI CHƠI CỤ THỂ (BẠN KHÔNG MUỐN THẤY THANH HP NỔI TRÊN ĐẦU CỦA CHÍNH BẠN)
            if (!IsOwner)
                characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;

            //  BODY TYPE
            playerNetworkManager.isMale.OnValueChanged += playerNetworkManager.OnIsMaleChanged;

            //  STATS
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

            //  LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

            //  EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.isBlocking.OnValueChanged += playerNetworkManager.OnIsBlockingChanged;
            playerNetworkManager.headEquipmentID.OnValueChanged += playerNetworkManager.OnHeadEquipmentChanged;
            playerNetworkManager.bodyEquipmentID.OnValueChanged += playerNetworkManager.OnBodyEquipmentChanged;
            playerNetworkManager.legEquipmentID.OnValueChanged += playerNetworkManager.OnLegEquipmentChanged;
            playerNetworkManager.handEquipmentID.OnValueChanged += playerNetworkManager.OnHandEquipmentChanged;

            //  TWO HAND
            playerNetworkManager.isTwoHandingWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingWeaponChanged;
            playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
            playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

            //  FLAGS
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

            // KHI KẾT NỐI, NẾU CHÚNG TA LÀ CHỦ SỞ HỮU NHÂN VẬT NÀY, NHƯNG CHÚNG TA KHÔNG PHẢI LÀ MÁY CHỦ, HÃY TẢI LẠI DỮ LIỆU NHÂN VẬT CỦA CHÚNG TA VÀO NHÂN VẬT MỚI ĐƯỢC TẠO RA NÀY
            // CHÚNG TA KHÔNG CHẠY NÀY NẾU CHÚNG TA LÀ MÁY CHỦ, BỞI VÌ CHÚNG TA LÀ MÁY CHỦ, CHÚNG ĐÃ ĐƯỢC TẢI VÀO VÀ KHÔNG CẦN TẢI LẠI DỮ LIỆU CỦA CHÚNG TA
            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

            // NẾU ĐÂY LÀ ĐỐI TƯỢNG NGƯỜI CHƠI DO KHÁCH HÀNG NÀY SỞ HỮU
            if (IsOwner)
            {
                // CẬP NHẬT TỔNG SỐ LƯỢNG MÁU HOẶC SỨC BỀN KHI THỐNG KÊ LIÊN KẾT VỚI MỘT TRONG HAI THAY ĐỔI
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                // CẬP NHẬT THANH THỐNG KÊ GIAO DIỆN KHI THỐNG KÊ THAY ĐỔI (SỨC BỀN HOẶC SỨC BỀN)
                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            }

            if (!IsOwner)
                characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHPChanged;

            //  BODY TYPE
            playerNetworkManager.isMale.OnValueChanged -= playerNetworkManager.OnIsMaleChanged;

            //  STATS
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

            //  LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

            //  EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.headEquipmentID.OnValueChanged -= playerNetworkManager.OnHeadEquipmentChanged;
            playerNetworkManager.bodyEquipmentID.OnValueChanged -= playerNetworkManager.OnBodyEquipmentChanged;
            playerNetworkManager.legEquipmentID.OnValueChanged -= playerNetworkManager.OnLegEquipmentChanged;
            playerNetworkManager.handEquipmentID.OnValueChanged -= playerNetworkManager.OnHandEquipmentChanged;

            //  TWO HAND
            playerNetworkManager.isTwoHandingWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingWeaponChanged;
            playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
            playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

            //  FLAGS
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
        }

        private void OnClientConnectedCallback(ulong clientID)
        {
            WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

            // NẾU CHÚNG TA LÀ MÁY CHỦ, CHÚNG TA LÀ MÁY CHỦ, VẬY CHÚNG TA KHÔNG CẦN TẢI NGƯỜI CHƠI ĐỂ ĐỒNG BỘ HỌ
            // BẠN CHỈ CẦN TẢI TRANG BỊ CỦA NGƯỜI CHƠI KHÁC ĐỂ ĐỒNG BỘ NẾU BẠN THAM GIA MỘT TRÒ CHƠI ĐÃ HOẠT ĐỘNG MÀ BẠN KHÔNG CÓ MẶT
            if (!IsServer && IsOwner)
            {
                foreach (var player in WorldGameSessionManager.instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }

            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if (IsOwner)
            {
                isDead.Value = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                // KHÔI PHỤC ĐIỂM TẬP TRUNG

                // CHƠI HIỆU ỨNG TÁI SINH
                playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.isMale = playerNetworkManager.isMale.Value;
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;

            //  EQUIPMENT
            currentCharacterData.headEquipment = playerNetworkManager.headEquipmentID.Value;
            currentCharacterData.bodyEquipment = playerNetworkManager.bodyEquipmentID.Value;
            currentCharacterData.legEquipment = playerNetworkManager.legEquipmentID.Value;
            currentCharacterData.handEquipment = playerNetworkManager.handEquipmentID.Value;

            currentCharacterData.rightWeaponIndex = playerInventoryManager.rightHandWeaponIndex;
            currentCharacterData.rightWeapon01 = playerInventoryManager.weaponsInRightHandSlots[0].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)
            currentCharacterData.rightWeapon02 = playerInventoryManager.weaponsInRightHandSlots[1].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)
            currentCharacterData.rightWeapon03 = playerInventoryManager.weaponsInRightHandSlots[2].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)

            currentCharacterData.leftWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
            currentCharacterData.leftWeapon01 = playerInventoryManager.weaponsInLeftHandSlots[0].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)
            currentCharacterData.leftWeapon02 = playerInventoryManager.weaponsInLeftHandSlots[1].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)
            currentCharacterData.leftWeapon03 = playerInventoryManager.weaponsInLeftHandSlots[2].itemID; // GIÁ TRỊ NÀY KHÔNG BAO GIỜ ĐƯỢC LÀ NULL (giá trị mặc định luôn là không có vũ khí)
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            playerNetworkManager.isMale.Value = currentCharacterData.isMale;
            playerBodyManager.ToggleBodyType(currentCharacterData.isMale); // TOGGLE TRONG TRƯỜNG HỢP GIÁ TRỊ GIỐNG NHƯ GIÁ TRỊ MẶC ĐỊNH (ONVALUECHANGED CHỈ HOẠT ĐỘNG KHI GIÁ TRỊ ĐƯỢC THAY ĐỔI)
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;

            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            // ĐIỀU NÀY SẼ ĐƯỢC DI CHUYỂN KHI THÊM LƯU VÀ TẢI
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);

            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

            // THIẾT BỊ
            if(WorldItemDatabase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment))
            {
                HeadEquipmentItem headEquipment = Instantiate(WorldItemDatabase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment));
                playerInventoryManager.headEquipment = headEquipment;
            }
            else
            {
                playerInventoryManager.headEquipment = null;
            }

            if (WorldItemDatabase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment))
            {
                BodyEquipmentItem bodyEquipment = Instantiate(WorldItemDatabase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment));
                playerInventoryManager.bodyEquipment = bodyEquipment;
            }
            else
            {
                playerInventoryManager.bodyEquipment = null;
            }

            if (WorldItemDatabase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment))
            {
                HandEquipmentItem handEquipment = Instantiate(WorldItemDatabase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment));
                playerInventoryManager.handEquipment = handEquipment;
            }
            else
            {
                playerInventoryManager.handEquipment = null;
            }

            if (WorldItemDatabase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment))
            {
                LegEquipmentItem legEquipment = Instantiate(WorldItemDatabase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment));
                playerInventoryManager.legEquipment = legEquipment;
            }
            else
            {
                playerInventoryManager.legEquipment = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon01))
            {
                WeaponItem rightWeapon01 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon01));
                playerInventoryManager.weaponsInRightHandSlots[0] = rightWeapon01;
            }
            else
            {
                playerInventoryManager.weaponsInRightHandSlots[0] = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon02))
            {
                WeaponItem rightWeapon02 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon02));
                playerInventoryManager.weaponsInRightHandSlots[1] = rightWeapon02;
            }
            else
            {
                playerInventoryManager.weaponsInRightHandSlots[1] = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon03))
            {
                WeaponItem rightWeapon03 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon03));
                playerInventoryManager.weaponsInRightHandSlots[2] = rightWeapon03;
            }
            else
            {
                playerInventoryManager.weaponsInRightHandSlots[2] = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon01))
            {
                WeaponItem leftWeapon01 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon01));
                playerInventoryManager.weaponsInLeftHandSlots[0] = leftWeapon01;
            }
            else
            {
                playerInventoryManager.weaponsInLeftHandSlots[0] = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon02))
            {
                WeaponItem leftWeapon02 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon02));
                playerInventoryManager.weaponsInLeftHandSlots[1] = leftWeapon02;
            }
            else
            {
                playerInventoryManager.weaponsInLeftHandSlots[1] = null;
            }

            if (WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon03))
            {
                WeaponItem leftWeapon03 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon03));
                playerInventoryManager.weaponsInLeftHandSlots[2] = leftWeapon03;
            }
            else
            {
                playerInventoryManager.weaponsInLeftHandSlots[2] = null;
            }

            playerEquipmentManager.EquipArmor();

            playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightWeaponIndex;
            playerNetworkManager.currentRightHandWeaponID.Value = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightWeaponIndex].itemID;
            playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftWeaponIndex;
            playerNetworkManager.currentLeftHandWeaponID.Value = playerInventoryManager.weaponsInLeftHandSlots[currentCharacterData.leftWeaponIndex].itemID;
        }

        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            //  SYNC BODY TYPE
            playerNetworkManager.OnIsMaleChanged(false, playerNetworkManager.isMale.Value);

            //  SYNC WEAPONS
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

            //  SYNC ARMOR
            playerNetworkManager.OnHeadEquipmentChanged(0, playerNetworkManager.headEquipmentID.Value);
            playerNetworkManager.OnBodyEquipmentChanged(0, playerNetworkManager.bodyEquipmentID.Value);
            playerNetworkManager.OnLegEquipmentChanged(0, playerNetworkManager.legEquipmentID.Value);
            playerNetworkManager.OnHandEquipmentChanged(0, playerNetworkManager.handEquipmentID.Value);

            //  SYNC TWO HAND STATUS
            playerNetworkManager.OnIsTwoHandingRightWeaponChanged(false, playerNetworkManager.isTwoHandingRightWeapon.Value);
            playerNetworkManager.OnIsTwoHandingLeftWeaponChanged(false, playerNetworkManager.isTwoHandingLeftWeapon.Value);

            //  SYNC BLOCK STATUS
            playerNetworkManager.OnIsBlockingChanged(false, playerNetworkManager.isBlocking.Value);

            //  ARMOR

            //  LOCK ON
            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }

    }
}
