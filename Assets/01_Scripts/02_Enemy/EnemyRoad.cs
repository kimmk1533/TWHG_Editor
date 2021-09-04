using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyRoad : MonoBehaviour
{
    public float m_Speed;

    public Enemy m_Enemy;
    public Vector2 m_InitPos;

    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;

    public virtual void OnPlayEnter()
    {
        m_Enemy.transform.position = m_InitPos;
    }
    public virtual void OnPlayExit()
    {
        m_Enemy.transform.position = m_InitPos;
    }

    public void Despawn()
    {
        M_Enemy.GetPool("Enemy").DeSpawn(m_Enemy);
    }

    protected abstract void Move();
}
