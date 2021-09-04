using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerClickHandler
{
    [ReadOnly(true)]
    public E_ObjectType m_Type;

    __EditManager M_Edit;
    __GameManager M_Game;

    Color m_Color;

    private void Awake()
    {
        M_Edit = __EditManager.Instance;
        M_Game = __GameManager.Instance;
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
                M_Edit.CurrentSelectedType = E_ObjectType.Player;

                if (m_Type == E_ObjectType.Player)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                M_Edit.CurrentSelectedType = E_ObjectType.Enemy;

                if (m_Type == E_ObjectType.Enemy)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                M_Edit.CurrentSelectedType = E_ObjectType.Coin;

                if (m_Type == E_ObjectType.Coin)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                M_Edit.CurrentSelectedType = E_ObjectType.SafetyZone;

                if (m_Type == E_ObjectType.SafetyZone)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                M_Edit.CurrentSelectedType = E_ObjectType.Wall;

                if (m_Type == E_ObjectType.Wall)
                {
                    UpdateSelected();
                }
            }
        }
    }
    public void UpdateSelected()
    {
        if (m_Type != E_ObjectType.None)
        {
            Image[] images = GetComponentsInChildren<Image>();
            foreach (var item in images)
            {
                if (item.name == "Image" &&
                    item.sprite != null)
                {
                    m_Color = item.color;
                    M_Edit.CurrentSelectedImage.sprite = item.sprite;
                    M_Edit.CurrentSelectedText.text = "Selected:" + "\n" + m_Type.ToString();
                }
            }

            M_Edit.CurrentSelectedImage.color = m_Color;
            M_Edit.UpdateOption();

            if (m_Type == E_ObjectType.SafetyZone ||
                m_Type == E_ObjectType.Wall)
            {
                M_Edit.CurrentSelectedImage.GetComponent<Outline>().enabled = true;
            }
            else
            {
                M_Edit.CurrentSelectedImage.GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            m_Color.a = 0;
            M_Edit.CurrentSelectedImage.color = m_Color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        M_Edit.CurrentSelectedType = m_Type;

        UpdateSelected();
    }
}
