using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour, IEventSystemHandler
{
    [Space(10)]
    [SerializeField]
    protected ScrollRect m_ScrollRect;
    [SerializeField]
    protected CheckBoxItem m_Template;
    [Space(10)]
    [SerializeField]
    protected List<OptionData> m_Options;
    [Space(10)]
    [SerializeField]
    protected CheckBoxEvent m_OnValueChanged;

    protected List<CheckBoxItem> m_ItemList;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion
    protected RectTransform content => m_ScrollRect.content;
    protected CheckBoxItem template => m_Template;
    #endregion
    #region 외부 프로퍼티
    public ScrollRect scrollRect { get => m_ScrollRect; set => m_ScrollRect = value; }
    public CheckBoxEvent onValueChanged { get => m_OnValueChanged; set => m_OnValueChanged = value; }
    public List<OptionData> options
    {
        get => m_Options;
        set
        {
            m_Options = value;
            RefreshShownValue();
            ResizeContent();
        }
    }
    public List<CheckBoxItem> items
    {
        get => m_ItemList;
    }
    #endregion
    #region 내부 함수
    protected CheckBoxItem CreateItem(OptionData optionData)
    {
        int index = content.childCount - 1;
        CheckBoxItem item = CheckBoxItem.Instantiate(m_Template, content);

        if (null != item.toggle)
            item.toggle.isOn = optionData.isOn;
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
        int index = content.childCount - 1;
        float size = index * template.rectTransform.rect.height;
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
        OptionData optionData = new OptionData(option);
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
        if (index < 0 || index >= content.childCount - 1)
            return;

        m_Options.RemoveAt(index);
        m_ItemList.RemoveAt(index);
        GameObject.Destroy(content.GetChild(index + 1).gameObject);

        for (int i = index; i < m_ItemList.Count; ++i)
        {
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
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(content.GetChild(i));
        }

        for (int i = 0; i < m_Options.Count; ++i)
        {
            CreateItem(m_Options[i]);
        }
    }
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        m_ItemList = new List<CheckBoxItem>();

        if (null == m_Template)
        {
            m_Template = content.GetChild(0).GetComponent<CheckBoxItem>();
        }
        m_Template.gameObject.SetActive(false);

        for (int i = 0; i < m_Options.Count; ++i)
        {
            CreateItem(m_Options[i]);
        }

        m_OnValueChanged.AddListener(index =>
        {
            m_Options[index - 1].isOn = m_ItemList[index - 1].toggle.isOn;
        });
    }
    #endregion

    [System.Serializable]
    public class CheckBoxEvent : UnityEvent<int>
    {
        public CheckBoxEvent()
        {

        }
    }
    [System.Serializable]
    public class OptionData
    {
        [SerializeField]
        protected bool m_IsOn;
        [SerializeField]
        protected string m_Text;

        public OptionData()
        {
            m_IsOn = false;
            m_Text = "";
        }
        public OptionData(string text)
        {
            m_IsOn = false;
            m_Text = text;
        }
        
        public bool isOn { get => m_IsOn; set => m_IsOn = value; }
        public string text { get => m_Text; set => m_Text = value; }
    }
}
