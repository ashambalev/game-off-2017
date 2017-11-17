using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public float CurrentLevelSpeed = 1f;
    public float TargetLevelSpeed = 1f;
    public float LevelSpeed = 1f;
    public float LevelSpeedIncrease = 0.1f;
    public Player Player;

    public float MinPosition = -4.5f;
    public float MaxPosition = -2f;
    public float PlayerPosition = -3.1f;
    public float FloorPosition = -3.8f;

    public bool GameStarted = false;

    // Use this for initialization
    void Start()
    {
        LevelSpeed = 0;
        Player.transform.position = new Vector3(PlayerPosition, FloorPosition, Player.transform.position.z);
    }

    public void StartGame()
    {
        LevelSpeed = 1f;
        GameStarted = true;
        Player.StartRunning();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {

        if (!Player.Grabbing)
            PlayerPosition = Mathf.Min(PlayerPosition, Player.transform.position.x);

        if (PlayerPosition < MinPosition)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        PlayerPosition = Mathf.Clamp(PlayerPosition, MinPosition, MaxPosition);

    }

    public void GrabEnded()
    {
        PlayerPosition += 0.2f;
    }

    public void IncreaseDifficulty()
    {
        TargetLevelSpeed += 0.2f;
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


        if (Input.GetKeyDown(KeyCode.U))
        {
            IncreaseDifficulty();
        }

        if (Player.transform.position.x > PlayerPosition)
        {
            CurrentLevelSpeed = LevelSpeed + Mathf.Abs(Player.transform.position.x - PlayerPosition) * LevelSpeedIncrease;
        }

        if (TargetLevelSpeed > LevelSpeed)
        {
            LevelSpeed = Mathf.Lerp(LevelSpeed, TargetLevelSpeed, 2f * Time.deltaTime);
        }
    }
}
