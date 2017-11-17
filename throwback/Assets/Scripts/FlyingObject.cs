using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{

    public Vector3 StartPosition;
    public Vector3 Orientation;

    public Vector2 Rotation;
    public Vector2 Force;

    private Rigidbody2D _rb;


    // Use this for initialization
    void Start()
    {
        transform.position = StartPosition;
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(StartPosition, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(StartPosition, StartPosition + Orientation.normalized);
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        transform.position = StartPosition;
        gameObject.layer = LayerMask.NameToLayer("Object");
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        _rb.isKinematic = false;
        _rb.AddForce(GameManager.Instance.CurrentLevelSpeed * Orientation.normalized * Random.Range(Force.x, Force.y), ForceMode2D.Impulse);
        _rb.AddTorque(Random.Range(Rotation.x, Rotation.y), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Spawn();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -5f)
            _rb.isKinematic = true;
    }
}
