using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ObjectManager<EnemyManager>
{
    protected override void Awake()
    {
        base.Awake();

        ResourcesType = E_ResourcesType.Enemy;

        PoolSize = 100;
    }

    public override void OnPlayEnter()
    {

    }
    public override void OnPlayExit()
    {

    }

    public override void __Initialize()
    {
        base.__Initialize();
    }

    public override void __Finalize()
    {
        base.__Finalize();
    }
}
