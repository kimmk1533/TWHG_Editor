using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextManager : ObjectManager<FloatingTextManager, FloatingText>
{
    protected List<FloatingText> m_FloatingTextList;

    [SerializeField]
    protected GameObject m_FloatingText_Panel;

    #region 내부 함수
    IEnumerator Spawn(string text, float time)
    {
        yield return new WaitForSeconds(time);

        SpawnFloatingText(text);
    }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        M_Game.OnEnterPlayMode += OnEnterPlayMode;
        M_Game.OnExitPlayMode += OnExitPlayMode;

        FloatingText floatingText = M_Resources.GetGameObject<FloatingText>("UI", "FloatingText");
        AddPool("FloatingText", floatingText, transform);

        if (null == m_FloatingTextList)
        {
            m_FloatingTextList = new List<FloatingText>();
        }
    }
    public override void __Finalize()
    {
        base.__Finalize();
    }

    public void SpawnFloatingText(string text)
    {
        // 스폰
        FloatingText floatingText = GetPool("FloatingText").Spawn();
        // 초기화
        floatingText.__Initialize();
        // 텍스트 설정
        floatingText.text = text;
        // 부모 설정
        floatingText.transform.SetParent(m_FloatingText_Panel.transform);
        // 크기 설정
        floatingText.transform.localScale = Vector3.one;
        // 활성화 설정
        floatingText.gameObject.SetActive(true);
        // 관리 리스트에 추가
        m_FloatingTextList.Add(floatingText);
    }
    public void SpawnFloatingText(string text, float time)
    {
        StartCoroutine(Spawn(text, time));
    }
    public void SpawnFloatingText(string text, Vector3 position)
    {
        // 스폰
        FloatingText floatingText = GetPool("FloatingText").Spawn();
        // 초기화
        floatingText.__Initialize();
        // 텍스트 설정
        floatingText.text = text;
        // 부모 설정
        floatingText.transform.SetParent(m_FloatingText_Panel.transform);
        // 크기 설정
        floatingText.transform.localScale = Vector3.one;
        // 위치 설정
        floatingText.transform.position = position;
        // 활성화 설정
        floatingText.gameObject.SetActive(true);
        // 관리 리스트에 추가
        m_FloatingTextList.Add(floatingText);
    }
    public void DespawnFloatingText(FloatingText floatingText)
    {
        m_FloatingTextList.Remove(floatingText);
        GetPool("FloatingText").DeSpawn(floatingText);
    }
    #endregion
    #region 이벤트 함수
    public void OnEnterPlayMode()
    {
        for (int i = 0; i < m_FloatingTextList.Count; i++)
        {
            DespawnFloatingText(m_FloatingTextList[0]);
        }
    }
    public void OnExitPlayMode()
    {
        
    }
    #endregion
}
