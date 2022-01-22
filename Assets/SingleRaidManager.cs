﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;
using static UiRewardView;
using BackEnd;

public class SingleRaidManager : ContentsManagerBase
{
    [Header("BossInfo")]
    private BossEnemyBase singleRaidEnemy;
    private AgentHpController bossHpController;

    private BossTableData bossTableData;
    private ReactiveProperty<ObscuredDouble> damageAmount = new ReactiveProperty<ObscuredDouble>();
    private ReactiveProperty<ObscuredDouble> bossRemainHp = new ReactiveProperty<ObscuredDouble>();

    public override Transform GetMainEnemyObjectTransform()
    {
        return singleRaidEnemy.transform;
    }
    public override double GetBossRemainHpRatio()
    {
        return damageAmount.Value / bossRemainHp.Value;
    }
    public double BossRemainHp => bossRemainHp.Value;

    public override double GetDamagedAmount()
    {
        return damageAmount.Value;
    }

    [Header("Ui")]
    [SerializeField]
    private TextMeshProUGUI damageIndicator;
    [SerializeField]
    private Animator damagedAnim;
    private string DamageAnimName = "Play";

    [Header("State")]
    private ReactiveProperty<ObscuredInt> contentsState = new ReactiveProperty<ObscuredInt>((int)ContentsState.Fight);

    [SerializeField]
    private UiBossResultPopup uiBossResultPopup;

    [SerializeField]
    private GameObject statusUi;

    [SerializeField]
    private GameObject directionUi;

    [SerializeField]
    private GameObject portalObject;

    [SerializeField]
    private Transform bossSpawnParent;

    #region Security
    private void OnEnable()
    {
        StartCoroutine(RandomizeRoutine());
    }

    private IEnumerator RandomizeRoutine()
    {
        var delay = new WaitForSeconds(1.0f);

        while (true)
        {
            RandomizeKey();
            yield return delay;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        RandomizeKey();
    }

    private void RandomizeKey()
    {
        damageAmount.Value.RandomizeCryptoKey();
        bossRemainHp.Value.RandomizeCryptoKey();
        contentsState.Value.RandomizeCryptoKey();
    }
    #endregion

    protected new void Start()
    {
        base.Start();
        Initialize();
        Subscribe();


    }
    private void Initialize()
    {
        SoundManager.Instance.PlaySound("BossAppear");

        SetBossHp();
    }

    private void Subscribe()
    {
        bossHpController.whenEnemyDamaged.AsObservable().Subscribe(WhenBossDamaged).AddTo(this);
        PlayerStatusController.Instance.whenPlayerDead.Subscribe(e => { WhenPlayerDead(); }).AddTo(this);

        damageAmount.AsObservable().Subscribe(whenDamageAmountChanged).AddTo(this);
        bossRemainHp.AsObservable().Subscribe(WhenBossDamaged).AddTo(this);

        contentsState.AsObservable().Subscribe(WhenBossModeStateChanged).AddTo(this);
    }

    private void WhenBossModeStateChanged(ObscuredInt state)
    {
        if (state != (int)ContentsState.Fight)
        {
            EndBossMode();
        }
    }

    private void SetBossHp()
    {
        bossTableData = TableManager.Instance.BossTableData[GameManager.Instance.bossId];
        bossRemainHp.Value = float.MaxValue;

        var prefab = Resources.Load<BossEnemyBase>($"Boss/{GameManager.Instance.bossId}");

        singleRaidEnemy = Instantiate<BossEnemyBase>(prefab, bossSpawnParent);
        singleRaidEnemy.transform.localPosition = Vector3.zero;
        singleRaidEnemy.gameObject.SetActive(false);
        bossHpController = singleRaidEnemy.GetComponent<AgentHpController>();
        bossHpController.SetRaidEnemy();
    }

    private void whenDamageAmountChanged(ObscuredDouble hp)
    {
        damageIndicator.SetText(Utils.ConvertBigNum(hp));
        damagedAnim.SetTrigger(DamageAnimName);
    }

    private void WhenBossDamaged(ObscuredDouble hp)
    {
        //  bossHpBar.UpdateHpBar(hp, bossTableData.Hp);

        if (hp <= 0f && contentsState.Value == (int)ContentsState.Fight)
        {
       //     WhenBossDead();
        }
    }

    private void WhenBossDamaged(double damage)
    {
        damageAmount.Value -= damage;
        bossRemainHp.Value += damage;
    }
    #region EndConditions
    //클리어조건1 플레이어 사망
    private void WhenPlayerDead()
    {
        if (contentsState.Value != (int)ContentsState.Fight) return;

        contentsState.Value = (int)ContentsState.Dead;
    }

    //클리어조건1 보스 처치 성공
    private void WhenBossDead()
    {
        //클리어 체크
        contentsState.Value = (int)ContentsState.Clear;

        //SendClearInfo();
    }

    //private void SendClearInfo()
    //{
    //    var serverData = ServerData.bossServerTable.TableDatas[bossTableData.Stringid];

    //    if (serverData.clear.Value != 1)
    //    {
    //        serverData.clear.Value = 1;

    //        ServerData.bossServerTable.UpdateData(bossTableData.Stringid);
    //    }
    //}

    //클리어조건1 타이머 종료
    protected override void TimerEnd()
    {
        base.TimerEnd();
        contentsState.Value = (int)ContentsState.TimerEnd;
    }
    #endregion

    private void EndBossMode()
    {
        //공격루틴 제거 + 클리어면 이펙트 켜주던지.?
        singleRaidEnemy.gameObject.SetActive(false);

        //타이머 종료
        if (contentsState.Value != (int)ContentsState.TimerEnd)
        {
            StopTimer();
        }

        //점수 전송
        SendScore();

        //보상팝업
        RewardItem();

        UiTutorialManager.Instance.SetClear(TutorialStep.PlayCatContents);
    }

    private void SendScore()
    {
        RankManager.Instance.UpdateBoss_Score(damageAmount.Value);

        //var serverData = ServerData.bossServerTable.TableDatas[bossTableData.Stringid];

        //serverData.score.Value = damageAmount.Value.ToString();

        //ServerData.bossServerTable.UpdateData(bossTableData.Stringid);
    }

    public static List<RewardData> GetRewawrdData(BossTableData bossTableData, double damagedHp, int clearCount = 1)
    {
        if (damagedHp > float.MaxValue) 
        {
            damagedHp = float.MaxValue;
        }

        List<RewardData> rewardDatas = new List<RewardData>();
        double rewardPer = Mathf.Clamp((float)damagedHp, (float)bossTableData.Rewardminhp, (float)bossTableData.Rewardmaxhp) / bossTableData.Rewardmaxhp;
        //보상 산정
        for (int i = 0; i < bossTableData.Rewardtypes.Length; i++)
        {
            Item_Type rewardType = (Item_Type)bossTableData.Rewardtypes[i];
            float rewardAmount = (Mathf.Ceil((float)rewardPer * bossTableData.Rewardmaxvalues[i])) * clearCount;

            RewardData viewData = new RewardData(rewardType, rewardAmount);
            rewardDatas.Add(viewData);
        }

        //데이터 적용(로컬)
        for (int i = 0; i < rewardDatas.Count; i++)
        {
            ServerData.AddLocalValue(rewardDatas[i].itemType, rewardDatas[i].amount);
        }

        return rewardDatas;
    }

    private void RewardItem()
    {
        DailyMissionManager.UpdateDailyMission(DailyMissionKey.RewardedBossContents, 1);

        double damagedHp = damageAmount.Value;

        List<RewardData> rewardDatas = GetRewawrdData(bossTableData, damagedHp);

        //데이터 적용(서버)
        ServerData.SendTransaction(rewardDatas);

        //결과 UI
        uiBossResultPopup.gameObject.SetActive(true);
        statusUi.SetActive(false);
        uiBossResultPopup.Initialize(damageAmount.Value, damageAmount.Value / bossTableData.Hp, rewardDatas);
    }

    protected override IEnumerator ModeTimer()
    {
        while (direciontEnd == false)
        {
            yield return null;
        }
        directionUi.SetActive(false);
        singleRaidEnemy.gameObject.SetActive(true);


        portalObject.gameObject.SetActive(false);

        float remainSec = playTime;

        while (remainSec >= 0)
        {
            timerText.SetText($"남은시간 : {(int)remainSec}");
            yield return null;
            remainSec -= Time.deltaTime;
            this.remainSec = remainSec;
        }

        TimerEnd();
    }

    private ObscuredBool direciontEnd = false;

    public void WhenDirectionAnimEnd()
    {
        direciontEnd = true;
    }
}
