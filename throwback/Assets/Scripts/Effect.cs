using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private Animator _animator;
    private bool _on_level = false;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void SpawnEffect(Vector3 position, bool on_level)
    {
        gameObject.SetActive(true);
        _on_level = on_level;
        transform.position = position;
        _animator.Play(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
    }

    public void StopEffect()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_on_level)
        {
            transform.position -= new Vector3(GameManager.Instance.LevelSpeed / 2f, 0, 0);
        }
    }
}
