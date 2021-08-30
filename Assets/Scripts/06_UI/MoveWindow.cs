using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveWindow : MonoBehaviour
{
    public Vector3 m_MoveDistance;

    bool m_Flag;

    private void Awake()
    {
        m_Flag = false;
    }

    public void ButtonPressed()
    {
        if (!m_Flag)
        {
            transform.localPosition = transform.localPosition + m_MoveDistance;
        }
        else
        {
            transform.localPosition = transform.localPosition - m_MoveDistance;
        }

        m_Flag = !m_Flag;
    }

    //public Sprite UpButtonSprite;
    //public Sprite DownButtonSprite;

    //Vector3 UpPos;
    //Vector3 DownPos;
    //RectTransform rc;

    //Image image;
    //RectTransform image_rc;
    //Vector3 image_pos;

    //private void Start()
    //{
    //    rc = GetComponent<RectTransform>();

    //    UpPos = DownPos = rc.anchoredPosition3D;
    //    UpPos.y += rc.rect.height * 2;
    //}

    //public void MoveButtonPressed(Image image)
    //{
    //    if (this.image == null)
    //    {
    //        this.image = image;
    //        image_rc = this.image.GetComponent<RectTransform>();
    //        image_pos = image_rc.position;
    //    }

    //    // 올라가기
    //    if (rc.anchoredPosition3D == DownPos)
    //    {
    //        MoveUp();
    //    }
    //    // 내려가기
    //    else if (rc.anchoredPosition3D == UpPos)
    //    {
    //        MoveDown();
    //    }

    //    image_rc.position = image_pos;
    //}

    //public void MoveUp()
    //{
    //    rc.anchoredPosition3D = UpPos;
    //    image.sprite = DownButtonSprite;
    //}
    //public void MoveDown()
    //{
    //    rc.anchoredPosition3D = DownPos;
    //    image.sprite = UpButtonSprite;
    //}
}
