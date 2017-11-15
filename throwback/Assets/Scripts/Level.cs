using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{


    public float SpeedMultiplier = 5f;
    public float SpeedBoostMultiplier = 10f;
    public LevelItem[] LevelItems;

    // Use this for initialization
    void Start()
    {
        LevelItems = GetComponentsInChildren<LevelItem>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.GameStarted)
            return;
        var speed = SpeedMultiplier * GameManager.Instance.CurrentLevelSpeed + GameManager.Instance.Player.SpeedBoost * SpeedBoostMultiplier;
        foreach (var item in LevelItems)
        {
            item.transform.position -= new Vector3(speed * Time.deltaTime * item.ParallaxX, 0, 0);
            if (item.transform.position.x < -item.Width)
            {
                item.transform.position += new Vector3(2f * item.Width, 0, 0);
            }
        }
    }
}
