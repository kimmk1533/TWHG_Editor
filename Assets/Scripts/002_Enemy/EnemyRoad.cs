using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyRoad : MonoBehaviour
{
    public float Speed;

    public GameObject Enemy;
    public Vector2 InitPos;

    protected __GameManager m_Game;
    protected __EditManager m_Edit;
    protected EnemyManager m_Enemy;

    protected virtual void Awake()
    {
        m_Game = __GameManager.Instance;
        m_Edit = __EditManager.Instance;
        m_Enemy = EnemyManager.Instance;
    }
    private void Start()
    {

    }

    public virtual void OnPlayEnter()
    {
        Enemy.transform.position = InitPos;
    }
    public virtual void OnPlayExit()
    {
        Enemy.transform.position = InitPos;
    }

    public void Despawn()
    {
        m_Enemy.GetPool().DeSpawn(Enemy);
    }

    protected abstract void Move();
}
