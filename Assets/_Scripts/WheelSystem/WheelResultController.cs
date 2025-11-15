using System;
using DefaultNamespace;
using UnityEngine;

namespace WheelSystem
{
    public class WheelResultController : MonoBehaviour
    {
        [SerializeField] private Slot[] _slotArray;


        private void Awake()
        {
            WheelController.OnWheelStopped += HandleWheelStopped;
        }

        private void OnDestroy()
        {
            WheelController.OnWheelStopped -= HandleWheelStopped;
        }


        private void HandleWheelStopped(int slotIndex)
        {
            
        }


        private void OnValidate()
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                _slotArray = GetComponentsInChildren<Slot>();
                if (_slotArray.Length != 8)                {
                    Debug.LogWarning("WheelResultController expects exactly 8 Slot components as children.", this);
                }
            }
        }
    }
}