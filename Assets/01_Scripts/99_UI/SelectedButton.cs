using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerClickHandler
{
    [ReadOnly(true)]
    public E_ObjectType m_Type;

    #region 내부 컴포넌트
    protected Image m_ItemImage;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    #endregion
    #endregion

    Color m_Color;

    #region 유니티 콜백 함수
    private void Start()
    {
        m_Color = Color.white;
        if (null == m_ItemImage)
        {
            m_ItemImage = transform.Find("Image").GetComponent<Image>();
        }
    }
    #endregion
    public void UpdateSelected()
    {
        M_Edit.SetSelectedType(m_Type);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateSelected();
    }
}
