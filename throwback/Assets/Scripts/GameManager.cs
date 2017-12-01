using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameUI GameUI;

    public ulong Score;

    public int BisonHitPoints = 300;
    public int GrabPoints = 50;
    public int ThrowPointsPoints = 80;
    public int JumpOverObstaclePoints = 25;

    public float CurrentLevelSpeed = 1f;
    public float TargetLevelSpeed = 1f;
    public float LevelSpeed = 1f;
    public float LevelSpeedIncrease = 0.1f;
    public Player Player;

    public float MinPosition = -8f;
    public float MaxPosition = -2f;
    public float PlayerPosition = -3.1f;
    public float FloorPosition = -3.8f;

    public bool GameStarted = false;
    public bool GameFinished = false;

    public AudioClip MenuMusic;
    public AudioClip GameMusic;
    public AudioSource AudioSource;

    public Effect JumpEffect;
    public Effect ThrowEffect;
    public Effect LandEffect;
    public Effect ExplodeEffect;

    public ScreenShake ScreenShake;
    public Animator BisonWallAnimator;
    public Bison Bison;

    private float _scoreDistance;
    private float _scoreTimer;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));

                if (FindObjectsOfType(typeof(GameManager)).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopening the scene might fix it.");
                    return _instance;
                }
            }
            return _instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        LevelSpeed = 0;
        Player.transform.position = new Vector3(PlayerPosition, FloorPosition, Player.transform.position.z);
        AudioSource.clip = MenuMusic;
        AudioSource.Play();
    }

    public void AddScore(int score)
    {
        Score += (ulong)score;
    }

    public void StartGame()
    {
        BisonWallAnimator.SetBool("Running", true);
        LevelSpeed = 1f;
        GameStarted = true;
        Player.StartRunning();
        GameUI.StartGame();
        GameFinished = false;
        AudioSource.clip = GameMusic;
        AudioSource.Play();
    }

    [ContextMenu("Finish")]
    public void FinishGame()
    {

        GameFinished = true;
        BisonWallAnimator.SetBool("Running", false);
        Invoke("ShowEnd", 2f);
    }


    public void LoseGame()
    {

        GameFinished = true;
        BisonWallAnimator.SetTrigger("Overrun");
        Invoke("ShowLost", 0.2f);
    }

    public void ShowEnd()
    {
        BisonWallAnimator.SetBool("Running", false);
        Player.RunAway = true;
        GameUI.FinishGame();
    }


    public void ShowLost()
    {
        GameUI.ShowLostGame();
    }

    void FixedUpdate()
    {

        if (Player.SpeedBoost <= 0)
            PlayerPosition = Mathf.Min(PlayerPosition, Player.transform.position.x);

        if (PlayerPosition < MinPosition)
        {
            LoseGame();
        }
        PlayerPosition = Mathf.Clamp(PlayerPosition, MinPosition, MaxPosition);
    }

    public void GrabEnded()
    {
        PlayerPosition += 1f;
    }

    public void IncreaseDifficulty()
    {
        TargetLevelSpeed += 0.1f;
        if (TargetLevelSpeed > 4f)
        {
            FinishGame();
        }
    }

    public void DecreaseDifficulty()
    {
        TargetLevelSpeed -= 0.2f;
    }

    void Update()
    {
        if (!GameStarted)
        {
            if (Input.GetButtonDown("Jump"))
            {
                StartGame();
            }
            return;
        }
        if (GameFinished)
        {
            return;
        }

        if (Player.transform.position.x > PlayerPosition && !Player.RunAway)
        {
            CurrentLevelSpeed = LevelSpeed + Mathf.Abs(Player.transform.position.x - PlayerPosition) * LevelSpeedIncrease;
        }

        if (TargetLevelSpeed > LevelSpeed)
        {
            LevelSpeed = Mathf.Lerp(LevelSpeed, TargetLevelSpeed, 2f * Time.deltaTime);
        }

        PlayerPosition -= Time.deltaTime * 0.05f;

        if (_scoreTimer <= 0)
        {
            AddScore(Mathf.CeilToInt(_scoreDistance));
            _scoreDistance = 0;
            _scoreTimer = 1f / 60f;
        }
        else
        {
            _scoreDistance += Player.CurrentSpeed + 10 * Player.SpeedBoost - 0.5f * Player.SpeedSlow;
            _scoreTimer -= Time.deltaTime;
        }

    }
}
