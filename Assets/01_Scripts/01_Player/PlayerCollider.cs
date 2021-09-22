using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCollider : MonoBehaviour, IEraserable, IClickedObject
{
    protected Player m_Player;
    protected BoxCollider2D m_Collider;

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Player player { get => m_Player; }
    public Vector2 size => m_Collider.size;
    #endregion
    #region 외부 함수
    public void __Initialize(Player player)
    {
        // 이벤트 링크
        M_Game.OnEnterPlayMode += OnPlayEnter;
        M_Game.OnExitPlayMode += OnPlayExit;

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
    public SpriteRenderer GetSpriteRenderer()
    {
        return m_Player.renderer;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Player;
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
        if (M_Game.isPlayMode)
        {
            if (collision.CompareTag("Enemy") && !m_Player.isSafe)
            {
                m_Player.Death();
            }

            if (collision.CompareTag("Coin"))
            {
                collision.GetComponent<CoinCollider>().coin.gameObject.SetActive(false);
            }
        }

        if (collision.CompareTag("SafetyZone"))
        {
            m_Player.isSafe = true;
            m_Player.spawnPos = collision.transform.position;

            if (M_Game.isPlayMode)
            {
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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone") &&
            m_Player.spawnPos == collision.transform.position)
        {
            m_Player.isSafe = false;
        }
    }
    #endregion
}
