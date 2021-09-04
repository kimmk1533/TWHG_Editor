using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Player m_Player;

    #region 내부 컴포넌트
    protected Animator m_Animator;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    #endregion
    #endregion
    #region 외부 함수
    public void __Initialize(Player player)
    {
        // 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        m_Player = player;

        if (null == m_Animator)
        {
            m_Animator = GetComponent<Animator>();
        }
    }

    public void Death()
    {
        m_Animator.SetTrigger("Death");
    }
    public void Respawn()
    {
        M_Player.RespawnPlayer();
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {

    }
    public void OnPlayExit()
    {

    }
    #endregion
}
