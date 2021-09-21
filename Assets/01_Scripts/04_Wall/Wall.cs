using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class Wall : MonoBehaviour
{
    protected Tile m_Tile;
    [SerializeField, ReadOnly]
    protected Wall[] m_Walls;

    #region 내부 컴포넌트
    protected LineRenderer[] m_Lines;
    protected WallCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public Tile tile => m_Tile;
    public Wall[] walls => m_Walls;
    public LineRenderer[] lines => m_Lines;
    public new WallCollider collider => m_Collider;
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize(Tile tile)
    {
        m_Tile = tile;

        m_Walls = new Wall[(int)E_WallDirection.Max];
        m_Lines = new LineRenderer[(int)E_WallDirection.Max];

        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<WallCollider>();
            m_Collider.__Initialize(this);
        }

        int layerMask = LayerMask.GetMask("Wall");
        RaycastHit2D[] hits = new RaycastHit2D[(int)E_WallDirection.Max];
        hits[0] = Physics2D.Raycast(transform.position, Vector2.left, 1f, layerMask);
        hits[1] = Physics2D.Raycast(transform.position, Vector2.right, 1f, layerMask);
        hits[2] = Physics2D.Raycast(transform.position, Vector2.up, 1f, layerMask);
        hits[3] = Physics2D.Raycast(transform.position, Vector2.down, 1f, layerMask);

        for (E_WallDirection i = 0; i < E_WallDirection.Max; ++i)
        {
            if (null == m_Lines[(int)i])
            {
                m_Lines[(int)i] = transform.Find(i.ToString()).GetComponent<LineRenderer>();
            }

            if (null != hits[(int)i].transform)
            {
                m_Walls[(int)i] = hits[(int)i].transform.GetComponentInParent<Wall>();

                if (null != m_Walls[(int)i])
                {
                    int index = (int)(i + 1 - (int)i % 2 * 2);
                    m_Walls[(int)i].m_Walls[index] = this;

                    m_Lines[(int)i].gameObject.SetActive(false);
                    m_Walls[(int)i].m_Lines[index].gameObject.SetActive(false);
                }
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
