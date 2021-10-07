﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using BackEnd;
using System;

public class UiBuffPopupView : MonoBehaviour
{
    [SerializeField]
    private Image buffIcon;

    [SerializeField]
    private TextMeshProUGUI buffDescription;

    [SerializeField]
    private GameObject useButtonObject;

    [SerializeField]
    private TextMeshProUGUI remainSecText;

    private BuffTableData buffTableData;

    [SerializeField]
    private Image buffGetButton;

    [SerializeField]
    private Sprite getEnable;

    [SerializeField]
    private Sprite getDisable;

    [SerializeField]
    private TextMeshProUGUI yomulDesc;

    [SerializeField]
    private TextMeshProUGUI remainUseDesc;

    private bool initialized = false;

    public void Initalize(BuffTableData buffTableData)
    {
        this.buffTableData = buffTableData;

        TimeSpan ts = TimeSpan.FromSeconds(buffTableData.Buffseconds);


        StatusType type = (StatusType)buffTableData.Bufftype;

        if (type.IsPercentStat())
        {
            buffDescription.SetText($"{CommonString.GetStatusName(type)}+{buffTableData.Buffvalue * 100f}%({ts.TotalMinutes}분)");
        }
        else
        {
            buffDescription.SetText($"{CommonString.GetStatusName(type)}+{Utils.ConvertBigNum(buffTableData.Buffvalue)}({ts.TotalMinutes}분)");
        }

        buffIcon.sprite = CommonUiContainer.Instance.buffIconList[buffTableData.Id];

        Subscribe();

        initialized = true;

        if (yomulDesc != null)
        {
            yomulDesc.gameObject.SetActive(buffTableData.Isyomulabil);
        }

        if (buffTableData.Isyomulabil == true)
        {
            var yomulTableData = TableManager.Instance.YomulAbilTable.dataArray[buffTableData.Yomulid];
            yomulDesc.SetText($"{yomulTableData.Abilname} LV:{buffTableData.Unlockyomullevel} 필요");
        }


    }

    private void OnEnable()
    {
        if (initialized == false) return;

        WhenRemainSecChanged(ServerData.buffServerTable.TableDatas[buffTableData.Stringid].remainSec.Value);
    }

    private void WhenRemainSecChanged(float remainSec)
    {
        if (this.gameObject.activeInHierarchy == false) return;

        if (remainSec <= 0f)
        {
            remainSecText.SetText("0초");
        }
        else
        {
            TimeSpan ts = TimeSpan.FromSeconds(remainSec);

            if (ts.Hours != 0)
            {
                remainSecText.SetText($"{ts.Hours}시간 {ts.Minutes}분 {ts.Seconds}초");
            }
            else
            {
                remainSecText.SetText($"{ts.Minutes}분 {ts.Seconds}초");
            }
        }
    }

    private void Subscribe()
    {
        ServerData.buffServerTable.TableDatas[buffTableData.Stringid].remainSec.AsObservable().Subscribe(e =>
        {
            WhenRemainSecChanged(e);
        }).AddTo(this);

        ServerData.userInfoTable.GetTableData(buffTableData.Stringid).AsObservable().Subscribe(e =>
        {
            buffGetButton.sprite = e < buffTableData.Usecount ? getEnable : getDisable;

            if (remainUseDesc != null)
            {
                remainUseDesc.SetText($"{buffTableData.Usecount - e}/{buffTableData.Usecount}회");
            }
        }).AddTo(this);
    }

    public void OnClickGetBuffButton()
    {
        if (ServerData.userInfoTable.GetTableData(buffTableData.Stringid).Value >= buffTableData.Usecount)
        {
            PopupManager.Instance.ShowAlarmMessage("오늘은 더이상 획득할 수 없습니다.");
            return;
        }

        if (buffTableData.Isyomulabil == false)
        {
            AdManager.Instance.ShowRewardedReward(() =>
            {
                BuffGetRoutine();
            });
        }
        else
        {
            var yomulTableData = TableManager.Instance.YomulAbilTable.dataArray[buffTableData.Yomulid];
            var yomulServerData = ServerData.yomulServerTable.TableDatas[yomulTableData.Stringid];

            if (yomulServerData.hasAbil.Value == 0 || yomulServerData.level.Value < buffTableData.Unlockyomullevel)
            {
                PopupManager.Instance.ShowAlarmMessage("요물 능력치 레벨이 부족합니다.");
                return;
            }
            else
            {
                BuffGetRoutine();
            }

        }
    }

    private void BuffGetRoutine()
    {
        if (ServerData.userInfoTable.GetTableData(buffTableData.Stringid).Value >= buffTableData.Usecount)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "중복요청", null);
            return;
        }

        ServerData.userInfoTable.GetTableData(buffTableData.Stringid).Value++;
        ServerData.buffServerTable.TableDatas[buffTableData.Stringid].remainSec.Value += buffTableData.Buffseconds;

        List<TransactionValue> transactions = new List<TransactionValue>();

        Param userInfoParam = new Param();

        userInfoParam.Add(buffTableData.Stringid, ServerData.userInfoTable.GetTableData(buffTableData.Stringid).Value);

        transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

        Param buffParam = new Param();

        buffParam.Add(buffTableData.Stringid, ServerData.buffServerTable.TableDatas[buffTableData.Stringid].ConvertToString());

        transactions.Add(TransactionValue.SetUpdate(BuffServerTable.tableName, BuffServerTable.Indate, buffParam));

        ServerData.SendTransaction(transactions, successCallBack: () =>
          {
              LogManager.Instance.SendLog("버프 획득", $"{buffTableData.Stringid}");
          });
    }
}
