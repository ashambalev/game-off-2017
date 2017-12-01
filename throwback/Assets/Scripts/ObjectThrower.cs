using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectThrowItem
{
    public bool Obstacle;
    public bool ThrowItem;
    public bool AddBison;
    public bool IncreaseDifficulty;
    public float MinDelay;
    public float MaxDelay;
}

[System.Serializable]
public class ObjectThrowSection
{

    public List<ObjectThrowItem> Items;
}

public class ObjectThrower : MonoBehaviour
{

    public Vector3[] ThrowPositions;
    public float[] ThrowRotations;

    public Vector3 ObstaclePosition;
    public int ObjectCounter = 0;
    public GameObject Prefab;
    public GameObject ObstaclePrefab;
    public float Interval;
    public List<ObjectThrowSection> Sections;
    public List<ObjectThrowItem> CurrentSection;
    public int OverrideSectionIndex = -1;
    public int CurrentIndex = 0;

    private float _timer;

    // Use this for initialization
    void Start()
    {
        _timer = Interval;
        CurrentIndex = 0;
        if (OverrideSectionIndex >= 0)
        {
            CurrentSection = Sections[OverrideSectionIndex].Items;
        }
        else
        {
            CurrentSection = Sections[Random.Range(0, Sections.Count)].Items;
        }
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < ThrowPositions.Length; i++)
        {
            var pos = ThrowPositions[i];
            var angle = Mathf.Deg2Rad * ThrowRotations[i];
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(pos, Vector3.one);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos, pos + new Vector3(1.5f * Mathf.Sin(angle), 1.5f * Mathf.Cos(angle), 0f));
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ObstaclePosition, 1f);
    }

    void ThrowObject()
    {
        var i = Random.Range(0, ThrowPositions.Length);
        var pos = ThrowPositions[i];
        var angle = Mathf.Deg2Rad * ThrowRotations[i];
        var direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
        var obj = GameObject.Instantiate(Prefab, pos, Quaternion.identity);
        obj.GetComponent<FlyingObject>().SpawnAndThrow(pos, direction);
        ObjectCounter++;
        if (ObjectCounter > 5)
        {
            GameManager.Instance.IncreaseDifficulty();
            ObjectCounter = 0;
        }
    }

    void AddObstacle()
    {

        var i = Random.Range(0, ThrowPositions.Length);
        var pos = ThrowPositions[i];
        var angle = Mathf.Deg2Rad * ThrowRotations[i];
        var direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
        var obj = GameObject.Instantiate(ObstaclePrefab, ObstaclePosition, Quaternion.identity);
        obj.GetComponent<Obstacle>().SpawnObstacle();
        ObjectCounter++;

    }

    void GetItem()
    {
        var i = CurrentSection[CurrentIndex];
        if (i.AddBison)
        {
            GameManager.Instance.Bison.SpawnBison();
        }
        if (i.ThrowItem)
        {
            ThrowObject();
        }
        if (i.Obstacle)
        {
            AddObstacle();
        }
        if (i.IncreaseDifficulty)
        {
            GameManager.Instance.IncreaseDifficulty();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.GameStarted || GameManager.Instance.GameFinished)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddObstacle();
        }

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

        }
        else
        {
            CurrentIndex++;
            if (CurrentIndex > CurrentSection.Count - 1)
            {
                if (OverrideSectionIndex >= 0)
                {
                    CurrentSection = Sections[OverrideSectionIndex].Items;
                }
                else
                {
                    CurrentSection = Sections[Random.Range(0, Sections.Count)].Items;
                }
                CurrentIndex = 0;
            }

            _timer = Random.Range(CurrentSection[CurrentIndex].MinDelay, CurrentSection[CurrentIndex].MaxDelay);
            GetItem();

        }
    }
}
