using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UndoRedoArgs
{
    private Action m_Undo;
    private Action m_Redo;

    public Action undo { get => m_Undo; set => m_Undo = value; }
    public Action redo { get => m_Redo; set => m_Redo = value; }

    #region Variables
    private GameObject m_ObjectValue;
    private int m_IntegerValue;
    private float m_FloatValue;

    public GameObject objectValue { get => m_ObjectValue; set => m_ObjectValue = value; }
    public int integerValue { get => m_IntegerValue; set => m_IntegerValue = value; }
    public float floatValue { get => m_FloatValue; set => m_FloatValue = value; }
    #endregion
}

public class UndoRedoManager : Singleton<UndoRedoManager>
{
    [SerializeField]
    protected int m_MaxCount = 100;

    protected Stack<UndoRedoArgs> m_UndoList;
    protected Stack<UndoRedoArgs> m_RedoList;

    public void AddUndo(UndoRedoArgs args)
    {
        if (m_UndoList.Count > m_MaxCount)
            m_UndoList.Pop();
        if (m_RedoList.Count > 0)
            m_RedoList.Clear();

        m_UndoList.Push(args);
    }
    #region 내부 함수
    private void Undo()
    {
        if (m_UndoList.Count <= 0)
            return;

        UndoRedoArgs args = m_UndoList.Pop();

        args.undo?.Invoke();

        m_RedoList.Push(args);
    }
    private void Redo()
    {
        if (m_RedoList.Count <= 0)
            return;

        UndoRedoArgs args = m_RedoList.Pop();

        args.redo?.Invoke();

        m_UndoList.Push(args);
    }
    #endregion
    #region 유니티 콜백 함수
    protected virtual void Awake()
    {
        m_UndoList = new Stack<UndoRedoArgs>();
        m_RedoList = new Stack<UndoRedoArgs>();
    }
    protected virtual void FixedUpdate()
    {
#if !UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl))
#endif
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Undo();
            }
            if (Input.GetKey(KeyCode.Y))
            {
                Redo();
            }
        }
    }
#endregion

    public enum E_CommandType
    {
        None = -1,

        Create,
        Delete,
        Edit,
    }
}