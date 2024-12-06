using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;

        [Header("Static Effects")]
        public List<StaticCharacterEffect> staticEffects = new List<StaticCharacterEffect>();

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // NẾU CHÚNG TA ĐẶT THỦ CÔNG VFX BẮN MÁU TRÊN MÔ HÌNH NÀY, HÃY CHƠI PHIÊN BẢN CỦA NÓ
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            // NẾU KHÔNG, HÃY SỬ DỤNG GENERIC (PHIÊN BẢN MẶC ĐỊNH) MÀ CHÚNG TA CÓ Ở NƠI KHÁC
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect)
        {
            // NẾU BẠN MUỐN ĐỒNG BỘ HIỆU ỨNG TRÊN MẠNG, NẾU BẠN LÀ CHỦ SỞ HỮU, HÃY KHỞI CHẠY MỘT SERVER RPC TẠI ĐÂY ĐỂ XỬ LÝ HIỆU ỨNG TRÊN TẤT CẢ CÁC MÁY KHÁCH KHÁC

            // 1. THÊM HIỆU ỨNG TĨNH VÀO KÝ TỰ
            staticEffects.Add(effect);

            // 2. XỬ LÝ HIỆU ỨNG CỦA NÓ
            effect.ProcessStaticEffect(character);

            // 3. KIỂM TRA CÁC MỤC NULL TRONG DANH SÁCH CỦA BẠN VÀ XÓA CHÚNG
            for (int i = staticEffects.Count - 1; i > -1; i--)
            {
                if (staticEffects[i] == null)
                    staticEffects.RemoveAt(i);
            }
        }

        public void RemoveStaticEffect(int effectID)
        {
            StaticCharacterEffect effect;

            for (int i = 0; i < staticEffects.Count; i++)
            {
                if (staticEffects[i] != null)
                {
                    if (staticEffects[i].staticEffectID == effectID)
                    {
                        effect = staticEffects[i];
                        // 1. XÓA HIỆU ỨNG TĨNH KHỎI CHARACTER
                        effect.RemoveStaticEffect(character);
                        // 2. XÓA HIỆU ỨNG TĨNH KHỎI DANH SÁCH
                        staticEffects.Remove(effect);
                    }
                }
            }

            // 3. KIỂM TRA CÁC MỤC NULL TRONG DANH SÁCH  VÀ XÓA CHÚNG
            for (int i = staticEffects.Count - 1; i > -1; i--)
            {
                if (staticEffects[i] == null)
                    staticEffects.RemoveAt(i);
            }
        }
    }
}
