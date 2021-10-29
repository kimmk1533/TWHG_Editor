using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UndoRedoArgs
{
	public Action undo { get; set; }
	public Action redo { get; set; }

	#region Variables
	public GameObject objectValue { get; set; }
	public int integerValue { get; set; }
	public float floatValue { get; set; }
	#endregion
}

public class UndoRedoManager : Singleton<UndoRedoManager>
{
	[SerializeField, ReadOnly(true)]
	protected int m_MaxCount = 100;

	protected LinkedList<UndoRedoArgs> m_UndoList;
	protected LinkedList<UndoRedoArgs> m_RedoList;

	public void AddUndoRedoArgs(UndoRedoArgs args)
	{
		if (m_UndoList.Count >= m_MaxCount)
			m_UndoList.RemoveLast();
		if (m_RedoList.Count > 0)
			m_RedoList.Clear();

		m_UndoList.AddFirst(args);
	}
	#region 내부 함수
	private void Undo()
	{
		if (m_UndoList.Count <= 0)
			return;

		UndoRedoArgs args = m_UndoList.First.Value;
		m_UndoList.RemoveFirst();

		args.undo?.Invoke();

		m_RedoList.AddFirst(args);
	}
	private void Redo()
	{
		if (m_RedoList.Count <= 0)
			return;

		UndoRedoArgs args = m_RedoList.First.Value;
		m_RedoList.RemoveFirst();

		args.redo?.Invoke();

		m_UndoList.AddFirst(args);
	}
	#endregion
	#region 유니티 콜백 함수
	protected void Awake()
	{
		m_UndoList = new LinkedList<UndoRedoArgs>();
		m_RedoList = new LinkedList<UndoRedoArgs>();
	}
	protected void Update()
	{
#if !UNITY_EDITOR
		if (Input.GetKey(KeyCode.LeftControl))
#endif
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Undo();
			}
			if (Input.GetKeyDown(KeyCode.Y))
			{
				Redo();
			}
		}
	}
	#endregion
}