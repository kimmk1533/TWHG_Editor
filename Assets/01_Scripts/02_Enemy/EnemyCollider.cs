using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour, IEraserableObject, IClickedObject
{
    protected Enemy m_Enemy;

    #region 내부 컴포넌트
    protected MyPhysics.CircleCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Enemy enemy => m_Enemy;
    #endregion
    #region 외부 함수
    public void __Initialize(Enemy enemy)
    {
        m_Enemy = enemy;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<MyPhysics.CircleCollider2D>();
        }
    }

    public void EraseObject()
    {
        for (int i = 0; i < m_Enemy.wayPointList.Count; ++i)
        {
            M_EnemyGizmo.DespawnGizmo(m_Enemy.wayPointList[i]);
        }

        M_Enemy.DespawnEnemy(m_Enemy);
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return m_Enemy.renderer;
    }
    public GameObject GetGameObject()
    {
        return m_Enemy.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Enemy;
    }
    #endregion
}
