using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour, IEraserable
{
    protected Enemy m_Enemy;

    #region 내부 컴포넌트
    protected CircleCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize(Enemy enemy)
    {
        m_Enemy = enemy;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<CircleCollider2D>();
        }
    }

    public void Erase()
    {
        M_Enemy.DespawnEnemy(m_Enemy);
    }
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        
    }
    void Update()
    {
        
    }
    #endregion
}
