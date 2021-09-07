using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class WallCollider : MonoBehaviour, IEraserable
{
    protected Wall m_Wall;
    protected BoxCollider2D m_Collider;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected WallManager M_Wall => WallManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize(Wall wall)
    {
        m_Wall = wall;

        if (null == m_Collider)
        {
            GetComponent<BoxCollider2D>();
        }
    }
    public void Erase()
    {
        for (E_WallDirection i = 0; i < E_WallDirection.Max; ++i)
        {
            int index = (int)(i + 1 - (int)i % 2 * 2);

            m_Wall.Lines[(int)i].gameObject.SetActive(true);

            if (null != m_Wall.Walls[(int)i])
            {
                m_Wall.Walls[(int)i].Walls[index] = null;
                m_Wall.Walls[(int)i].Lines[index].gameObject.SetActive(true);
            }
        }

        M_Wall.DespawnWall(m_Wall);
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