using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Vector2 SpawnPoint;

    Animator animator;

    __GameManager m_Game;

    private void Awake()
    {
        m_Game = __GameManager.Instance;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        m_Game.OnPlayEnter += OnPlayEnter;
        m_Game.OnPlayExit += OnPlayExit;
    }

    public void OnPlayEnter()
    {

    }
    public void OnPlayExit()
    {

    }

    public void Respawn()
    {
        // 스폰 포인트로 이동
        transform.parent.position = SpawnPoint;

        // 코인 재 생성
        CoinManager.Instance.RespawnCoin();

        // 부활
        animator.SetBool("Death", false);
    }
}
