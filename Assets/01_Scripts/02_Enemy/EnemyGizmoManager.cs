using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGizmoManager : ObjectManager<EnemyGizmoManager, EnemyGizmo>
{
    protected List<EnemyGizmo> m_GizmoList;
    
    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        M_Game.OnEnterPlayMode += OnEnterPlayMode;
        M_Game.OnExitPlayMode += OnExitPlayMode;

        // 적 풀 원본
        EnemyGizmo gizmo = M_Resources.GetGameObject<EnemyGizmo>("Enemy", "EnemyGizmo");
        // 적 풀 생성
        AddPool("EnemyGizmo", gizmo, transform);

        if (null == m_GizmoList)
        {
            m_GizmoList = new List<EnemyGizmo>();
        }
    }
    public override void __Finalize()
    {
        base.__Finalize();

    }

    public EnemyGizmo SpawnGizmo()
    {
        EnemyGizmo gizmo = GetPool("EnemyGizmo").Spawn();
        gizmo.__Initialize();
        gizmo.gameObject.SetActive(true);
        m_GizmoList.Add(gizmo);

        return gizmo;
    }
    public void DespawnGizmo(EnemyGizmo gizmo)
    {
        m_GizmoList.Remove(gizmo);
        GetPool("EnemyGizmo").DeSpawn(gizmo);
    }

    public void SetActiveAllGizmo(bool value)
    {
        foreach (var item in m_GizmoList)
        {
            item.gameObject.SetActive(value);
        }
    }
    #endregion
    #region 이벤트 함수
    public void OnEnterPlayMode()
    {
        SetActiveAllGizmo(false);
    }
    public void OnExitPlayMode()
    {
        
    }
    #endregion
}
