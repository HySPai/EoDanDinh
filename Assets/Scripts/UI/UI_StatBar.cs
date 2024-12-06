using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UI_StatBar : MonoBehaviour
    {
        protected Slider slider;
        protected RectTransform rectTransform;

        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1;
        // THANH PHỤ Ở PHÍA SAU CÓ THỂ THANH ĐÁNH BÓNG (THANH MÀU VÀNG HIỂN THỊ MỘT HÀNH ĐỘNG/SÁT THƯƠNG LÀM MẤT ĐI MỨC TRỊ SỐ LIỆU HIỆN TẠI)


        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {

        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStats)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

                // ĐẶT LẠI VỊ TRÍ CỦA CÁC THANH DỰA TRÊN CÁC THIẾT LẬP CỦA NHÓM BỐ TRÍ CỦA CHÚNG
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }
        }
    }
}