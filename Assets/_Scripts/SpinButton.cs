using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private Button _spinButton;
    
    public static event Action OnButtonClicked;
    
    
    private void Awake()
    {
        _spinButton.onClick.AddListener(OnSpinButtonClicked);
    }

    private void OnDestroy()
    {
        _spinButton.onClick.RemoveListener(OnSpinButtonClicked);
    }


    private void OnSpinButtonClicked()
    {
        OnButtonClicked?.Invoke();
    }


    private void OnValidate()
    {
        if (!_spinButton)
        {
            _spinButton = GetComponent<Button>();
        }
    }
}
