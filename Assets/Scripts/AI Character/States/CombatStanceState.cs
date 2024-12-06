using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks;    // Danh sách tất cả các đòn tấn công có thể mà nhân vật này có thể thực hiện
        [SerializeField] protected List<AICharacterAttackAction> potentialAttacks; // Tất cả các đòn tấn công có thể thực hiện trong tình huống này (dựa trên góc, khoảng cách, v.v.)
        [SerializeField] private AICharacterAttackAction choosenAttack;
        [SerializeField] private AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false; // Nếu nhân vật có thể thực hiện đòn tấn công kết hợp, sau đòn tấn công ban đầu
        [SerializeField] protected int chanceToPerformCombo = 25; // Cơ hội (tính theo phần trăm) để nhân vật thực hiện đòn tấn công kết hợp trong đòn tấn công tiếp theo
        protected bool hasRolledForComboChance = false; // Nếu chúng ta đã tung xúc xắc để có cơ hội trong trạng thái này

        [Header("Engagement Distance")]
        [SerializeField] public float maximumEngagementDistance = 5; // Khoảng cách chúng ta phải cách xa mục tiêu trước khi vào trạng thái mục tiêu theo đuổi

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // NẾU BẠN MUỐN NHÂN VẬT AI ĐỐI MẶT VÀ QUAY VỀ MỤC TIÊU CỦA NÓ KHI NÓ Ở NGOÀI GÓC NHÌN CỦA NÓ, HÃY BAO GỒM ĐIỀU NÀY
            if (aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            // NẾU MỤC TIÊU CỦA CHÚNG TA KHÔNG CÒN HIỆN DIỆN, HÃY CHUYỂN VỀ CHẾ ĐỘ KHÔNG HOẠT ĐỘNG
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // NẾU CHÚNG TA KHÔNG CÓ ĐỘT TẤN CÔNG, HÃY NHẬN MỘT ĐỘT
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = choosenAttack;
                // LĂN ĐỂ CÓ CƠ HỘI KẾT HỢP
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            // NẾU CHÚNG TA Ở NGOÀI KHOẢNG CÁCH GIAO CHIẾN, HÃY CHUYỂN SANG TRẠNG THÁI THEO ĐUỔI MỤC TIÊU
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach (var potentialAttack in aiCharacterAttacks)
            {
                // NẾU CHÚNG TA QUÁ GẦN ĐỐI VỚI CUỘC TẤN CÔNG NÀY, HÃY KIỂM TRA PHẦN TIẾP THEO
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                // NẾU CHÚNG TA QUÁ XA ĐỐI VỚI CUỘC TẤN CÔNG NÀY, HÃY KIỂM TRA PHẦN TIẾP THEO
                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                // NẾU MỤC TIÊU NẰM NGOÀI TẦM NHÌN TỐI THIỂU ĐỐI VỚI CUỘC TẤN CÔNG NÀY, HÃY KIỂM TRA PHẦN TIẾP THEO
                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                // NẾU MỤC TIÊU NẰM NGOÀI TẦM NHÌN TỐI ĐA CỦA CUỘC TẤN CÔNG NÀY, HÃY KIỂM TRA CUỘC TIẾP THEO
                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
                return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight)
                {
                    choosenAttack = attack;
                    previousAttack = choosenAttack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage < outcomeChance)
                outcomeWillBePerformed = true;

            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;
            hasRolledForComboChance = false;
        }
    }
}
