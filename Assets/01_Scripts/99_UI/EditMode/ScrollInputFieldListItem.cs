using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public void __Initialize(ScrollInputFieldList scrollList)
    {
        m_ScrollList = scrollList;

        if (null == m_Text)
        {
            m_Text = GetComponentInChildren<Text>();
        }
        if (null == m_InputFieldList)
        {
            m_InputFieldList = new List<InputField>();
            m_InputFieldList.AddRange(GetComponentsInChildren<InputField>());

            foreach (var item in m_InputFieldList)
            {
                item.onValueChanged.AddListener(str => m_ScrollList.selectedIndex = m_Index);
                item.onValueChanged.AddListener(m_ScrollList.onItemValueChanged);
                item.onEndEdit.AddListener(str =>
                {
                    if (str == "")
                    {
                        item.text = "0";
                    }
                });
            }
        }
    }

    public string GetInputFieldValue(int index)
    {
        if (index < 0 || index >= m_InputFieldList.Count)
            return "";

        return m_InputFieldList[index].text;
    }
    public void SetInputFieldValue(int index, string value)
    {
        if (index < 0 || index >= m_InputFieldList.Count)
            return;

        m_InputFieldList[index].text = value;
    }
    #endregion
    #region 유니티 콜백 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        m_ScrollList.selectedIndex = m_Index;
    }
    #endregion
}
