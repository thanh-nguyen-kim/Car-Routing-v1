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
        private int Delay = 60;

        void Start()
        {
            if (RedGreenCycle > 0)
                Invoke("ChangState",RedGreenCycle);
        }

        private void ChangState()
        {
            state = !state;
            if (transform.GetChild(0).gameObject != null)
            {
                if (state)
                {
                    transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                    Invoke("ChangState", Delay);
                }
                else
                {
                    transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                    Invoke("ChangState", RedGreenCycle);
                }
            }
        }

        public bool Equals(CheckPoint obj)
        {
            if (this == obj)
                return true;
            return false;
        }
    }
}