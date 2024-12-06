using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeUnitilDestroyed = 5;

        private void Awake()
        {
            Destroy(gameObject, timeUnitilDestroyed);
        }
    }
}

