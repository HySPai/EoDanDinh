using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class PlayerInputManager : MonoBehaviour
    {
        //  INPUT CONTROLS
        private PlayerControls playerControls;

        //  SINGLETON
        public static PlayerInputManager instance;

        //  LOCAL PLAYER
        public PlayerManager player;

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 camera_Input;
        public float cameraVertical_Input;
        public float cameraHorizontal_Input;

        [Header("Lock On Input")]
        [SerializeField] bool lockOn_Input;
        [SerializeField] bool lockOn_Left_Input;
        [SerializeField] bool lockOn_Right_Input;
        private Coroutine lockOnCoroutine;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;

        [Header("Player Action Input")]
        [SerializeField] bool dodge_Input = false;
        [SerializeField] bool sprint_Input = false;
        [SerializeField] bool jump_Input = false;
        [SerializeField] bool switch_Right_Weapon_Input = false;
        [SerializeField] bool switch_Left_Weapon_Input = false;
        [SerializeField] bool interaction_Input = false;

        [Header("Bumper Inputs")]
        [SerializeField] bool RB_Input = false;
        [SerializeField] bool LB_Input = false;

        [Header("Trigger Inputs")]
        [SerializeField] bool RT_Input = false;
        [SerializeField] bool Hold_RT_Input = false;

        [Header("Two Hand Inputs")]
        [SerializeField] bool two_Hand_Input = false;
        [SerializeField] bool two_Hand_Right_Weapon_Input = false;
        [SerializeField] bool two_Hand_Left_Weapon_Input = false;

        [Header("QUED INPUTS")]
        [SerializeField] private bool input_Que_Is_Active = false;
        [SerializeField] float default_Que_Input_Time = 0.35f;
        [SerializeField] float que_Input_Timer = 0;
        [SerializeField] bool que_RB_Input = false;
        [SerializeField] bool que_RT_Input = false;

        [Header("UI INPUTS")]
        [SerializeField] bool openCharacterMenuInput = false;
        [SerializeField] bool closeMenuInput = false;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {
                instance.enabled = false;

                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => camera_Input = i.ReadValue<Vector2>();

                //  ACTIONS
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
                playerControls.PlayerActions.Interact.performed += i => interaction_Input = true;

                //  BUMPERS
                playerControls.PlayerActions.RB.performed += i => RB_Input = true;
                playerControls.PlayerActions.LB.performed += i => LB_Input = true;
                playerControls.PlayerActions.LB.canceled += i => player.playerNetworkManager.isBlocking.Value = false;

                //  TRIGGERS
                playerControls.PlayerActions.RT.performed += i => RT_Input = true;
                playerControls.PlayerActions.HoldRT.performed += i => Hold_RT_Input = true;
                playerControls.PlayerActions.HoldRT.canceled += i => Hold_RT_Input = false;

                //  TWO HAND
                playerControls.PlayerActions.TwoHandWeapon.performed += i => two_Hand_Input = true;
                playerControls.PlayerActions.TwoHandWeapon.canceled += i => two_Hand_Input = false;
                playerControls.PlayerActions.TwoHandRightWeapon.performed += i => two_Hand_Right_Weapon_Input = true;
                playerControls.PlayerActions.TwoHandRightWeapon.canceled += i => two_Hand_Right_Weapon_Input = false;
                playerControls.PlayerActions.TwoHandLeftWeapon.performed += i => two_Hand_Left_Weapon_Input = true;
                playerControls.PlayerActions.TwoHandLeftWeapon.canceled += i => two_Hand_Left_Weapon_Input = false;

                //  LOCK ON
                playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;

                //  HOLDING THE INPUT, SETS THE BOOL TO TRUE
                playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
                //  RELEASING THE INPUT, SETS THE BOOL TO FALSE
                playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;

                //  QUED INPUTS
                playerControls.PlayerActions.QueRB.performed += i => QueInput(ref que_RB_Input);
                playerControls.PlayerActions.QueRT.performed += i => QueInput(ref que_RT_Input);

                //  UI INPUTS
                playerControls.PlayerActions.Dodge.performed += i => closeMenuInput = true;
                playerControls.PlayerActions.OpenCharacterMenu.performed += i => openCharacterMenuInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            if (player == null)
                return;
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandleTwoHandInput();
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleRBInput();
            HandleLBInput();
            HandleRTInput();
            HandleChargeRTInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();
            HandleQuedInputs();
            HandleInteractionInput();
            HandleCloseUIInput();
            HandleOpenCharacterMenuInput();
        }

        //  TWO HAND
        private void HandleTwoHandInput()
        {
            if (!two_Hand_Input)
                return;

            if (two_Hand_Right_Weapon_Input)
            {
                RB_Input = false;
                two_Hand_Right_Weapon_Input = false;
                player.playerNetworkManager.isBlocking.Value = false;

                if (player.playerNetworkManager.isTwoHandingWeapon.Value)
                {
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                    return;
                }
                else
                {
                    player.playerNetworkManager.isTwoHandingRightWeapon.Value = true;
                    return;
                }
            }
            else if (two_Hand_Left_Weapon_Input)
            {
                LB_Input = false;
                two_Hand_Left_Weapon_Input = false;
                player.playerNetworkManager.isBlocking.Value = false;

                if (player.playerNetworkManager.isTwoHandingWeapon.Value)
                {
                    player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                    return;
                }
                else
                {
                    player.playerNetworkManager.isTwoHandingLeftWeapon.Value = true;
                    return;
                }
            }
        }

        //  LOCK ON
        private void HandleLockOnInput()
        {
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false;
                }

                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }


            if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;
                PlayerCamera.instance.ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
                return;
            }

            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;


                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
            }
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if (lockOn_Left_Input)
            {
                lockOn_Left_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }

            if (lockOn_Right_Input)
            {
                lockOn_Right_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }

        //  MOVEMENT
        private void HandlePlayerMovementInput()
        {
            vertical_Input = movementInput.y;
            horizontal_Input = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            //  WE CLAMP THE VALUES, SO THEY ARE 0, 0.5 OR 1 (OPTIONAL)
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }


            if (player == null)
                return;

            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
            }
            else
            {
                player.playerNetworkManager.isMoving.Value = false;
            }

            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }
        }

        private void HandleCameraMovementInput()
        {
            cameraVertical_Input = camera_Input.y;
            cameraHorizontal_Input = camera_Input.x;
        }

        //  ACTION
        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprint_Input)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;

                if (PlayerUIManager.instance.menuWindowIsOpen)
                    return;

                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleRBInput()
        {
            if (two_Hand_Input)
                return;

            if (RB_Input)
            {
                RB_Input = false;

                player.playerNetworkManager.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleLBInput()
        {
            if (two_Hand_Input)
                return;

            if (LB_Input)
            {
                LB_Input = false;

                player.playerNetworkManager.SetCharacterActionHand(false);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentLeftHandWeapon.oh_LB_Action, player.playerInventoryManager.currentLeftHandWeapon);
            }
        }

        private void HandleRTInput()
        {
            if (RT_Input)
            {
                RT_Input = false;

                player.playerNetworkManager.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RT_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleChargeRTInput()
        {
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = Hold_RT_Input;
                }
            }
        }

        private void HandleSwitchRightWeaponInput()
        {
            if (switch_Right_Weapon_Input)
            {
                switch_Right_Weapon_Input = false;
                player.playerEquipmentManager.SwitchRightWeapon();
            }
        }

        private void HandleSwitchLeftWeaponInput()
        {
            if (switch_Left_Weapon_Input)
            {
                switch_Left_Weapon_Input = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }

        private void HandleInteractionInput()
        {
            if (interaction_Input)
            {
                interaction_Input = false;

                player.playerInteractionManager.Interact();
            }
        }

        private void QueInput(ref bool quedInput)   //  PASSING A REFERENCE MEANS WE PASS A SPECIFIC BOOL, AND NOT THE VALUE OF THAT BOOL (TRUE OR FALSE)
        {
            que_RB_Input = false;
            que_RT_Input = false;
            //que_LB_Input = false;
            //que_LT_Input = false;

            //  CHECK FOR UI WINDOW BEING OPEN, IF ITS OPEN RETURN

            if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
            {
                quedInput = true;
                que_Input_Timer = default_Que_Input_Time;
                input_Que_Is_Active = true;
            }
        }

        private void ProcessQuedInput()
        {
            if (player.isDead.Value)
                return;

            if (que_RB_Input)
                RB_Input = true;

            if (que_RT_Input)
                RT_Input = true;
        }

        private void HandleQuedInputs()
        {
            if (input_Que_Is_Active)
            {
                if (que_Input_Timer > 0)
                {
                    que_Input_Timer -= Time.deltaTime;
                    ProcessQuedInput();
                }
                else
                {
                    //  RESET ALL QUED INPUTS
                    que_RB_Input = false;
                    que_RT_Input = false;

                    input_Que_Is_Active = false;
                    que_Input_Timer = 0;
                }
            }
        }

        private void HandleOpenCharacterMenuInput()
        {
            if (openCharacterMenuInput)
            {
                openCharacterMenuInput = false;

                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
                PlayerUIManager.instance.CloseAllMenuWindows();
                PlayerUIManager.instance.playerUICharacterMenuManager.OpenCharacterMenu();
            }
        }

        private void HandleCloseUIInput()
        {
            if (closeMenuInput)
            {
                closeMenuInput = false;

                if (PlayerUIManager.instance.menuWindowIsOpen)
                {
                    PlayerUIManager.instance.CloseAllMenuWindows();
                }
            }
        }
    }
}
