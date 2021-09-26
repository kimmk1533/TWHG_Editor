using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollInputFieldList : MonoBehaviour, IEventSystemHandler
{
    [SerializeField, ReadOnly]
    protected int m_SelectedIndex;

    [Space(10)]
    [SerializeField]
    protected ScrollRect m_ScrollRect;
    [SerializeField]
    protected ScrollInputFieldListItem m_Template;
    [SerializeField]
    protected Button m_AddButton;
    [SerializeField]
    protected Button m_RemoveButton;
    [SerializeField]
    protected InputField m_SelectedIndexInputField;
    [Space(10)]
    [SerializeField]
    protected List<OptionData> m_Options;
    [Space(10)]
    [SerializeField]
    protected Button.ButtonClickedEvent m_OnAddButtonClicked;
    [SerializeField]
    protected Button.ButtonClickedEvent m_OnRemoveButtonClicked;
    [SerializeField]
    protected UnityAction<string> m_OnItemValueChanged;
    [SerializeField]
    protected InputField.SubmitEvent m_OnSelectedIndexEndEdit;

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
    public Button.ButtonClickedEvent onAddButtonClicked { get => m_OnAddButtonClicked; set => m_OnAddButtonClicked = value; }
    public Button.ButtonClickedEvent onRemoveButtonClicked { get => m_OnRemoveButtonClicked; set => m_OnRemoveButtonClicked = value; }
    public UnityAction<string> onItemValueChanged { get => m_OnItemValueChanged; set => m_OnItemValueChanged = value; }
    public InputField.SubmitEvent onSelectedIndexEndEdit { get => m_OnSelectedIndexEndEdit; set => m_OnSelectedIndexEndEdit = value; }
    public List<OptionData> options
    {
        get => m_Options;
        set
        {
            m_Options = value;
            RefreshShownValue();
        }
    }
    public List<ScrollInputFieldListItem> items
    {
        get => m_ItemList;
        set
        {
            m_ItemList = value;
            m_Options.Clear();
            for (int i = 0; i < m_ItemList.Count; ++i)
            {
                m_Options.Add(new OptionData(this, m_ItemList[i].text.text));
            }

            RefreshShownValue();
        }
    }
    public int Count { get => content.childCount - 1; }
    public int selectedIndex
    {
        get => m_SelectedIndex;
        set
        {
            m_SelectedIndex = value;
            m_SelectedIndexInputField.text = (m_SelectedIndex + 1).ToString();
        }
    }
    public InputField selectedIndexInputField { get => m_SelectedIndexInputField; set => m_SelectedIndexInputField = value; }
    #endregion
    #region 내부 함수
    protected ScrollInputFieldListItem CreateItem(OptionData optionData)
    {
        int index = Count;
        ScrollInputFieldListItem item = ScrollInputFieldListItem.Instantiate(m_Template, content);

        item.index = optionData.index;
        if (null != item.text)
            item.text.text = optionData.text;

        Vector3 position = item.rectTransform.anchoredPosition3D;
        position.y = -index * item.rectTransform.rect.height;
        item.rectTransform.anchoredPosition3D = position;

        item.name = "Item " + index + ": " + optionData.text;

        item.__Initialize(this);
        item.gameObject.SetActive(true);
        m_ItemList.Add(item);

        return item;
    }
    protected void ResizeContent()
    {
        float size = Count * template.rectTransform.rect.height;
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
        if (index < 0 || index >= Count)
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
    public void ClearOption()
    {
        int count = m_ItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            m_Options.RemoveAt(0);
            m_ItemList.RemoveAt(0);
            GameObject.DestroyImmediate(content.GetChild(1).gameObject);
        }
        m_SelectedIndex = Count;
        m_SelectedIndexInputField.text = (m_SelectedIndex + 1).ToString();

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
    public ScrollInputFieldListItem GetInputFieldItem(int index)
    {
        if (index < 0 || index >= m_ItemList.Count)
            return null;

        return m_ItemList[index];
    }
    #endregion
    #region 이벤트 함수
    protected void OnAddButtonClicked()
    {
        m_SelectedIndex = Count;
        m_SelectedIndexInputField.text = (m_SelectedIndex + 1).ToString();
        AddOption((Count + 1).ToString());
    }
    protected void OnRemoveButtonClicked()
    {
        RemoveOption(m_SelectedIndex);
        m_SelectedIndex = Mathf.Clamp(m_SelectedIndex, Mathf.Min(0, Count - 1), Mathf.Max(0, Count - 1));
        m_SelectedIndexInputField.text = (m_SelectedIndex + 1).ToString();
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

        m_OnAddButtonClicked = m_AddButton.onClick;
        m_AddButton.onClick.AddListener(() =>
        {
            OnAddButtonClicked();
        });
        m_OnRemoveButtonClicked = m_RemoveButton.onClick;
        m_RemoveButton.onClick.AddListener(() =>
        {
            OnRemoveButtonClicked();
        });

        m_SelectedIndex = Count;
        m_SelectedIndexInputField.text = (m_SelectedIndex + 1).ToString();
        m_OnSelectedIndexEndEdit = m_SelectedIndexInputField.onEndEdit;
    }
    #endregion

    [System.Serializable]
    public class OptionData
    {
        [SerializeField]
        protected int m_Index;
        [SerializeField]
        protected string m_Text;

        public OptionData(ScrollInputFieldList scrollList)
        {
            m_Index = scrollList.Count;
            m_Text = "";
        }
        public OptionData(ScrollInputFieldList scrollList, string text)
        {
            m_Index = scrollList.Count;
            m_Text = text;
        }

        public int index { get => m_Index; set => m_Index = value; }
        public string text { get => m_Text; set => m_Text = value; }
    }
}
