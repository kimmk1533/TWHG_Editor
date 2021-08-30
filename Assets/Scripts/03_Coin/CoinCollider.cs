using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollider : MonoBehaviour
{
    public Transform m_InitTransform;

    public Coin m_Coin;

    public void __Initialize(Coin coin)
    {
        m_Coin = coin;
    }

    EditManager M_Edit => EditManager.Instance;
    CoinManager M_Coin => CoinManager.Instance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!M_Edit.isEdit)
        {
            if (collision.CompareTag("Player"))
            {
                M_Coin.GetPool("Coin").DeSpawn(m_Coin);
            }
        }
    }
}
