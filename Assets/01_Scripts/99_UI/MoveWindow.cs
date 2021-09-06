using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveWindow : MonoBehaviour
{
    public E_Type m_Type;
    public float m_Duration = 1f;
    public Vector3 m_MoveDistance;

    bool m_IsMove;
    bool m_Flag;

    private void Awake()
    {
        m_IsMove = false;
        m_Flag = false;
    }

    public void ButtonPressed()
    {
        if (m_IsMove)
            return;

        StartCoroutine(Move());

        m_Flag = !m_Flag;
    }

    IEnumerator Move()
    {
        m_IsMove = true;

        int dir = m_Flag ? -1 : 1;

        switch (m_Type)
        {
            case E_Type.Direct:
                transform.localPosition += dir * m_MoveDistance;
                break;
            case E_Type.Smooth:
                float timer = 0f;
                Vector3 distance; 

                while (timer < m_Duration)
                {
                    distance = dir * m_MoveDistance * Time.deltaTime / m_Duration;

                    transform.localPosition += distance;

                    timer += Time.deltaTime;

                    yield return null;
                }

                // 오차 범위 수정
                distance = dir * m_MoveDistance * (m_Duration - timer) / m_Duration;

                transform.localPosition += distance;
                break;
        }

        m_IsMove = false;
    }

    public enum E_Type
    {
        None = -1,

        Direct,
        Smooth
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
