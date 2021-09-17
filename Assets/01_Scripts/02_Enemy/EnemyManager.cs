using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ObjectManager<EnemyManager, Enemy>
{
    protected List<Enemy> m_EnemyList;

    #region 외부 프로퍼티
    public List<Enemy> enemyList { get => m_EnemyList; }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        // 적 풀 원본
        Enemy enemy = M_Resources.GetGameObject<Enemy>("Enemy", "Enemy");
        // 적 풀 생성
        AddPool("Enemy", enemy, transform);

        if (null == m_EnemyList)
        {
            m_EnemyList = new List<Enemy>();
        }
    }
    public override void __Finalize()
    {
        base.__Finalize();
    }

    public Enemy SpawnEnemy()
    {
        Enemy enemy = GetPool("Enemy").Spawn();
        enemy.__Initialize();
        enemy.gameObject.SetActive(true);
        m_EnemyList.Add(enemy);

        return enemy;
    }
    public void DespawnEnemy(Enemy enemy)
    {
        m_EnemyList.Remove(enemy);
        GetPool("Enemy").DeSpawn(enemy);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {
        foreach (var item in m_EnemyList)
        {
            item.OnPlayEnter();
        }
    }
    public override void OnPlayExit()
    {
        foreach (var item in m_EnemyList)
        {
            item.OnPlayExit();
        }
    }
    #endregion
}