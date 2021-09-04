using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : ObjectManager<WallManager, Wall>
{
    // 관리 리스트
    List<Wall> m_WallList;

    #region 내부 프로퍼티
    #region 매니져
    protected StageManager M_Stage => StageManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    protected void ClearWall()
    {
        for (int i = 0; i < m_WallList.Count; ++i)
        {
            m_WallList[i].Collider.Erase();
        }

        m_WallList.Clear();
    }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        #region 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
        #endregion

        // 풀 사이즈 설정
        m_PoolSize = M_Game.m_width * M_Game.m_height;

        // 벽 풀 원본
        Wall wall = M_Resources.GetGameObject<Wall>("Wall", "Wall");
        // 벽 풀 생성
        AddPool("Wall", wall, transform);

        // 관리 리스트 초기화
        if (null == m_WallList)
        {
            m_WallList = new List<Wall>();
        }
    }
    public override void __Finalize()
    {
        ClearWall();

        base.__Finalize();
    }

    public Wall SpawnWall()
    {
        // 스폰
        Wall wall = GetPool("Wall").Spawn();
        // 관리 리스트에 추가
        m_WallList.Add(wall);

        return wall;
    }
    public void DespawnWall(Wall wall)
    {
        m_WallList.Remove(wall);
        GetPool("Wall").DeSpawn(wall);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {

    }
    public override void OnPlayExit()
    {

    }
    #endregion

    public enum E_WallDirection
    {
        Left,
        Right,
        Top,
        Bottom,

        Max
    }
}