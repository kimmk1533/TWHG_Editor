using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextManager : ObjectManager<FloatingTextManager, FloatingText>
{
    protected List<FloatingText> m_FloatingTextList;

    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        FloatingText floatingText = M_Resources.GetGameObject<FloatingText>("UI", "FloatingText");
        AddPool("FloatingText", floatingText, transform);

        if (null == m_FloatingTextList)
        {
            m_FloatingTextList = new List<FloatingText>();
        }
    }

    public FloatingText SpawnFloatingText(string text)
    {
        // 스폰
        FloatingText floatingText = GetPool("FloatingText").Spawn();
        // 초기화
        floatingText.__Initialize();
        // 텍스트 설정
        floatingText.text = text;
        // 활성화 설정
        floatingText.gameObject.SetActive(true);
        // 관리 리스트에 추가
        m_FloatingTextList.Add(floatingText);

        return floatingText;
    }
    public void DespawnFloatingText(FloatingText floatingText)
    {
        m_FloatingTextList.Remove(floatingText);
        GetPool("FloatingText").DeSpawn(floatingText);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {

    }
    public override void OnPlayExit()
    {
        
    }
    #endregion
}
