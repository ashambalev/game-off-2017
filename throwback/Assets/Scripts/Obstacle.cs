using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    public void SpawnObstacle()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > -14f)
        {
            transform.position -= new Vector3(GameManager.Instance.LevelSpeed * 10, 0, 0) * Time.deltaTime;
        }
        else
        {
            if (gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                GameManager.Instance.AddScore(GameManager.Instance.JumpOverObstaclePoints);
                gameObject.SetActive(false);

            }
            Destroy(gameObject);
        }
    }
}
