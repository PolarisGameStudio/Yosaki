﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UniRx;

public enum TitleMissionId
{
    Level1000,//★
    Level2000,//★
    Level3000,//★
    Level5000,//★
    Level7000,//★
    Level9000,//★
    Level11000,//★
    Level13000,//★
    Level15000,//★
    Level17000,//★
    Level20000,//★
    Stage100,//★
    Stage150,//★
    Stage200,//★
    Stage250,//★
    Stage300,//★
    Stage350,//★
    Stage400,//★
    GetLegendWeapon,//★
    GetYomulWeapon,//★
    GetLegendNorigae,//★
    GetSinMulNorigae,//★
    AwakeMarble,//★
    EvolutionPet,//★
    Yomul0,//★
    Yomul1,//★
    Yomul2,//★
    Yomul3,//★
    Stage450,//★
    Stage500//★

}
public class UiTitleManager : SingletonMono<UiTitleManager>
{
    private void Start()
    {
        Subscribe();
    }
    private void Subscribe()
    {
        ServerData.statusTable.GetTableData(StatusTable.Level).AsObservable().Subscribe(e =>
        {
            if (e >= 1000)
            {
                ClearTitleMission(TitleMissionId.Level1000);
            }

            if (e >= 2000)
            {
                ClearTitleMission(TitleMissionId.Level2000);
            }

            if (e >= 3000)
            {
                ClearTitleMission(TitleMissionId.Level3000);
            }

            if (e >= 5000)
            {
                ClearTitleMission(TitleMissionId.Level5000);
            }

            if (e >= 7000)
            {
                ClearTitleMission(TitleMissionId.Level7000);
            }

            if (e >= 9000)
            {
                ClearTitleMission(TitleMissionId.Level9000);
            }

            if (e >= 11000)
            {
                ClearTitleMission(TitleMissionId.Level11000);
            }

            if (e >= 13000)
            {
                ClearTitleMission(TitleMissionId.Level13000);
            }

            if (e >= 15000)
            {
                ClearTitleMission(TitleMissionId.Level15000);
            }

            if (e >= 17000)
            {
                ClearTitleMission(TitleMissionId.Level17000);
            }

            if (e >= 20000)
            {
                ClearTitleMission(TitleMissionId.Level20000);
            }


        }).AddTo(this);
        ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).AsObservable().Subscribe(e =>
        {
            if (e >= 100 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage100);
            }
            if (e >= 150 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage150);
            }
            if (e >= 200 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage200);
            }
            if (e >= 250 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage250);
            }
            if (e >= 300 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage300);
            }
            if (e >= 350 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage350);
            }
            if (e >= 400 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage400);
            }
            if (e >= 450 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage450);
            }
            if (e >= 500 - 1)
            {
                ClearTitleMission(TitleMissionId.Stage500);
            }

        }).AddTo(this);

        ServerData.userInfoTable.GetTableData(UserInfoTable.marbleAwake).AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.AwakeMarble);
            }
        }).AddTo(this);

        //무기
        ServerData.weaponTable.TableDatas["weapon16"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendWeapon);
            }
        }).AddTo(this);

        ServerData.weaponTable.TableDatas["weapon17"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendWeapon);
            }
        }).AddTo(this);

        ServerData.weaponTable.TableDatas["weapon18"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendWeapon);
            }
        }).AddTo(this);

        ServerData.weaponTable.TableDatas["weapon19"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendWeapon);
            }
        }).AddTo(this);

        ServerData.weaponTable.TableDatas["weapon20"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetYomulWeapon);
            }
        }).AddTo(this);
        //노리개

        ServerData.magicBookTable.TableDatas["magicBook12"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook13"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook14"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook15"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetLegendNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook16"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetSinMulNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook17"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetSinMulNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook18"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetSinMulNorigae);
            }
        }).AddTo(this);

        ServerData.magicBookTable.TableDatas["magicBook19"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.GetSinMulNorigae);
            }
        }).AddTo(this);

        //환수
        ServerData.petTable.TableDatas["pet4"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.EvolutionPet);
            }
        }).AddTo(this);

        ServerData.petTable.TableDatas["pet5"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.EvolutionPet);
            }
        }).AddTo(this);

        ServerData.petTable.TableDatas["pet6"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.EvolutionPet);
            }
        }).AddTo(this);

        ServerData.petTable.TableDatas["pet7"].hasItem.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.EvolutionPet);
            }
        }).AddTo(this);

        //영베
        ServerData.yomulServerTable.TableDatas["yomul0"].level.AsObservable().Subscribe(e =>
        {
            if (e >= 100)
            {
                ClearTitleMission(TitleMissionId.Yomul0);
            }
        }).AddTo(this);

        //제물
        ServerData.yomulServerTable.TableDatas["yomul1"].hasAbil.AsObservable().Subscribe(e =>
        {
            if (e == 1)
            {
                ClearTitleMission(TitleMissionId.Yomul1);
            }
        }).AddTo(this);

        //한계돌파
        ServerData.yomulServerTable.TableDatas["yomul2"].level.AsObservable().Subscribe(e =>
        {
            if (e >= 1000)
            {
                ClearTitleMission(TitleMissionId.Yomul2);
            }
        }).AddTo(this);

        //심장베기
        ServerData.yomulServerTable.TableDatas["yomul3"].level.AsObservable().Subscribe(e =>
        {
            if (e >= 1000)
            {
                ClearTitleMission(TitleMissionId.Yomul3);
            }
        }).AddTo(this);
    }

    public void ClearTitleMission(TitleMissionId id)
    {
        var tableData = TableManager.Instance.TitleTable.dataArray[(int)id];
        var serverData = ServerData.titleServerTable.TableDatas[tableData.Stringid];

        //이미 클리어
        if (serverData.clearFlag.Value == 1)
        {
            return;
        }

        serverData.clearFlag.Value = 1;

        List<TransactionValue> transactions = new List<TransactionValue>();

        Param param = new Param();

        param.Add(tableData.Stringid, serverData.ConvertToString());

        transactions.Add(TransactionValue.SetUpdate(TitleServerTable.tableName, TitleServerTable.Indate, param));

        ServerData.SendTransaction(transactions);
    }
}
