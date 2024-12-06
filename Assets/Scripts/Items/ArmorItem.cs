using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ArmorItem : EquipmentItem
    {
        [Header("Equipment Absorption Bonus")]
        public float physicalDamageAbsorption;
        public float magicDamageAbsorption;
        public float fireDamageAbsorption;
        public float holyDamageAbsorption;
        public float lightningDamageAbsorption;

        [Header("Equipment Resistance Bonus")]
        public float immunity; // KHẢ NĂNG CHỐNG ROT VÀ ĐỘC
        public float robustness; // KHẢ NĂNG CHỐNG CHẢY MÁU VÀ BĂNG GIÁ
        public float focus; // KHẢ NĂNG CHỐNG MADNESS VÀ NGỦ
        public float vitality; // KHẢ NĂNG CHỐNG LẠI DEATH CURSE

        [Header("Poise")]
        public float poise;

        public EquipmentModel[] equipmentModels;
    }
}
