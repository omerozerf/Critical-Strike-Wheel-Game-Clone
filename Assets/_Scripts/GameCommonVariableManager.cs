using UnityEngine;

public class GameCommonVariableManager : MonoBehaviour
{
    [SerializeField] private int _safeZoneInterval;
    [SerializeField] private int _superZoneInterval;
    
    private static GameCommonVariableManager ms_Instance;


    private void Awake()
    {
        ms_Instance = this;
    }


    public static int GetSafeZoneInterval()
    {
        return ms_Instance._safeZoneInterval;
    }
    
    public static int GetSuperZoneInterval()
    {
        return ms_Instance._superZoneInterval;
    }
}