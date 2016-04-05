using System.Collections;
using System;
using UnityEngine;
namespace Assets.Scripts.Entity
{
    [System.Serializable]
    public class CheckPoint : MonoBehaviour
    {
        public bool state = true;
        public int RedGreenCycle = 0;

        void Start()
        {
            if (RedGreenCycle > 0)
                InvokeRepeating("ChangState", RedGreenCycle, RedGreenCycle);
        }

        private void ChangState()
        {
            state = !state;
        }

        public bool Equals(CheckPoint obj)
        {
            if (this == obj)
                return true;
            return false;
        }
    }
}