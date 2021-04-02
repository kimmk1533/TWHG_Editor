using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerClickHandler
{
    [ReadOnly(true)]
    public E_SelectedType type;

    __EditManager m_Edit;
    __GameManager m_Game;

    Color color;

    private void Awake()
    {
        m_Edit = __EditManager.Instance;
        m_Game = __GameManager.Instance;
    }
    private void Start()
    {
        color = Color.white;
    }

    private void Update()
    {
        if (m_Edit.isEdit &&
            !m_Edit.input_red.isFocused &&
            !m_Edit.input_green.isFocused &&
            !m_Edit.input_blue.isFocused &&
            !m_Edit.input_EnemySpeed.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_Edit.currentSelectedType = E_SelectedType.Player;

                if (type == E_SelectedType.Player)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_Edit.currentSelectedType = E_SelectedType.Enemy;

                if (type == E_SelectedType.Enemy)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_Edit.currentSelectedType = E_SelectedType.Coin;

                if (type == E_SelectedType.Coin)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_Edit.currentSelectedType = E_SelectedType.SafetyZone;

                if (type == E_SelectedType.SafetyZone)
                {
                    UpdateSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                m_Edit.currentSelectedType = E_SelectedType.Wall;

                if (type == E_SelectedType.Wall)
                {
                    UpdateSelected();
                }
            }
        }
    }
    public void UpdateSelected()
    {
        if (type != E_SelectedType.None)
        {
            Image[] images = GetComponentsInChildren<Image>();
            foreach (var item in images)
            {
                if (item.name == "Image" &&
                    item.sprite != null)
                {
                    color = item.color;
                    m_Edit.currentSelectedImage.sprite = item.sprite;
                    m_Edit.currentSelectedText.text = "Selected:" + "\n" + type.ToString();
                }
            }

            m_Edit.currentSelectedImage.color = color;
            m_Edit.UpdateOption();

            if (type == E_SelectedType.SafetyZone ||
                type == E_SelectedType.Wall)
            {
                m_Edit.currentSelectedImage.GetComponent<Outline>().enabled = true;
            }
            else
            {
                m_Edit.currentSelectedImage.GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            color.a = 0;
            m_Edit.currentSelectedImage.color = color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_Edit.currentSelectedType = type;

        UpdateSelected();
    }
}
