﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UiAutoBoss : SingletonMono<UiAutoBoss>
{
    [SerializeField]
    private Toggle toggle;

    public static ReactiveProperty<bool> AutoMode = new ReactiveProperty<bool>(false);

    private new void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        WhenToggleChanged(AutoMode.Value);
    }

    public void WhenToggleChanged(bool on)
    {
        if (on)
        {
            if (AutoMode.Value == false)
            {
                PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, "자동으로 진행 합니까?", () =>
                {
                    StartBossContents();
                }, () =>
                {
                    StopAutoBoss();
                });
            }
            else
            {
                StartBossContents();
            }

        }
        else
        {
            StopAutoBoss();
        }

    }

    private void StartBossContents()
    {
        AutoMode.Value = true;

        if (autoRoutine != null)
        {
            StopCoroutine(autoRoutine);
        }

        autoRoutine = StartCoroutine(AutoRoutine());
    }

    public void StopAutoBoss()
    {
        toggle.isOn = false;
        AutoMode.Value = false;

        if (autoRoutine != null)
        {
            StopCoroutine(autoRoutine);
        }
    }

    private Coroutine autoRoutine;

    private IEnumerator AutoRoutine()
    {
        yield return null;
        yield return null;
        AutoManager.Instance.SetAuto(true);
        BossSpawnButton.Instance.OnClickSpawnButton();
    }
}
