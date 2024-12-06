using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;

        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManeger characterCombatManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [HideInInspector] public CharacterUIManager characterUIManager;

        [Header("Character Group")]
        public CharacterGroup characterGroup;

        [Header("Flags")]
        public bool isPerformingAction = false;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManeger>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterUIManager = GetComponent<CharacterUIManager>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);

            // NẾU NHÂN VẬT NÀY ĐƯỢC KIỂM SOÁT TỪ PHÍA CHÚNG TA, THÌ GÁN VỊ TRÍ MẠNG CỦA NÓ CHO VỊ TRÍ BIẾN ĐỔI CỦA CHÚNG TA
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // NẾU NHÂN VẬT NÀY ĐƯỢC KIỂM SOÁT TỪ NƠI KHÁC, THÌ GÁN VỊ TRÍ CỦA NÓ TẠI ĐÂY THEO VỊ TRÍ CỦA BIẾN ĐỔI MẠNG CỦA NÓ
            else
            {
                // Vị trí
                transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

                // Xoay
                transform.rotation = Quaternion.Slerp
                (transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
            characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // ĐẶT LẠI BẤT KỲ CỜ NÀO Ở ĐÂY CẦN ĐẶT LẠI
                // CHƯA CÓ GÌ

                // NẾU CHÚNG TA CHƯA ĐƯỢC ĐẶT LẠI, HÃY CHƠI MỘT HOẠT HÌNH CÁI CHẾT TRÊN KHÔNG

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }

            // CHƠI MỘT SỐ SFX CÁI CHẾT

            yield return new WaitForSeconds(5);

            // TRAO THƯỞNG CHO NGƯỜI CHƠI BẰNG RUNE

            // VÔ HIỆU HÓA NHÂN VẬT
        }

        public virtual void ReviveCharacter()
        {

        }

        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
            List<Collider> ignoreColliders = new List<Collider>();

            // THÊM TẤT CẢ CÁC COLLIDER NHÂN VẬT CÓ THỂ GÂY HẠI CỦA CHÚNG TA VÀO DANH SÁCH SẼ ĐƯỢC SỬ DỤNG ĐỂ BỎ QUA VA CHẠM
            foreach (var collider in damageableCharacterColliders)
            {
                ignoreColliders.Add(collider);
            }

            // THÊM CÁC COLLIDER ĐIỀU KHIỂN NHÂN VẬT CỦA CHÚNG TA VÀO DANH SÁCH SẼ ĐƯỢC SỬ DỤNG ĐỂ BỎ QUA VA CHẠM
            ignoreColliders.Add(characterControllerCollider);

            // ĐI QUA TẤT CẢ CÁC COLLIDER TRONG DANH SÁCH VÀ BỎ QUA VA CHẠM VỚI NHAU
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
    }
}
