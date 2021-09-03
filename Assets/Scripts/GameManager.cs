using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON PATTERN
    private static GameManager _instance;
    
    public static GameManager Instance { get { return _instance; } }
    private void Awake()
    {

        if (_instance != null && _instance != this)
        {

        }
        else
        {
            _instance = this;
        }
    }

    #endregion


    public bool AnimatorPlaying;
    public bool RailObstacle;
    public bool AirObstacle;
    public int Xcount;
    public bool GameOver;
    public bool Win;
    public int CoinCount;


}

