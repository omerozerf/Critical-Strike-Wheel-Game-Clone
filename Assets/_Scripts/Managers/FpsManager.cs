using System;
using UnityEngine;

namespace Managers
{
    public class FpsManager : MonoBehaviour
    {
        [SerializeField] private int _targetFps;


        private void Awake()
        {
            Application.targetFrameRate = _targetFps;
        }
    }
}