using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckBoxItem : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
    protected CheckBox m_CheckBox;

    #region 내부 컴포넌트
    [SerializeField]
    protected RectTransform m_RectTransform;
    [SerializeField]
    protected Toggle m_Toggle;
    [SerializeField]
    protected Text m_Text;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public int index
    {
        get
        {
            int childCount = transform.parent.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                if (transform.parent.GetChild(i).Equals(transform))
                    return i;
            }

            return -1;
        }
    }
    public Text text { get => m_Text; set => m_Text = value; }
    public RectTransform rectTransform { get => m_RectTransform; set => m_RectTransform = value; }
    public Toggle toggle { get => m_Toggle; set => m_Toggle = value; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        if (null == m_CheckBox)
        {
            m_CheckBox = GetComponentInParent<CheckBox>();
        }

        if (null == m_RectTransform)
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        if (null == m_Toggle)
        {
            m_Toggle = GetComponent<Toggle>();
        }
        if (null == m_Text)
        {
            m_Text = GetComponentInChildren<Text>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_CheckBox?.onValueChanged?.Invoke(index);
    }
    #endregion
}