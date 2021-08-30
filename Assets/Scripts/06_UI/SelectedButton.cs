using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerClickHandler
{
    [ReadOnly(true)]
    public E_SelectedType m_Type;

    EditManager M_Edit;
    InGameManager M_Game;

    Color m_Color;

    private void Awake()
    {
        M_Edit = EditManager.Instance;
        M_Game = InGameManager.Instance;
    }
    private void Start()
    {
        m_Color = Color.white;
    }

    private void Update()
    {
        if (M_Edit.isEdit &&
            !M_Edit.input_red.isFocused &&
            !M_Edit.input_green.isFocused &&
            !M_Edit.input_blue.isFocused &&
            !M_Edit.input_EnemySpeed.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                M_Edit.currentSelectedType = E_SelectedType.Player;

                if (m_Type == E_SelectedType.Player)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                M_Edit.currentSelectedType = E_SelectedType.Enemy;

                if (m_Type == E_SelectedType.Enemy)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                M_Edit.currentSelectedType = E_SelectedType.Coin;

                if (m_Type == E_SelectedType.Coin)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                M_Edit.currentSelectedType = E_SelectedType.SafetyZone;

                if (m_Type == E_SelectedType.SafetyZone)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                M_Edit.currentSelectedType = E_SelectedType.Wall;

                if (m_Type == E_SelectedType.Wall)
                {
                    UpdateSelected();
                }
            }
        }
    }
    public void UpdateSelected()
    {
        if (m_Type != E_SelectedType.None)
        {
            Image[] images = GetComponentsInChildren<Image>();
            foreach (var item in images)
            {
                if (item.name == "Image" &&
                    item.sprite != null)
                {
                    m_Color = item.color;
                    M_Edit.currentSelectedImage.sprite = item.sprite;
                    M_Edit.currentSelectedText.text = "Selected:" + "\n" + m_Type.ToString();
                }
            }

            M_Edit.currentSelectedImage.color = m_Color;
            M_Edit.UpdateOption();

            if (m_Type == E_SelectedType.SafetyZone ||
                m_Type == E_SelectedType.Wall)
            {
                M_Edit.currentSelectedImage.GetComponent<Outline>().enabled = true;
            }
            else
            {
                M_Edit.currentSelectedImage.GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            m_Color.a = 0;
            M_Edit.currentSelectedImage.color = m_Color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        M_Edit.currentSelectedType = m_Type;

        UpdateSelected();
    }
}
