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
    //[Space(10)]
    //[SerializeField]
    //protected Text m_ItemText;
    //[SerializeField]
    //protected Image m_ItemImage;
    [Space(10)]
    [SerializeField]
    protected List<OptionData> m_Options;
    [Space(10)]
    [SerializeField]
    protected CheckBoxEvent m_OnValueChanged;

    protected List<CheckBoxItem> m_Items;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public ScrollRect scrollRect { get => m_ScrollRect; set => m_ScrollRect = value; }
    public CheckBoxEvent onValueChanged { get => m_OnValueChanged; set => m_OnValueChanged = value; }
    //public Text itemText { get => m_ItemText; set => m_ItemText = value; }
    //public Image itemImage { get => m_ItemImage; set => m_ItemImage = value; }
    public List<OptionData> options { get => m_Options; set => m_Options = value; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        if (null == m_Items)
        {
            m_Items = new List<CheckBoxItem>();
        }

        CheckBoxItem template = m_ScrollRect.content.GetChild(0).GetComponent<CheckBoxItem>();
        template.gameObject.SetActive(false);
        for (int i = 0; i < m_Options.Count; ++i)
        {
            CheckBoxItem item = CheckBoxItem.Instantiate(template, m_ScrollRect.content);

            if (null != item.toggle)
                item.toggle.isOn = m_Options[i].isOn;
            if (null != item.text)
                item.text.text = m_Options[i].text;
            if (null != item.image)
                item.image.sprite = m_Options[i].image;

            Vector3 position = item.rectTransform.anchoredPosition3D;
            position.y = -i * item.rectTransform.rect.height;
            item.rectTransform.anchoredPosition3D = position;
            item.name = "Item " + i + ": " + m_Options[i].text;

            item.gameObject.SetActive(true);
        }
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
        [SerializeField]
        protected Sprite m_Image;

        public OptionData()
        {
            m_IsOn = false;
            m_Text = "";
            m_Image = null;
        }
        public OptionData(string text)
        {
            m_IsOn = false;
            m_Text = text;
            m_Image = null;
        }
        public OptionData(Sprite image)
        {
            m_IsOn = false;
            m_Text = "";
            m_Image = image;
        }
        public OptionData(string text, Sprite image)
        {
            m_IsOn = false;
            m_Text = text;
            m_Image = image;
        }

        public bool isOn { get => m_IsOn; set => m_IsOn = value; }
        public string text { get => m_Text; set => m_Text = value; }
        public Sprite image { get => m_Image; set => m_Image = value; }
    }
}
