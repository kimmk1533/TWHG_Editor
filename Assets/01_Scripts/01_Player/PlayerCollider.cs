using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour, IEraserableObject, IClickedObject
{
    protected Player m_Player;

    protected delegate void OnTrigger(Collider2D collider);
    protected Dictionary<string, OnTrigger> m_TriggerEnter;
    protected Dictionary<string, OnTrigger> m_TriggerExit;

    #region 내부 컴포넌트
    protected BoxCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    protected StageManager M_Stage => StageManager.Instance;

    protected CoinManager M_Coin => CoinManager.Instance;
    protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Player player { get => m_Player; }
    public Vector2 size => m_Collider.size;
    #endregion
    #region 내부 함수
    protected IEnumerator WaitAndExitPlayMode(float time)
    {
        yield return new WaitForSeconds(time);

        M_Game.ExitPlayMode();
    }

    #region TriggerEnter
    protected void TriggerEnterEnemy(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        if (!m_Player.isSafe)
        {
            m_Player.Death();
        }
    }
    protected void TriggerEnterCoin(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        collider.GetComponent<CoinCollider>().coin.gameObject.SetActive(false);
    }
    protected void TriggerEnterSafetyZone(Collider2D collider)
    {
        m_Player.isSafe = true;
        m_Player.spawnPos = collider.transform.position;

        if (M_Edit.isPlayMode)
        {
            bool isFinishZone = collider.GetComponent<SafetyZoneCollider>().isFinishZone;

            if (isFinishZone && !M_Coin.IsLeftCoin)
            {
                // 승리
                M_Stage.canSave = true;
                M_FloatingText.SpawnFloatingText("클리어!", new Vector2(0.5f, 0.3f));
                StartCoroutine(WaitAndExitPlayMode(0.5f));
            }
        }
    }
    protected void TriggerEnterGravityZone(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        m_Player.rigidBody2D.useGravity = true;
        m_Player.rigidBody2D.gravity = collider.GetComponent<GravityZoneCollider>().gravityZone.gravity;
    }
    protected void TriggerEnterIceZone(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        m_Player.rigidBody2D.isKinematic = false;
        m_Player.rigidBody2D.drag = collider.GetComponent<IceZoneCollider>().iceZone.drag;
        //if (m_Player.rigidBody2D.velocity.magnitude == 0f)
        //{
        //    m_Player.rigidBody2D.velocity += m_Player.rigidBody2D.force;
        //}
    }
    #endregion
    #region TriggerExit
    protected void TriggerExitSafetyZone(Collider2D collider)
    {
        int layerMask = LayerMask.GetMask("SafetyZone");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
        if (colliders.Length <= 0)
        {
            m_Player.isSafe = false;
        }
    }
    protected void TriggerExitGravityZone(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        int layerMask = LayerMask.GetMask("GravityZone");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
        if (colliders.Length <= 0)
        {
            m_Player.rigidBody2D.useGravity = false;
            m_Player.rigidBody2D.gravity = MyPhysics.Physics2D.gravity;
        }
        else
        {
            m_Player.rigidBody2D.gravity = colliders[0].GetComponent<GravityZoneCollider>().gravityZone.gravity;
        }
    }
    protected void TriggerExitIceZone(Collider2D collider)
    {
        if (M_Edit.isEditMode)
            return;

        int layerMask = LayerMask.GetMask("IceZone");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
        if (colliders.Length <= 0)
        {
            m_Player.rigidBody2D.isKinematic = true;
            m_Player.rigidBody2D.drag = 1f;
        }
        else
        {
            m_Player.rigidBody2D.drag = collider.GetComponent<IceZoneCollider>().iceZone.drag;
        }
    }
    #endregion
    #endregion
    #region 외부 함수
    public void __Initialize(Player player)
    {
        // 이벤트 링크
        M_Game.OnEnterPlayMode += OnPlayEnter;
        M_Game.OnExitPlayMode += OnPlayExit;

        m_Player = player;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<BoxCollider2D>();
        }
    }

    public void EraseObject()
    {
        m_Player.gameObject.SetActive(false);
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return m_Player.renderer;
    }
    public GameObject GetGameObject()
    {
        return m_Player.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.Player;
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        int layerMask = LayerMask.GetMask("Enemy", "Coin", "GravityZone");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(m_Player.transform.position, size, 0f, layerMask);
        foreach (var item in colliders)
        {
            if (item.CompareTag("Enemy"))
            {
                TriggerEnterEnemy(item);
            }
            if (item.CompareTag("Coin"))
            {
                TriggerEnterCoin(item);
            }
            if (item.CompareTag("GravityZone"))
            {
                TriggerEnterGravityZone(item);
            }
        }
    }
    public void OnPlayExit()
    {

    }
    #endregion
    #region 유니티 콜백 함수
    private void Awake()
    {
        m_TriggerEnter = new Dictionary<string, OnTrigger>();
        m_TriggerExit = new Dictionary<string, OnTrigger>();

        m_TriggerEnter.Add("Enemy", TriggerEnterEnemy);
        m_TriggerEnter.Add("Coin", TriggerEnterCoin);
        m_TriggerEnter.Add("SafetyZone", TriggerEnterSafetyZone);
        m_TriggerEnter.Add("GravityZone", TriggerEnterGravityZone);
        m_TriggerEnter.Add("IceZone", TriggerEnterIceZone);

        m_TriggerExit.Add("SafetyZone", TriggerExitSafetyZone);
        m_TriggerExit.Add("GravityZone", TriggerExitGravityZone);
        m_TriggerExit.Add("IceZone", TriggerExitIceZone);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!m_TriggerEnter.ContainsKey(collider.tag))
            return;

        m_TriggerEnter[collider.tag]?.Invoke(collider);
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!m_TriggerExit.ContainsKey(collider.tag))
            return;

        m_TriggerExit[collider.tag]?.Invoke(collider);
    }
    #endregion
}
