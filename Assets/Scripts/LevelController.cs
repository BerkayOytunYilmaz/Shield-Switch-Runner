using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{


    public Text TextUiXcount;
    public Text TextUiCoincount;
    public Text TextUiSkor;
    public GameObject Directions;
    public GameObject Win1;
    public GameObject Win2;
    public GameObject Win3;
    public GameObject Win4;
    public GameObject Win5;
    public GameObject Fail1;
    public GameObject Fail2;
    public GameObject Fail3;
    public GameObject Starter;
    public GameObject Panel;
    public bool JustOne;

    private void Start()
    {
        Time.timeScale = 0;
        if (!PlayerPrefs.HasKey("PlayerLevel"))
        {
            PlayerPrefs.SetInt("PlayerLevel" , 1);
        }

    }
    void Update()
    {
        GameFinished();
    }

    void GameFinished()
    {
        if (GameManager.Instance.Win)
        {
            Directions.SetActive(false);
            if (JustOne==false)
            {
                DOVirtual.Float(0f, GameManager.Instance.Xcount, 1f, (v) => TextUiXcount.text = v.ToString());
                DOVirtual.Float(0f, GameManager.Instance.CoinCount, 1f, (v) => TextUiCoincount.text = v.ToString());
                DOVirtual.Float(0f, GameManager.Instance.CoinCount* GameManager.Instance.Xcount, 1f, (v) => TextUiSkor.text = v.ToString());
                Win1.SetActive(true);
                Win2.SetActive(true);
                Win3.SetActive(true);
                Win4.SetActive(true);
                Win5.SetActive(true);
                PlayerPrefs.SetInt("PlayerLevel", 1 + PlayerPrefs.GetInt("PlayerLevel"));
                JustOne = true;
            }

        }
        if (GameManager.Instance.GameOver)
        {
            Directions.SetActive(false);
            Fail1.SetActive(true);
            Fail2.SetActive(true);
            Fail3.SetActive(true);

        }
    }
    public void CanStart()
    {
        Time.timeScale = 1;
        Starter.SetActive(false);
        Panel.SetActive(false);
    }
    public void LevelUp()
    {
        if (PlayerPrefs.GetInt("PlayerLevel")==1)
        {
            SceneManager.LoadScene("Level1");
        }
        if (PlayerPrefs.GetInt("PlayerLevel") == 2)
        {
            SceneManager.LoadScene("Level2");
        }
        if (PlayerPrefs.GetInt("PlayerLevel") == 3)
        {
            SceneManager.LoadScene("Level3");
        }
        if (PlayerPrefs.GetInt("PlayerLevel") == 4)
        {
            SceneManager.LoadScene("Level4");
        }
        if (PlayerPrefs.GetInt("PlayerLevel") == 5)
        {
            SceneManager.LoadScene("Level5");
        }

    }
}
