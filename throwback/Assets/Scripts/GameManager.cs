using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public float CurrentLevelSpeed = 1f;
    public float LevelSpeed = 1f;
    public float LevelSpeedIncrease = 0.1f;
    public Player Player;


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

        if (Player.transform.position.x > PlayerPosition)
        {
            CurrentLevelSpeed = LevelSpeed + Mathf.Abs(Player.transform.position.x - PlayerPosition) * LevelSpeedIncrease;
        }
    }
}
