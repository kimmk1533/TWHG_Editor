using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCollider : MonoBehaviour
{
    Animator m_Animator;

    public Vector2 m_SpawnPoint;

    public bool m_IsSafety;

    InGameManager M_Game;
    EditManager M_Edit;
    SafetyZoneManager M_SafetyZone;

    private void Awake()
    {
        M_Game = InGameManager.Instance;
        M_Edit = EditManager.Instance;
        M_SafetyZone = SafetyZoneManager.Instance;
    }

    private void Start()
    {
        m_Animator = transform.parent.GetComponentInChildren<Animator>();
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
    }

    public void OnPlayEnter()
    {
        // 시작 위치 설정
        m_SpawnPoint = M_SafetyZone.m_StartPoint.GetCenter();
        m_Animator.GetComponent<PlayerAnim>().m_SpawnPoint = m_SpawnPoint;
    }
    public void OnPlayExit()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!M_Edit.isEdit)
        {
            if (collision.CompareTag("SafetyZone"))
            {
                m_IsSafety = true;

                PolygonCollider2D poly = collision as PolygonCollider2D;

                for (int i = 0; i < M_SafetyZone.m_Colliders.Count; ++i)
                {
                    if (poly == M_SafetyZone.m_Colliders[i].m_Polygon)
                    {
                        if (poly != M_SafetyZone.m_EndPoint.m_Polygon)
                        {
                            m_SpawnPoint = M_SafetyZone.m_Colliders[i].GetCenter();
                            m_Animator.GetComponent<PlayerAnim>().m_SpawnPoint = m_SpawnPoint;
                        }
                        else
                        {
                            if (!CoinManager.Instance.IsLeftedCoin())
                            {
                                // 승리
                                // 임시
                                __SceneManager.LoadMainMenuScene();
                                Debug.Log("승리");
                            }
                        }
                    }
                }
            }

            if (collision.CompareTag("Enemy") && !m_IsSafety)
            {
                m_Animator.SetBool("Death", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone"))
        {
            m_IsSafety = false;
        }
    }
}
