using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollider : MonoBehaviour, IEraserable, IClickedObject
{
    // 관리 컴포넌트
    [SerializeField, ReadOnly]
    protected Coin m_Coin;

    #region 내부 프로퍼티
    #region 매니져
    protected __EditManager M_Edit => __EditManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Coin coin { get => m_Coin; }
    #endregion
    #region 외부 함수
    public void __Initialize(Coin coin)
    {
        m_Coin = coin;
    }

    public void Erase()
    {
        M_Coin.DespawnCoin(m_Coin);
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return m_Coin.renderer;
    }
    public GameObject GetGameObject()
    {
        return m_Coin.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Coin;
    }
    #endregion
    #region 유니티 콜백 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!M_Edit.isEditMode)
        {
            if (collision.CompareTag("Player"))
            {
                --M_Coin.CoinCount;
            }
        }
    }
    #endregion
}
