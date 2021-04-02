using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollider : MonoBehaviour
{
    public Transform InitTransform;

    __EditManager m_Edit;
    CoinManager m_Coin;

    private void Awake()
    {
        m_Edit = __EditManager.Instance;
        m_Coin = CoinManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_Edit.isEdit)
        {
            if (collision.CompareTag("Player"))
            {
                CoinManager.Instance.GetPool().DeSpawn(this.transform.parent.gameObject);
            }
        }
    }
}
