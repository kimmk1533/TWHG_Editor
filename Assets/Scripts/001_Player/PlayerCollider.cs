using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCollider : MonoBehaviour
{
    Animator animator;

    public Vector2 SpawnPoint;

    public bool IsSafety;

    __GameManager m_Game;
    __EditManager m_Edit;
    SafetyZoneManager m_SafetyZone;

    private void Awake()
    {
        m_Game = __GameManager.Instance;
        m_Edit = __EditManager.Instance;
        m_SafetyZone = SafetyZoneManager.Instance;
    }

    private void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
        m_Game.OnPlayEnter += OnPlayEnter;
        m_Game.OnPlayExit += OnPlayExit;
    }

    public void OnPlayEnter()
    {
        // 시작 위치 설정
        SpawnPoint = m_SafetyZone.StartPoint.GetCenter();
        animator.GetComponent<PlayerAnim>().SpawnPoint = SpawnPoint;
    }
    public void OnPlayExit()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_Edit.isEdit)
        {
            if (collision.CompareTag("SafetyZone"))
            {
                IsSafety = true;

                PolygonCollider2D poly = collision as PolygonCollider2D;

                for (int i = 0; i < m_SafetyZone.Colliders.Count; ++i)
                {
                    if (poly == m_SafetyZone.Colliders[i].Polygon)
                    {
                        if (poly != m_SafetyZone.EndPoint.Polygon)
                        {
                            SpawnPoint = m_SafetyZone.Colliders[i].GetCenter();
                            animator.GetComponent<PlayerAnim>().SpawnPoint = SpawnPoint;
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

            if (collision.CompareTag("Enemy") && !IsSafety)
            {
                animator.SetBool("Death", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SafetyZone"))
        {
            IsSafety = false;
        }
    }
}
