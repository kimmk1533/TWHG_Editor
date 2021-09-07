using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    protected void Awake()
    {
        // 게임 도중 화면 안꺼지게 하기
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // 화면 해상도 (16:9) 고정
        Screen.SetResolution(Screen.width, Screen.width * 16 / 9, true);

        DontDestroyOnLoad(gameObject);
    }
}
