using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{

    public float RunTimeout = 0.5f;

    public GameObject Splash;
    public GameObject Run;
    public GameObject FinishedGame;
    public GameObject LostGame;
    public Text ScoreText;
    public bool AllowControl = false;

    // Use this for initialization
    void Start()
    {
        AllowControl = true;
        Splash.SetActive(true);
        Run.SetActive(false);
        FinishedGame.SetActive(false);
        LostGame.SetActive(false);
        ScoreText.gameObject.SetActive(false);
    }

    public void HideRun()
    {

        Run.SetActive(false);
    }

    public void FinishGame()
    {
        AllowControl = true;
        FinishedGame.SetActive(true);

    }
    public void ShowLostGame()
    {

        AllowControl = true;
        LostGame.SetActive(true);
    }

    public void StartGame()
    {
        AllowControl = false;
        Splash.SetActive(false);

        ScoreText.gameObject.SetActive(true);
        Run.SetActive(true);
        Invoke("HideRun", RunTimeout);
    }

    void FixedUpdate()
    {
        ScoreText.text = "SCORE: " + GameManager.Instance.Score.ToString();
    }

    void Update()
    {

        if (AllowControl && Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
