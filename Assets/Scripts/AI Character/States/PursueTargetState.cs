using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // KIỂM TRA XEM CHÚNG TA CÓ ĐANG THỰC HIỆN HÀNH ĐỘNG KHÔNG (NẾU CÓ, KHÔNG LÀM GÌ CHO ĐẾN KHI HÀNH ĐỘNG HOÀN THÀNH)
            if (aiCharacter.isPerformingAction)
                return this;

            // KIỂM TRA XEM MỤC TIÊU CỦA CHÚNG TA CÓ NULL KHÔNG, NẾU CHÚNG TA KHÔNG CÓ MỤC TIÊU, TRỞ VỀ TRẠNG THÁI KHÔNG HOẠT ĐỘNG
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // ĐẢM BẢO NAVMESH AGENT CỦA CHÚNG TA ĐANG HOẠT ĐỘNG, NẾU KHÔNG BẬT
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // NẾU MỤC TIÊU CỦA CHÚNG TA RA NGOÀI GÓC NHÂN VẬT, HÃY QUAY ĐỂ ĐỐI MẶT VỚI CHÚNG

            if (aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV
                || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }

            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // TÙY CHỌN 01
            //if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStance.maximumEngagementDistance)
            //return SwitchState(aiCharacter, aiCharacter.combatStance);

            // TÙY CHỌN 02
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.combatStance);

            // NẾU KHÔNG THỂ ĐẾN ĐƯỢC MỤC TIÊU VÀ CHÚNG Ở XA, HÃY TRỞ VỀ NHÀ

            // THEO ĐUỔI MỤC TIÊU
            //TÙY CHỌN 01
            //aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);

            // TÙY CHỌN 02
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}
