using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ObjectManager<EnemyManager, Enemy>
{
    protected override void Awake()
    {
        base.Awake();

        //AddPool("Enemy", )
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

        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
    }

    public override void __Finalize()
    {
        base.__Finalize();
    }
}
