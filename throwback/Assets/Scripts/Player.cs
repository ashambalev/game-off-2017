using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator Animator;

    public bool CanJump = true;
    public bool InAir = false;
    public bool Grabbing = false;
    public float JumpSpeed = 50f;
    public float JumpGravity = 10f;
    public float FallCoef = 1.1f;
    public float MaxSpeed = 50f;
    public float VerticalSpeed = 0;

    public float CurrentSpeed = 1f;
    public float Speed = 1f;
    public float SpeedSlow = 0f;
    public float SpeedBoost = 0f;
    public float SpeedBoostDecrease = 0.7f;

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


        transform.position += new Vector3(CurrentSpeed - GameManager.Instance.CurrentLevelSpeed, 0, 0);

        if (SpeedBoost > 0)
        {
            SpeedBoost -= SpeedBoostDecrease * Time.deltaTime;
        }
        else
        {
            SpeedBoost = 0;
        }



        CurrentSpeed = Speed - SpeedSlow + SpeedBoost;



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
        Grabbing = true;
        SpeedSlow = 0.1f;
    }

    public void EndGrab()
    {
        Grabbing = false;
        SpeedSlow = 0;
        SpeedBoost = 1f;
    }

}
