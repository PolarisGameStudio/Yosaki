﻿using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : SingletonMono<SaveManager>
{
    private WaitForSeconds updateDelay = new WaitForSeconds(180.0f);

    private WaitForSeconds versionCheckDelay = new WaitForSeconds(300.0f);

    //12시간
    private WaitForSeconds tockenRefreshDelay = new WaitForSeconds(43200f);

    public void StartAutoSave()
    {
        StartCoroutine(AutoSaveRoutine());
        StartCoroutine(TockenRefreshRoutine());
        StartCoroutine(VersionCheckRoutine());
    }

    private IEnumerator VersionCheckRoutine()
    {
        while (true)
        {
            yield return versionCheckDelay;

            CheckClientVersion();
        }
    }

    private void CheckClientVersion()
    {
        SendQueue.Enqueue(Backend.Utils.GetLatestVersion, bro =>
        {
            if (bro.IsSuccess())
            {
                int clientVersion = int.Parse(Application.version);

                var jsonData = bro.GetReturnValuetoJSON();
                string serverVersion = jsonData["version"].ToString();

                //버전이 높거나 같음
                if (clientVersion >= int.Parse(serverVersion))
                {

                }
                else
                {
                    SyncDatasForce();

                    PopupManager.Instance.ShowVersionUpPopup(CommonString.Notice, "업데이트 버전이 있습니다. 스토어로 이동합니다.", () =>
                    {
                        Application.OpenURL("https://play.google.com/store/apps/details?id=com.DefaultCompany.SinGame");
                    }, false);
                }
            }
        });
    }

    private IEnumerator TockenRefreshRoutine()
    {
        while (true)
        {
            yield return tockenRefreshDelay;
            BackEnd.Backend.BMember.LoginWithTheBackendToken(e =>
            {
                if (e.IsSuccess())
                {
                    Debug.Log("토큰 갱신 성공");
                }
                else
                {
                    Debug.Log("토큰 갱신 실패");
                }
            });
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            SyncDatasInQueue();
            yield return updateDelay;
        }
    }

    //SendQueue에서 저장
    public void SyncDatasInQueue()
    {
       
        DatabaseManager.goodsTable.SyncAllData();

        DatabaseManager.userInfoTable.AutoUpdateRoutine();

        DatabaseManager.growthTable.UpData(GrowthTable.Exp, false);

        if (GameManager.Instance != null && GameManager.Instance.contentsType == GameManager.ContentsType.NormalField)
        {
            DatabaseManager.fieldBossTable.SyncCurrentStageKillCount();
        }

        CollectionManager.Instance.SyncToServer();

        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.UpdateBuffTime();
            DatabaseManager.buffServerTable.SyncAllData();
        }


    }
    private void OnApplicationQuit()
    {
        SyncDatasForce();
    }

    //동기로 저장
    public void SyncDatasForce()
    {
    
        DatabaseManager.goodsTable.SyncAllDataForce();

        CollectionManager.Instance.SyncToServerForce();

        DatabaseManager.growthTable.SyncDataForce();

        if (GameManager.Instance != null && GameManager.Instance.contentsType == GameManager.ContentsType.NormalField)
        {
            DatabaseManager.fieldBossTable.SyncCurrentStageKillCountForce();
        }

        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.UpdateBuffTime();
            DatabaseManager.buffServerTable.SyncAllDataForce();
        }
    }
}
