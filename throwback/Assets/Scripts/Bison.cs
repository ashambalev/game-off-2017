using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bison : MonoBehaviour
{

    public enum BisonState
    {
        None = -1,
        Spawned = 0,
        InAir = 1,
        Attack = 2,
        Return = 3,
        Hurt = 4
    }

    private Animator _animator;
    private AudioSource _audio_source;

    public float MinHeight;
    public float MaxHeight;
    public Vector3 SpawnPosition;
    public float SpawnSpeed = 5f;
    public Vector3 StartAirPosition;
    public Vector3 EndAirPosition;
    public Vector3 AttackOffset;

    public Vector3 ReturnPosition;
    public float AirSpeed = 0.2f;
    public float AttackSpeed = 0.2f;

    private SpriteRenderer _sprite;
    public BisonState State = BisonState.None;

    public AudioClip HurtSound;


    // Use this for initialization
    void Start()
    {
        _audio_source = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ReturnPosition, SpawnPosition);
        Gizmos.DrawLine(SpawnPosition, StartAirPosition);
        Gizmos.DrawLine(StartAirPosition, EndAirPosition);
    }

    public void SpawnBison()
    {
        transform.position = SpawnPosition;
        StartAirPosition = new Vector3(StartAirPosition.x, Random.Range(MinHeight, MaxHeight), 0);
        EndAirPosition = new Vector3(EndAirPosition.x, StartAirPosition.y + 0.2f, 0);
        State = BisonState.Spawned;
        _animator.SetBool("Flying", true);
        _animator.SetBool("Hurt", false);
    }

    void FixedUpdate()
    {
        _sprite.color = Color.Lerp(_sprite.color, Color.white, Time.fixedDeltaTime * 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnBison();
        }

        switch (State)
        {
            case BisonState.Spawned:
                transform.position = Vector3.MoveTowards(transform.position, StartAirPosition, SpawnSpeed * Time.deltaTime);
                if ((transform.position - StartAirPosition).magnitude < 0.1f)
                {
                    State = BisonState.InAir;
                }
                break;
            case BisonState.InAir:
                transform.position = Vector3.MoveTowards(transform.position, EndAirPosition, AirSpeed * Time.deltaTime);
                if ((transform.position - EndAirPosition).magnitude < 0.1f)
                {
                    State = BisonState.Attack;
                }
                break;
            case BisonState.Attack:

                transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position + AttackOffset, AttackSpeed * Time.deltaTime);
                if ((transform.position - GameManager.Instance.Player.transform.position - AttackOffset).magnitude < 0.1f)
                {
                    GameManager.Instance.Player.Hurt(2);
                    State = BisonState.Return;
                    _animator.SetBool("Flying", false);
                }
                break;
            case BisonState.Return:

                transform.position = Vector3.MoveTowards(transform.position, SpawnPosition, SpawnSpeed * Time.deltaTime);
                if ((transform.position - SpawnPosition).magnitude < 0.1f)
                {
                    State = BisonState.None;
                }
                break;
            case BisonState.Hurt:

                transform.position -= new Vector3(15f * GameManager.Instance.LevelSpeed, 0, 0) * Time.deltaTime;
                if (transform.position.x < SpawnPosition.x)
                {
                    State = BisonState.None;
                }
                break;
        }


    }


    public void Hurt()
    {
        _sprite.color = Color.red;
        _animator.SetBool("Hurt", true);
        State = BisonState.Hurt;
        _audio_source.PlayOneShot(HurtSound);
        GameManager.Instance.ScreenShake.Shake(2f, 0.3f);
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (transform.position.x<-14f)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("ThrowedObject"))
        {

            GameManager.Instance.AddScore(GameManager.Instance.BisonHitPoints);
            other.gameObject.layer = LayerMask.NameToLayer("InactiveObject");
            other.rigidbody.AddForce(new Vector2(-15f, 0), ForceMode2D.Impulse);
            GameManager.Instance.ExplodeEffect.SpawnEffect(other.transform.position, false);
            Hurt();
        }
    }
}
