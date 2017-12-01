using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator Animator;

    public bool CanJump = true;
    public bool CanControlJump = false;
    public bool InAir = false;
    public bool Grabbing = false;
    public bool CanGrab = false;
    public bool RunAway = false;
    public float JumpSpeed = 50f;
    public float JumpGravity = 10f;
    public float FallCoef = 1.1f;
    public float MaxSpeed = 50f;
    public float VerticalSpeed = 0;

    public Rigidbody2D GrabbableObject;
    private SpriteRenderer _sprite;

    public AudioSource AudioSource;
    public AudioClip JumpSound;
    public AudioClip GrabSound;
    public AudioClip ThrowSound;
    public AudioClip HitSound;

    public float CurrentSpeed = 1f;
    public float SpeedSlow = 0f;
    public float SpeedSlowRecovery = 0.1f;
    public float SpeedBoost = 0f;
    public float SpeedBoostDecrease = 0.7f;
    public Vector3 HoldPosition = new Vector3(1.25f, 1.84f, 0f);
    public Vector3 ThrowPosition = new Vector3(-1.25f, 1.84f, 0f);
    public Vector2 ThrowRotation = new Vector2(-1f, 1f);
    public Vector2 ThrowForce = new Vector2(8f, 13f);

    public LayerMask GrabLayerMask = -1;
    public Vector2 GrabPosition;
    public Vector2 GrabSize;
    public float ThrowSpeedBoost = 0.6f;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        Animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        Invoke("IdleVariation", Random.Range(5f, 10f));
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayOneShot(clip);
    }

    public void StartRunning()
    {
        Animator.SetBool("Running", true);
        CancelInvoke("IdleVariation");
    }

    public void IdleVariation()
    {
        Animator.SetTrigger("IdleVariation");
        Invoke("IdleVariation", Random.Range(5f, 10f));
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        Animator.SetFloat("Speed", CurrentSpeed);
        Animator.SetBool("Flying", InAir);
        _sprite.color = Color.Lerp(_sprite.color, Color.white, Time.fixedDeltaTime * 10f);
        CheckForGrabbableObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.GameStarted)
            return;

        if (CanJump && (Input.GetButtonDown("Jump") || Input.GetButton("Jump")))
        {
            Jump();
        }
        if (InAir && Input.GetButton("Jump"))
        {
            ContinueJump();
        }
        if (InAir && Input.GetButtonUp("Jump"))
        {
            EndJump();
        }
        if (!Grabbing && Input.GetButtonDown("Grab"))
        {
            StartGrab();
        }
        if (Grabbing && Input.GetButtonUp("Grab"))
        {
            EndGrab();
        }

        if (Grabbing && GrabbableObject != null)
        {
            GrabbableObject.transform.position = Vector3.Lerp(GrabbableObject.transform.position, transform.position + HoldPosition, 30f * Time.deltaTime);
        }


        transform.position += new Vector3(CurrentSpeed - GameManager.Instance.CurrentLevelSpeed, 0, 0);

        if (RunAway)
        {
            SpeedBoost = 0.6f;
        }
        if (SpeedBoost > 0)
        {
            SpeedBoost -= SpeedBoostDecrease * Time.deltaTime;
        }
        else
        {
            SpeedBoost = 0;
        }



        CurrentSpeed = GameManager.Instance.LevelSpeed - SpeedSlow + SpeedBoost;

        if (SpeedSlow > 0)
        {
            SpeedSlow -= SpeedSlowRecovery * Time.deltaTime;
        }
        else
        {
            SpeedSlow = 0;
        }



        if (InAir)
        {

            transform.position += new Vector3(0, VerticalSpeed * Time.deltaTime, 0);
            if (VerticalSpeed > 0)
            {
                VerticalSpeed -= JumpGravity * Time.deltaTime;
            }
            else if (VerticalSpeed < 0)
            {
                VerticalSpeed = Mathf.Max(-MaxSpeed, VerticalSpeed - FallCoef * JumpGravity * Time.deltaTime);
            }

            if (transform.position.y <= GameManager.Instance.FloorPosition)
            {
                VerticalSpeed = 0;
                transform.position = new Vector3(transform.position.x, GameManager.Instance.FloorPosition, transform.position.z);
                CanJump = true;
                InAir = false;
                GameManager.Instance.LandEffect.SpawnEffect(transform.position, true);
            }
        }
    }

    public void Jump()
    {
        CanJump = false;
        CanControlJump = true;
        InAir = true;
        VerticalSpeed = JumpSpeed / 4f;
        PlaySFX(JumpSound);
        GameManager.Instance.JumpEffect.SpawnEffect(transform.position, true);
    }
    public void ContinueJump()
    {
        if (transform.position.y < -2 && CanControlJump)
        {
            VerticalSpeed = Mathf.Min(JumpSpeed, VerticalSpeed + JumpSpeed / 4f);
        }
        else
        {
            CanControlJump = false;
        }
    }
    public void EndJump()
    {
        CanControlJump = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(GrabPosition.x, GrabPosition.y), GrabSize);
    }

    public void CheckForGrabbableObject()
    {
        var cols = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y) + GrabPosition, GrabSize, 0, GrabLayerMask);
        if (cols.Length > 0)
        {
            CanGrab = true;
            GrabbableObject = cols[0].attachedRigidbody;
        }
        else
        {
            CanGrab = false;
            GrabbableObject = null;
        }
    }

    public void StartGrab()
    {
        if (!CanGrab || GrabbableObject == null)
        {

            Animator.SetBool("Holding", true);
            Invoke("EndFakeGrab", 0.2f);
            return;
        }
        GameManager.Instance.AddScore(GameManager.Instance.GrabPoints);
        GrabbableObject.isKinematic = true;
        GrabbableObject.angularVelocity = 0;
        GrabbableObject.velocity = Vector2.zero;
        Grabbing = true;

        Animator.SetBool("Holding", Grabbing);
        SpeedSlow = 0.0f;
        Time.timeScale = 0.6f;

        PlaySFX(GrabSound);
    }

    void EndFakeGrab()
    {

        Animator.SetTrigger("Throw");
        Animator.SetBool("Holding", false);
    }

    public void EndGrab()
    {
        GameManager.Instance.GrabEnded();
        GrabbableObject.gameObject.layer = LayerMask.NameToLayer("ThrowedObject");
        GrabbableObject.isKinematic = false;
        GrabbableObject.AddForce(new Vector2(-Random.Range(ThrowForce.x, ThrowForce.y), VerticalSpeed / 2f), ForceMode2D.Impulse);
        GrabbableObject.AddTorque(Random.Range(ThrowRotation.x, ThrowRotation.y), ForceMode2D.Impulse);
        Grabbing = false;
        GameManager.Instance.AddScore(GameManager.Instance.ThrowPointsPoints);
        Animator.SetTrigger("Throw");
        Animator.SetBool("Holding", Grabbing);
        SpeedSlow = 0;
        SpeedBoost = ThrowSpeedBoost;

        Time.timeScale = 1f;

        PlaySFX(ThrowSound);

        GameManager.Instance.ThrowEffect.SpawnEffect(transform.position + new Vector3(2f, 1.8f, 0), false);
    }

    public void Hurt(int damage)
    {
        _sprite.color = Color.red;
        SpeedSlow = 0.05f * damage;
        PlaySFX(HitSound);
        GameManager.Instance.ScreenShake.Shake(1f, 0.3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("InactiveObject");
            Hurt(3);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("InactiveObject");
            Hurt(1);
        }
    }

}
