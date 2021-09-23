using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCollider : MonoBehaviour, IEraserable, IClickedObject
{
    protected Player m_Player;

    #region 내부 컴포넌트
    protected BoxCollider2D m_Collider;
    #endregion
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
    #region 내부 함수
    protected void TriggerEnterEnemy(Collider2D collision)
    {
        if (!m_Player.isSafe)
        {
            m_Player.Death();
        }
    }
    protected void TriggerEnterCoin(Collider2D collision)
    {
        collision.GetComponent<CoinCollider>().coin.gameObject.SetActive(false);
    }
    protected void TriggerEnterSafetyZone(Collider2D collision)
    {
        m_Player.isSafe = true;
        m_Player.spawnPos = collision.transform.position;

        if (M_Game.isPlayMode)
        {
            bool isFinishZone = collision.GetComponent<SafetyZoneCollider>().isFinishZone;

            if (isFinishZone && !M_Coin.IsLeftCoin)
            {
                // 승리
                Debug.Log("승리");
            }
        }
    }
    protected void TriggerEnterGravityZone(Collider2D collision)
    {
        m_Player.rigidBody2D.useGravity = true;
        m_Player.rigidBody2D.gravity = collision.GetComponent<GravityZoneCollider>().gravityZone.gravity;
    }
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
        return m_Player.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Player;
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        int layerMask = LayerMask.GetMask("Enemy", "Coin", "GravityZone");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
        foreach (var item in colliders)
        {
            if (item.CompareTag("Enemy"))
            {
                TriggerEnterEnemy(item);
            }
            if (item.CompareTag("Coin"))
            {
                TriggerEnterCoin(item);
            }
            if (item.CompareTag("GravityZone"))
            {
                TriggerEnterGravityZone(item);
            }
        }
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
            if (collision.CompareTag("Enemy"))
            {
                TriggerEnterEnemy(collision);
            }

            if (collision.CompareTag("Coin"))
            {
                TriggerEnterCoin(collision);
            }

            if (collision.CompareTag("GravityZone"))
            {
                TriggerEnterGravityZone(collision);
            }
        }

        if (collision.CompareTag("SafetyZone"))
        {
            TriggerEnterSafetyZone(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone"))
        {
            int layerMask = LayerMask.GetMask("SafetyZone");
            Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
            if (colliders.Length <= 0)
            {
                m_Player.isSafe = false;
            }
        }

        if (M_Game.isPlayMode)
        {
            if (collision.CompareTag("GravityZone"))
            {
                int layerMask = LayerMask.GetMask("GravityZone");
                Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
                if (colliders.Length <= 0)
                {
                    m_Player.rigidBody2D.useGravity = false;
                    m_Player.rigidBody2D.gravity = MyRigidBody2D.Gravity;
                }
                else
                {
                    m_Player.rigidBody2D.gravity = colliders[0].GetComponent<GravityZoneCollider>().gravityZone.gravity;
                }
            }
        }
    }
    #endregion
}
