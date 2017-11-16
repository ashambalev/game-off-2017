using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator Animator;

    public bool CanJump = true;
    public bool InAir = false;
    public bool Grabbing = false;
    public bool CanGrab = false;
    public float JumpSpeed = 50f;
    public float JumpGravity = 10f;
    public float FallCoef = 1.1f;
    public float MaxSpeed = 50f;
    public float VerticalSpeed = 0;

    public Rigidbody2D GrabbableObject;

    public float CurrentSpeed = 1f;
    public float SpeedSlow = 0f;
    public float SpeedSlowRecovery = 0.1f;
    public float SpeedBoost = 0f;
    public float SpeedBoostDecrease = 0.7f;
    public Vector3 HoldPosition = new Vector3(1.25f, 1.84f, 0f);
    public Vector3 ThrowPosition = new Vector3(-1.25f, 1.84f, 0f);
    public Vector2 ThrowRotation = new Vector2(-1f, 1f);
    public Vector2 ThrowForce = new Vector2(8f, 13f);
    public float ThrowSpeedBoost = 0.6f;

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Invoke("IdleVariation", Random.Range(5f, 10f));
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.GameStarted)
            return;

        if (CanJump && Input.GetButtonDown("Jump"))
        {
            Jump();
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
            GrabbableObject.transform.position = Vector3.Lerp(GrabbableObject.transform.position, transform.position + HoldPosition, 50f * Time.deltaTime);
        }


        transform.position += new Vector3(CurrentSpeed - GameManager.Instance.CurrentLevelSpeed, 0, 0);

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
            }
        }
    }

    public void Jump()
    {
        CanJump = false;
        InAir = true;
        VerticalSpeed = JumpSpeed;
    }

    public void StartGrab()
    {
        if (!CanGrab || GrabbableObject == null)
            return;
        GrabbableObject.isKinematic = true;
        GrabbableObject.angularVelocity = 0;
        GrabbableObject.velocity = Vector2.zero;
        Grabbing = true;
        SpeedSlow = 0.0f;
    }

    public void EndGrab()
    {
        GameManager.Instance.GrabEnded();
        GrabbableObject.gameObject.layer = LayerMask.NameToLayer("InactiveObject");
        GrabbableObject.isKinematic = false;
        GrabbableObject.AddForce(new Vector2(-Random.Range(ThrowForce.x, ThrowForce.y), VerticalSpeed / 2f), ForceMode2D.Impulse);
        GrabbableObject.AddTorque(Random.Range(ThrowRotation.x, ThrowRotation.y), ForceMode2D.Impulse);
        Grabbing = false;
        SpeedSlow = 0;
        SpeedBoost = ThrowSpeedBoost;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
            return;
        GrabbableObject = other.GetComponent<Rigidbody2D>();
        CanGrab = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
            return;
        CanGrab = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Grabbing || other.gameObject.layer == LayerMask.NameToLayer("Level"))
            return;

        GrabbableObject = null;
        CanGrab = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            SpeedSlow = 0.05f;
            other.gameObject.layer = LayerMask.NameToLayer("InactiveObject");
        }
    }

}
