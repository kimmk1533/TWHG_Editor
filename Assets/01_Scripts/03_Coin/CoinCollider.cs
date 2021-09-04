using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollider : MonoBehaviour, IEraserable
{
    // 관리 컴포넌트
    [SerializeField, ReadOnly]
    protected Coin m_Coin;

    #region 내부 프로퍼티
    protected __EditManager M_Edit => __EditManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
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
    #endregion

    #region 유니티 콜백 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!M_Edit.isEdit)
        {
            if (collision.CompareTag("Player"))
            {
                --M_Coin.CoinCount;
                gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
