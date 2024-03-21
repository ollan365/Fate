using UnityEngine;
using System;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private string playerName;
    public string Name { get => playerName; set => playerName = value; }

    private int language = 1;
    public int Language { get => language; set => language = value; }

    private int gender;
    public int Gender { get => gender; set => gender = value; }
    private int month, day;
    public int Month { get => month; set => month = value; }
    public int Day { get => day; set => day = value; }

    private int actPower;
    public int ActPower { get => actPower; set => actPower = Math.Clamp(value, 0, 15); }
}
