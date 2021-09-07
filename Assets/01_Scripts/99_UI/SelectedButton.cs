using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : Selectable, IPointerClickHandler
{
    [Space(15)]
    [ReadOnly(true)]
    public E_ObjectType m_Type;

    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __EditManager M_Edit => __EditManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    protected void UpdateSelected()
    {
        M_Edit.SetSelectedType(m_Type);
    }
    #endregion
    #region 유니티 콜백 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UpdateSelected();
        }
    }
    #endregion
}
