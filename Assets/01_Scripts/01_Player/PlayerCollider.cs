using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCollider : MonoBehaviour, IEraserable
{
    public Player m_Player;
    protected BoxCollider2D m_Collider;

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Vector2 size => m_Collider.size;
    #endregion
    #region 외부 함수
    public void __Initialize(Player player)
    {
        // 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        m_Player = player;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<BoxCollider2D>();
        }
    }

    public void Erase()
    {
        m_Player.gameObject.SetActive(false);
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
    #region 유니티 콜백 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone"))
        {
            m_Player.isSafe = true;
            m_Player.spawnPos = collision.transform.position;

            bool? isFinishZone = collision.gameObject.GetComponent<SafetyZoneCollider>()?.isFinishZone;

            if (isFinishZone.HasValue)
            {
                if (isFinishZone.Value && !M_Coin.IsLeftCoin)
                {
                    // 승리
                    Debug.Log("승리");
                }
            }
        }

        if (M_Edit.isPlayMode)
        {
            if (collision.CompareTag("Enemy") && !m_Player.isSafe)
            {
                m_Player.Death();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone") &&
            m_Player.spawnPos == collision.transform.position)
        {
            m_Player.isSafe = false;
            //Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, size, 0f, LayerMask.GetMask("SafetyZone"));
            //if (hits.Length <= 0)
            //{
            //    m_Player.isSafe = false;
            //}
        }
    }
    #endregion
}
