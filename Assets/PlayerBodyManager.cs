using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerBodyManager : MonoBehaviour
    {
        PlayerManager player;

        [Header("Hair Object")]
        [SerializeField] public GameObject hair;
        [SerializeField] public GameObject facialHair;

        [Header("Male")]
        [SerializeField] public GameObject maleObject;     // CHỦ GAMEOBJECT PARENT
        [SerializeField] public GameObject maleHead; // MÔ HÌNH ĐẦU MẶC ĐỊNH KHI THOÁT GIÁP
        [SerializeField] public GameObject[] maleBody; // MÔ HÌNH THÂN TRÊN MẶC ĐỊNH KHI THOÁT GIÁP (NGỰC, TAY PHẢI TRÊN, TAY TRÁI TRÊN)
        [SerializeField] public GameObject[] maleArms; // MÔ HÌNH THÂN TRÊN MẶC ĐỊNH KHI THOÁT GIÁP (TAY PHẢI DƯỚI, TAY PHẢI, TAY TRÁI DƯỚI, TAY TRÁI)
        [SerializeField] public GameObject[] maleLegs; // MÔ HÌNH THÂN TRÊN MẶC ĐỊNH KHI THOÁT GIÁP (HÔNG, CHÂN PHẢI, CHÂN TRÁI)
        [SerializeField] public GameObject maleEyebrows; // ĐẶC ĐIỂM KHUÔN MẶT
        [SerializeField] public GameObject maleFacialHair; // ĐẶC ĐIỂM KHUÔN MẶT

        [Header("Female")]
        [SerializeField] public GameObject femaleObject;
        [SerializeField] public GameObject femaleHead;
        [SerializeField] public GameObject[] femaleBody;
        [SerializeField] public GameObject[] femaleArms;
        [SerializeField] public GameObject[] femaleLegs;
        [SerializeField] public GameObject femaleEyebrows;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        // BẬT CÁC TÍNH NĂNG CỦA BODY
        public void EnableHead()
        {
            // BẬT ĐỐI TƯỢNG ĐẦU
            maleHead.SetActive(true);
            femaleHead.SetActive(true);

            // BẬT BẤT KỲ ĐỐI TƯỢNG NÀO TRÊN MẶT (LÔNG MÀY, MÔI, MŨI, V.V.)
            maleEyebrows.SetActive(true);
            femaleEyebrows.SetActive(true);
        }

        public void DisableHead()
        {
            // VÔ HIỆU HÓA ĐỐI TƯỢNG ĐẦU
            maleHead.SetActive(false);
            femaleHead.SetActive(false);

            // VÔ HIỆU HÓA BẤT KỲ ĐỐI TƯỢNG NÀO TRÊN MẶT (LÔNG MÀY, MÔI, MŨI, V.V.)
            maleEyebrows.SetActive(false);
        }

        public void EnableHair()
        {
            hair.SetActive(true);
        }

        public void DisableHair()
        {
            hair.SetActive(false);
        }

        public void EnableFacialHair()
        {
            facialHair.SetActive(true);
        }

        public void DisableFacialHair()
        {
            facialHair.SetActive(false);
        }

        public void EnableBody()
        {
            foreach (var model in maleBody)
            {
                model.SetActive(true);
            }

            foreach (var model in femaleBody)
            {
                model.SetActive(true);
            }
        }

        public void EnableArms()
        {
            foreach (var model in maleArms)
            {
                model.SetActive(true);
            }

            foreach (var model in femaleArms)
            {
                model.SetActive(true);
            }
        }

        public void EnableLowerBody()
        {
            foreach (var model in maleLegs)
            {
                model.SetActive(true);
            }

            foreach (var model in femaleLegs)
            {
                model.SetActive(true);
            }
        }

        public void DisableBody()
        {
            foreach (var model in maleBody)
            {
                model.SetActive(false);
            }

            foreach (var model in femaleBody)
            {
                model.SetActive(false);
            }
        }

        public void DisableArms()
        {
            foreach (var model in maleArms)
            {
                model.SetActive(false);
            }

            foreach (var model in femaleArms)
            {
                model.SetActive(false);
            }
        }

        public void DisableLowerBody()
        {
            foreach (var model in maleLegs)
            {
                model.SetActive(false);
            }

            foreach (var model in femaleLegs)
            {
                model.SetActive(false);
            }
        }

        public void ToggleBodyType(bool isMale)
        {
            if (isMale)
            {
                maleObject.SetActive(true);
                femaleObject.SetActive(false);
            }
            else
            {
                maleObject.SetActive(false);
                femaleObject.SetActive(true);
            }

            player.playerEquipmentManager.EquipArmor();
        }
    }
}
