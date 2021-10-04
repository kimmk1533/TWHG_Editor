using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class WallCollider : MonoBehaviour, IEraserableTile, IClickedObject
{
    protected Wall m_Wall;

    #region 내부 컴포넌트
    protected MyPhysics.BoxCollider2D m_Collider;
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
    #region 외부 함수
    public void __Initialize(Wall wall)
    {
        m_Wall = wall;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
        }
    }
    public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
    {
        if (currentType != E_ObjectType.Wall)
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
            collision.GetComponent<IEraserableObject>()?.EraseObject();
        }
    }
    #endregion
}
