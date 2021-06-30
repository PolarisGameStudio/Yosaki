﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using UniRx;
using CodeStage.AntiCheat.ObscuredTypes;
using BackEnd;

public class MapInfo : SingletonMono<MapInfo>
{
    private List<EnemySpawnPlatform> spawnPlatforms;
    private List<EnemyTableData> spawnEnemyData;
    private List<Enemy> spawnedEnemyList = new List<Enemy>();
    public List<Enemy> SpawnedEnemyList => spawnedEnemyList;
    private int maxEnemy = 0;

    [SerializeField]
    private PolygonCollider2D cameracollider;

    public ReactiveCommand whenSpawnedEnemyCountChanged = new ReactiveCommand();
    public ReactiveProperty<float> spawnGaugeValue = new ReactiveProperty<float>();

    public Dictionary<int, int> spawnedEnemyPlatforms = new Dictionary<int, int>();

    private new void Awake()
    {
        m_DontDestroy = false;
        Initialize();
        base.Awake();
        SetCameraCollider();
    }

    private bool IsEnemyEmpty()
    {
        var e = spawnedEnemyPlatforms.GetEnumerator();

        while (e.MoveNext())
        {
            if (e.Current.Value > 0) return false;
        }

        return true;
    }

    private void SetCameraCollider()
    {
        var cameraConfiner = GameObject.FindObjectOfType<CinemachineConfiner>();
        cameraConfiner.m_BoundingShape2D = cameracollider;
    }
    private void Initialize()
    {

        spawnPlatforms = GetComponentsInChildren<EnemySpawnPlatform>().ToList();

        SetEnemyData();


    }
    void Start()
    {
        StartCoroutine(EnemySpawnRoutine());

        Subscribe();
    }

    private void Subscribe()
    {
        //플레이어 사망시
        PlayerStatusController.Instance.whenPlayerDead.AsObservable().Subscribe(e =>
        {
            //자동사냥 종료
            AutoManager.Instance.SetAuto(false);

            //몹 스폰 중단
            StopAllCoroutines();

            //경험치 절반 감소
            GrowthManager.Instance.WhenPlayerDeadInNormalField();

            PopupManager.Instance.ShowDeadConfirmPopup(CommonString.Notice, "플레이어가 사망했습니다.\n경험치 50%를 잃습니다.", () =>
            {
                GameManager.Instance.LoadNormalField();
            });

        }).AddTo(this);

        UiStageNameIndicater.Instance.whenFieldBossTimerEnd.AsObservable().Subscribe(WhenFieldBossTimerEnd).AddTo(this);
    }

    private void WhenFieldBossTimerEnd(Unit unit)
    {
        UiStageNameIndicater.Instance.StopFieldBossTimer();
        DisableFieldBoss();
        PopupManager.Instance.ShowAlarmMessage("보스가 도망갔습니다!");
    }

    private void SetEnemyData()
    {
        spawnEnemyData = GameManager.Instance.GetEnemyTableData();

        maxEnemy = spawnPlatforms.Count * GameManager.Instance.CurrentStageData.Spawnamountperplatform;
    }

    public Vector3 GetRandomPos()
    {
        return Vector3.zero;
        //return new Vector2(Random.Range(min.position.x, max.position.x), Random.Range(min.position.y, max.position.y));
    }

    private bool IsEnemyMax()
    {
        return spawnedEnemyList.Count >= maxEnemy;
    }


    private IEnumerator EnemySpawnRoutine()
    {
        if (spawnPlatforms == null || spawnPlatforms.Count == 0) yield break;

        //초기딜레이
        yield return new WaitForSeconds(2.0f);

        if (UiAutoRevive.autoRevive)
        {
            UiAutoRevive.autoRevive = false;
            AutoManager.Instance.SetAuto(true);
        }

        WaitForSeconds spawnDelay = new WaitForSeconds(GameManager.Instance.CurrentStageData.Spawndelay);

        //스폰 간격
        WaitForSeconds spawnInterval = new WaitForSeconds(GameBalance.spawnIntervalTime);
        while (true)
        {
            if (gaugeRoutine != null)
            {
                StopCoroutine(gaugeRoutine);
            }

            gaugeRoutine = StartCoroutine(UpdateGauge(GameManager.Instance.CurrentStageData.Spawndelay + GameBalance.spawnIntervalTime * GameBalance.spawnDivideNum));

            bool isEnemyEmpty = IsEnemyEmpty();

            int spawnNum = maxEnemy - spawnedEnemyList.Count;

            for (int i = 0; i < spawnNum; i++)
            {
                SpawnEnemy(false, isEnemyEmpty);

                whenSpawnedEnemyCountChanged.Execute();

                if (i == spawnNum / GameBalance.spawnDivideNum)
                {
                    yield return spawnInterval;
                }
            }

            yield return spawnDelay;
        }
    }

    private int GetMinimalSpawnedPlatformIdx()
    {
        int platformId = 0;
        int currentEnemyCount = int.MaxValue;

        var e = spawnedEnemyPlatforms.GetEnumerator();

        while (e.MoveNext())
        {
            if (e.Current.Value < currentEnemyCount)
            {
                currentEnemyCount = e.Current.Value;
                platformId = e.Current.Key;
            }

        }

        return platformId;
    }

    private void AddEnemyCountInPlatform(int platformId)
    {
        if (spawnedEnemyPlatforms.ContainsKey(platformId) == false)
        {
            spawnedEnemyPlatforms.Add(platformId, 0);
        }

        spawnedEnemyPlatforms[platformId]++;
    }

    private void RemoveEnemyCountInPlatform(int platformId)
    {
        spawnedEnemyPlatforms[platformId]--;
    }

    //제일 하단 발판
    private int GetBossSpawnPlatformIdx()
    {
        int ret = 0;
        float yPos = float.MaxValue;

        for (int i = 0; i < spawnPlatforms.Count; i++)
        {
            if (spawnPlatforms[i].transform.position.y < yPos)
            {
                yPos = spawnPlatforms[i].transform.position.y;
                ret = i;
            }
        }

        return ret;
    }

    private void SpawnEnemy(bool isBossEnemy, bool isRandomTurn)
    {
        if (spawnEnemyData.Count == 0)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "데이터 없음", null);
            return;
        }

        var enemyObject = BattleObjectManager.Instance.GetItem($"Enemy/{spawnEnemyData[0].Prefabname}") as Enemy;
        int spawnedIdx = 0;

        if (isBossEnemy == false)
        {
            if (isRandomTurn)
            {
                spawnedIdx = Random.Range(0, spawnPlatforms.Count);
            }
            else
            {
                spawnedIdx = GetMinimalSpawnedPlatformIdx();
            }

            AddEnemyCountInPlatform(spawnedIdx);

            Vector3 spawnPos = spawnPlatforms[spawnedIdx].GetRandomSpawnPos();

            enemyObject.transform.position = spawnPos;
        }
        else
        {
            UiStageNameIndicater.Instance.StartFieldBossTimer(15);

            PopupManager.Instance.ShowAlarmMessage("필드보스 출현!");

            //첫번째 발판에 소환
            enemyObject.transform.position = spawnPlatforms[GetBossSpawnPlatformIdx()].GetRandomSpawnPos();

            EffectManager.SpawnEffect("Circle1", enemyObject.transform.position);
            SoundManager.Instance.PlaySound("4-1");
        }


        enemyObject.SetReturnCallBack(EnemyRemoveCallBack);

        enemyObject.Initialize(spawnEnemyData[0], isBossEnemy, spawnedIdx);

        spawnedEnemyList.Add(enemyObject);
    }

    private Coroutine gaugeRoutine;

    private IEnumerator UpdateGauge(float spawnDelay)
    {
        spawnGaugeValue.Value = 0f;

        while (spawnGaugeValue.Value < spawnDelay)
        {
            spawnGaugeValue.Value += Time.deltaTime;

            yield return null;
        }

        spawnGaugeValue.Value = spawnDelay;
    }

#if UNITY_EDITOR
    IEnumerator SpawnTimerRoutine_Text(float delay)
    {
        WaitForSeconds oneSecond = new WaitForSeconds(1);
        while (delay > 0)
        {
            Debug.LogError($"Enemy Spawn remain {delay}");
            yield return oneSecond;
            delay -= 1f;
        }
    }
#endif

    private void EnemyRemoveCallBack(Enemy enemy)
    {
        RemoveEnemyCountInPlatform(enemy.spawnedPlatformIdx);

        spawnedEnemyList.Remove(enemy);
        whenSpawnedEnemyCountChanged.Execute();
    }


    public void SpawnBossEnemy()
    {
        if (GameManager.Instance.contentsType != GameManager.ContentsType.NormalField)
        {
            PopupManager.Instance.ShowAlarmMessage("이곳에서는 소환할 수 없습니다.");
            return;
        }

        SpawnEnemy(true, false);
    }

    public bool HasSpawnedBossEnemy()
    {
        for (int i = 0; i < spawnedEnemyList.Count; i++)
        {
            if (spawnedEnemyList[i].isFieldBossEnemy == true)
            {
                return true;
            }
        }
        return false;
    }
    private void DisableFieldBoss()
    {
        for (int i = 0; i < spawnedEnemyList.Count; i++)
        {
            if (spawnedEnemyList[i].isFieldBossEnemy == true)
            {
                spawnedEnemyList[i].gameObject.SetActive(false);
                return;
            }
        }
    }

    public void SetFieldClear()
    {
        List<TransactionValue> transactions = new List<TransactionValue>();

        Param goodsParam = new Param();

        //보상지급
        int rewardValue = GameManager.Instance.CurrentStageData.Bossrewardvalue;
        DatabaseManager.goodsTable.GetTableData(GoodsTable.GrowthStone).Value += rewardValue;
        goodsParam.Add(GoodsTable.GrowthStone, DatabaseManager.goodsTable.GetTableData(GoodsTable.GrowthStone).Value);

        transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

        Param stageParam = new Param();
        DatabaseManager.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value = GameManager.Instance.CurrentStageData.Id;
        stageParam.Add(UserInfoTable.topClearStageId, DatabaseManager.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value);

        transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, stageParam));

        DatabaseManager.SendTransaction(transactions, successCallBack: () =>
          {
              LogManager.Instance.SendLog("스테이지클리어", GameManager.Instance.CurrentStageData.Id.ToString());
              //결과표시
              UiFieldBossRewardView.Instance.Initialize(rewardValue);
          });
    }

    //#if UNITY_EDITOR
    //    private void Update()
    //    {
    //        if (Input.GetKeyDown(KeyCode.W)) 
    //        {
    //            DatabaseManager.userInfoTable.GetTableData(UserInfoTable.wingGrade).Value++;
    //        }
    //    }
    //#endif
}
