using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler))]
public class FloatingTextManager : ObjectManager<FloatingTextManager, FloatingText>
{
	protected List<FloatingText> m_FloatingTextList;

	#region 내부 함수
	IEnumerator Spawn(string text, float time)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(time);
		}

		SpawnFloatingText(text, Vector2.one * 0.5f);
	}
	IEnumerator Spawn(string text, float time, Vector2 position)
	{
		if (time > 0f)
		{
			yield return new WaitForSeconds(time);
		}

		SpawnFloatingText(text, position);
	}
	#endregion
	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		M_Game.onPlayModeEnter += OnEnterPlayMode;
		M_Game.onPlayModeExit += OnExitPlayMode;

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
		SpawnFloatingText(text, Vector2.one * 0.5f);
	}
	public void SpawnFloatingText(string text, Vector2 position)
	{
		// 스폰
		FloatingText floatingText = GetPool("FloatingText").Spawn();
		// 초기화
		floatingText.__Initialize();
		// 텍스트 설정
		floatingText.text = text;
		// 위치 설정
		floatingText.transform.position = Camera.main.ViewportToScreenPoint(position);
		// 활성화 설정
		floatingText.gameObject.SetActive(true);
		// 관리 리스트에 추가
		m_FloatingTextList.Add(floatingText);
	}
	public void SpawnFloatingText(string text, float time)
	{
		StartCoroutine(Spawn(text, time));
	}
	public void SpawnFloatingText(string text, float time, Vector2 position)
	{
		StartCoroutine(Spawn(text, time, position));
	}
	public void DespawnFloatingText(FloatingText floatingText)
	{
		// 관리 리스트에서 제거
		m_FloatingTextList.Remove(floatingText);
		// 디스폰
		GetPool("FloatingText").DeSpawn(floatingText);
	}
	#endregion
	#region 이벤트 함수
	private void OnEnterPlayMode()
	{
		int count = m_FloatingTextList.Count;
		for (int i = 0; i < count; i++)
		{
			DespawnFloatingText(m_FloatingTextList[0]);
		}
	}
	private void OnExitPlayMode()
	{

	}
	#endregion
}
