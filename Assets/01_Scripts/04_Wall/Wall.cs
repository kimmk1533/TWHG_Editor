using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class Wall : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected Wall[] m_Walls;

    #region 내부 컴포넌트
    protected LineRenderer[] m_Lines;
    protected WallCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public Wall[] Walls => m_Walls;
    public LineRenderer[] Lines => m_Lines;
    public WallCollider Collider => m_Collider;
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        m_Walls = new Wall[(int)E_WallDirection.Max];
        m_Lines = new LineRenderer[(int)E_WallDirection.Max];

        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<WallCollider>();
            m_Collider.__Initialize(this);
        }

        RaycastHit2D[] hits = new RaycastHit2D[(int)E_WallDirection.Max];
        hits[0] = Physics2D.Raycast(transform.position, Vector2.left, 1f);
        hits[1] = Physics2D.Raycast(transform.position, Vector2.right, 1f);
        hits[2] = Physics2D.Raycast(transform.position, Vector2.up, 1f);
        hits[3] = Physics2D.Raycast(transform.position, Vector2.down, 1f);

        for (E_WallDirection i = 0; i < E_WallDirection.Max; ++i)
        {
            if (null == m_Lines[(int)i])
            {
                m_Lines[(int)i] = transform.Find(i.ToString()).GetComponent<LineRenderer>();
            }

            if (null != hits[(int)i].transform)
            {
                m_Walls[(int)i] = hits[(int)i].transform.GetComponentInParent<Wall>();

                int index = (int)(i + 1 - (int)i % 2 * 2);
                m_Walls[(int)i].m_Walls[index] = this;

                m_Lines[(int)i].gameObject.SetActive(false);
                m_Walls[(int)i].m_Lines[index].gameObject.SetActive(false);
            }
        }
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
