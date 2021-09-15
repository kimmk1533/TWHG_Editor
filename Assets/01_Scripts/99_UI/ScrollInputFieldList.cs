using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollInputFieldList : MonoBehaviour, IEventSystemHandler
{
    [Space(10)]
    [SerializeField]
    protected ScrollRect m_ScrollRect;
    [SerializeField]
    protected ScrollInputFieldListItem m_Template;
    [SerializeField]
    protected Button m_AddButton;
    [SerializeField]
    protected Button m_RemoveButton;
    [Space(10)]
    [SerializeField]
    protected List<OptionData> m_Options;
    [Space(10)]
    [SerializeField]
    protected ScrollListEvent m_OnValueChanged;

    [SerializeField, ReadOnly]
    protected int m_SelectIndex;
    protected List<ScrollInputFieldListItem> m_ItemList;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion

    protected RectTransform content => m_ScrollRect.content;
    protected ScrollInputFieldListItem template => m_Template;
    #endregion
    #region 외부 프로퍼티
    public ScrollRect scrollRect { get => m_ScrollRect; set => m_ScrollRect = value; }
    public ScrollListEvent onValueChanged { get => m_OnValueChanged; set => m_OnValueChanged = value; }
    public List<OptionData> options
    {
        get => m_Options;
        set
        {
            m_Options = value;
            RefreshShownValue();
        }
    }
    public int itemIndex => content.childCount - 1; 
    public int selectIndex { get => m_SelectIndex; set => m_SelectIndex = value; }
    #endregion
    #region 내부 함수
    protected ScrollInputFieldListItem CreateItem(OptionData optionData)
    {
        int index = itemIndex;
        ScrollInputFieldListItem item = ScrollInputFieldListItem.Instantiate(m_Template, content);

        item.index = index;
        if (null != item.text)
            item.text.text = optionData.text;

        Vector3 position = item.rectTransform.anchoredPosition3D;
        position.y = -index * item.rectTransform.rect.height;
        item.rectTransform.anchoredPosition3D = position;

        item.name = "Item " + index + ": " + optionData.text;

        item.gameObject.SetActive(true);
        m_ItemList.Add(item);

        return item;
    }
    protected void ResizeContent()
    {
        float size = itemIndex * template.rectTransform.rect.height;
        content.sizeDelta = new Vector2(content.sizeDelta.x, size);
    }
    #endregion
    #region 외부 함수
    public void AddOption(OptionData option)
    {
        m_Options.Add(option);
        CreateItem(option);

        ResizeContent();
    }
    public void AddOption(string option)
    {
        OptionData optionData = new OptionData(this, option);
        m_Options.Add(optionData);
        CreateItem(optionData);

        ResizeContent();
    }
    public void AddOptions(List<OptionData> options)
    {
        foreach (var item in options)
        {
            AddOption(item);
        }
    }
    public void AddOptions(List<string> options)
    {
        foreach (var item in options)
        {
            AddOption(item);
        }
    }
    public void RemoveOption(int index)
    {
        if (index < 0 || index >= itemIndex)
            return;

        m_Options.RemoveAt(index);
        m_ItemList.RemoveAt(index);
        GameObject.DestroyImmediate(content.GetChild(index + 1).gameObject);

        for (int i = index; i < m_ItemList.Count; ++i)
        {
            m_Options[i].index = i;
            m_ItemList[i].index = i;
            m_Options[i].text = (i + 1).ToString();
            m_ItemList[i].text.text = (i + 1).ToString();

            Vector3 position = m_ItemList[i].rectTransform.anchoredPosition3D;
            position.y = -i * m_ItemList[i].rectTransform.rect.height;
            m_ItemList[i].rectTransform.anchoredPosition3D = position;
        }

        ResizeContent();
    }
    public void RefreshShownValue()
    {
        for (int i = content.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(content.GetChild(i));
        }

        for (int i = 0; i < m_Options.Count; ++i)
        {
            CreateItem(m_Options[i]);
        }

        ResizeContent();
    }
    #endregion
    #region 이벤트 함수
    public void OnAddButtonClicked()
    {
        m_SelectIndex = itemIndex;
        AddOption((itemIndex + 1).ToString());
    }
    public void OnRemoveButtonClicked()
    {
        RemoveOption(m_SelectIndex);
        m_SelectIndex = Mathf.Clamp(m_SelectIndex, 0, itemIndex - 1);
    }
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        m_ItemList = new List<ScrollInputFieldListItem>();

        if (null == m_Template)
        {
            m_Template = content.GetChild(0).GetComponent<ScrollInputFieldListItem>();
        }
        m_Template.gameObject.SetActive(false);

        for (int i = 0; i < m_Options.Count; ++i)
        {
            CreateItem(m_Options[i]);
        }

        m_OnValueChanged.AddListener(index =>
        {
            m_Options[index - 1].index = m_ItemList[index - 1].index;
        });

        m_AddButton.onClick.AddListener(() =>
        {
            OnAddButtonClicked();
        });
        m_RemoveButton.onClick.AddListener(() =>
        {
            OnRemoveButtonClicked();
        });

        m_SelectIndex = itemIndex;
    }
    #endregion

    [System.Serializable]
    public class ScrollListEvent : UnityEvent<int>
    {
        public ScrollListEvent()
        {

        }
    }
    [System.Serializable]
    public class OptionData
    {
        [SerializeField]
        protected int m_Index;
        [SerializeField]
        protected string m_Text;

        public OptionData(ScrollInputFieldList scrollList)
        {
            m_Index = scrollList.itemIndex;
            m_Text = "";
        }
        public OptionData(ScrollInputFieldList scrollList, string text)
        {
            m_Index = scrollList.itemIndex;
            m_Text = text;
        }

        public int index { get => m_Index; set => m_Index = value; }
        public string text { get => m_Text; set => m_Text = value; }
    }
}
