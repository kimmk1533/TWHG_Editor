using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollInputFieldListItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
    protected ScrollInputFieldList m_ScrollList;
    protected int m_Index;

    #region 내부 컴포넌트
    [SerializeField]
    protected RectTransform m_RectTransform;
    [SerializeField]
    protected Text m_Text;

    protected List<InputField> m_InputFieldList;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public int index { get => m_Index; set => m_Index = value; }
    public RectTransform rectTransform { get => m_RectTransform; }
    public Text text { get => m_Text; set => m_Text = value; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public InputField.OnChangeEvent GetInputFieldValueChangedEvent(int index)
    {
        if (index < 0 || index >= m_InputFieldList.Count)
            return null;

        return m_InputFieldList[index].onValueChanged;
    }
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        if (null == m_ScrollList)
        {
            m_ScrollList = GetComponentInParent<ScrollInputFieldList>();
        }

        if (null == m_Text)
        {
            m_Text = GetComponentInChildren<Text>();
        }
        if (null == m_InputFieldList)
        {
            m_InputFieldList = new List<InputField>();
            m_InputFieldList.AddRange(GetComponentsInChildren<InputField>());
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        m_ScrollList.selectIndex = m_Index;
    }
    #endregion
}
