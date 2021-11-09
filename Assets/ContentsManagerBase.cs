﻿using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ContentsState
{
    Fight, Dead, TimerEnd, Clear
}

public class ContentsManagerBase : SingletonMono<ContentsManagerBase>
{
    [SerializeField]
    private PolygonCollider2D cameracollider;

    [SerializeField]
    protected ObscuredInt playTime = 60;

    protected Coroutine timerRoutine;

    [SerializeField]
    protected TextMeshProUGUI timerText;

    protected ObscuredFloat remainSec = 0;
    public ObscuredFloat RemainSec => remainSec;
    public ObscuredInt PlayTime => playTime;

    public virtual Transform GetMainEnemyObjectTransform()
    {
        return null;
    }

    public virtual float GetDamagedAmount()
    {
        return 0f;
    }

    public virtual float GetBossRemainHpRatio()
    {
        return 0f;
    }

    protected virtual IEnumerator ModeTimer()
    {
        remainSec = playTime;

        while (remainSec >= 0)
        {
            timerText.SetText($"남은시간 : {(int)remainSec}");
            yield return null;
            remainSec -= Time.deltaTime;
        }

        TimerEnd();
    }


    protected virtual void TimerEnd() { }

    protected void Start()
    {
        SetCameraCollider();

        timerRoutine = StartCoroutine(ModeTimer());

        UiSubMenues.Instance.gameObject.SetActive(false);
    }

    protected void StopTimer()
    {
        StopCoroutine(timerRoutine);
    }

    private void SetCameraCollider()
    {
        var cameraConfiner = GameObject.FindObjectOfType<CinemachineConfiner>();
        cameraConfiner.m_BoundingShape2D = cameracollider;
    }

    public virtual void DiscountRelicDungeonHp()
    {

    }
}
