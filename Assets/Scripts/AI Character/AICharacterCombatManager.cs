using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AICharacterCombatManager : CharacterCombatManeger
    {
        protected AICharacterManager aiCharacter;

        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0;

        [Header("Pivot")]
        public bool enablePivot = true;

        [Header("Target Information")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        public float minimumFOV = -35;
        public float maximumFOV = 35;

        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }

        public virtual void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;

            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter == null)
                    continue;

                if (targetCharacter == aiCharacter)
                    continue;

                if (targetCharacter.isDead.Value)
                    continue;

                // TÔI CÓ THỂ TẤN CÔNG NHÂN VẬT NÀY KHÔNG, NẾU CÓ, HÃY BIẾN CHÚNG THÀNH MỤC TIÊU CỦA TÔI
                if (WorldUtilityManager.Instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    // NẾU TÌM THẤY MỤC TIÊU TIỀM NĂNG, NÓ PHẢI Ở TRƯỚC MÌNH
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // CUỐI CÙNG, CHÚNG TA KIỂM TRA CÁC KHỐI MÔI TRƯỜNG
                        if (Physics.Linecast(
                            aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.Instance.GetEnviroLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                        }
                        else
                        {
                            targetsDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                            if (enablePivot)
                                PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // PHÁT ANIMATION TRỤC TÙY THUỘC VÀO GÓC NHÌN CỦA MỤC TIÊU
            if (aiCharacter.isPerformingAction)
                return;

            if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if (viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if (viewableAngle <= -146 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;

            if (!aiCharacter.characterLocomotionManager.canRotate)
                return;

            if (!aiCharacter.isPerformingAction)
                return;

            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);

        }

        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}
