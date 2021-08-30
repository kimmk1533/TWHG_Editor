using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Vector2 m_SpawnPoint;

    Animator m_Animator;

    InGameManager M_Game;

    private void Awake()
    {
        M_Game = InGameManager.Instance;
    }
    private void Start()
    {
        m_Animator = GetComponent<Animator>();

        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
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
        transform.parent.position = m_SpawnPoint;

        // 코인 재 생성
        CoinManager.Instance.RespawnCoin();

        // 부활
        m_Animator.SetBool("Death", false);
    }
}
