using SlotSystem;
using UnityEngine;

namespace WheelSystem
{
    public class WheelSlotSelector : MonoBehaviour
    {
        [SerializeField] private Slot[] _slotArray;


        private void Awake()
        {
            WheelSpinController.OnWheelStopped += HandleWheelStopped;
        }

        private void OnDestroy()
        {
            WheelSpinController.OnWheelStopped -= HandleWheelStopped;
        }
        
        private void OnValidate()
        {
            ValidateSlotComponents();
        }
        
        
        private void HandleWheelStopped(int slotIndex)
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                Debug.LogWarning("HandleWheelStopped called but _slotArray is empty.", this);
                return;
            }

            var selectedSlot = _slotArray[slotIndex];
            Debug.Log(selectedSlot.GetSlotSO() + " " + selectedSlot.GetCount());
        }
        
        private void ValidateSlotComponents()
        {
            if (_slotArray == null || _slotArray.Length == 0)
            {
                _slotArray = GetComponentsInChildren<Slot>();
                if (_slotArray.Length != 8)
                {
                    Debug.LogWarning("WheelResultController expects exactly 8 Slot components as children.", this);
                }
            }
        }
    }
}