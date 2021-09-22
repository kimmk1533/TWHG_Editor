using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class WallCollider : MonoBehaviour, IEraserable, IClickedObject
{
    protected Wall m_Wall;
    protected BoxCollider2D m_Collider;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __EditManager M_Edit => __EditManager.Instance;

    protected WallManager M_Wall => WallManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Wall wall => m_Wall;
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

            m_Wall.lines[(int)i].gameObject.SetActive(true);

            if (null != m_Wall.walls[(int)i])
            {
                m_Wall.walls[(int)i].walls[index] = null;
                m_Wall.walls[(int)i].lines[index].gameObject.SetActive(true);
            }
        }

        M_Wall.DespawnWall(m_Wall);
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return null;
    }
    public GameObject GetGameObject()
    {
        return m_Wall.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Wall;
    }
    #endregion
    #region 유니티 콜백 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (M_Edit.isEditMode)
        {
            collision.GetComponent<IEraserable>()?.Erase();
        }
    }
    #endregion
}
