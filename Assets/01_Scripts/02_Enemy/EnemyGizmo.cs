using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyGizmo : MonoBehaviour
{
    protected Enemy m_Enemy;

    protected int m_Index;

    #region 내부 컴포넌트
    protected SpriteRenderer m_Renderer;
    protected TextMeshPro m_Text;
    protected CircleCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public Enemy enemy { get => m_Enemy; set => m_Enemy = value; }
    public TextMeshPro text { get => m_Text; set => m_Text = value; }
    public int index { get => m_Index; set => m_Index = value; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        if (null == m_Renderer)
        {
            m_Renderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (null == m_Text)
        {
            m_Text = GetComponentInChildren<TextMeshPro>();
        }
        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<CircleCollider2D>();
        }
    }
    #endregion
}
