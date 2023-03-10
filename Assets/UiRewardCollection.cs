using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UiRewardCollection : MonoBehaviour
{
    [SerializeField]
    private Button oniButton;

    [SerializeField]
    private Button oniButton2;

    [SerializeField]
    private Button baekGuiButton;

    [SerializeField]
    private Button sonButton;

    [SerializeField]
    private Button gangChulButton;

    [SerializeField]
    private Button gumGiButton;

    [SerializeField]
    private Button hellFireButton;

    [SerializeField]
    private Button chunFlowerButton;

    [SerializeField]
    private Button hellRelicButton;

    [SerializeField]
    private Button dokebiClearButton;

    [SerializeField]
    private Button sumiClearButton;

    [SerializeField]
    private Button newGachaButton;

    [SerializeField]
    private GameObject Bandi0;
    [SerializeField]
    private GameObject Bandi1;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        ServerData.statusTable.GetTableData(StatusTable.Level).AsObservable().Subscribe(e =>
        {
            oniButton.interactable = e >= 300;
            baekGuiButton.interactable = e >= 3000;
            sonButton.interactable = e >= 5000;
            gangChulButton.interactable = e >= 30000;
            gumGiButton.interactable = e >= 50000;
            hellFireButton.interactable = e >= 50000;
            chunFlowerButton.interactable = e >= 200000;
            hellRelicButton.interactable = e >= 50000;
            dokebiClearButton.interactable = e >= 500000;
            sumiClearButton.interactable = e >= 1000000;
            oniButton2.interactable = e >= 500000;


            if (e >= GameBalance.banditUpgradeLevel)
            {
                Bandi0.SetActive(false);
            }
            else
            {
                Bandi1.SetActive(false);
            }
        }).AddTo(this);

        ServerData.userInfoTable.GetTableData(UserInfoTable.relicKillCount).AsObservable().Subscribe(e =>
        {
            newGachaButton.interactable = e >= 25000;
        }).AddTo(this);
    }

    public void OnClickBanditReward()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonEnterCount).Value >= GameBalance.bonusDungeonEnterCount)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ????????? ????????? ??? ????????????.");
            return;
        }

        int killCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonMaxKillCount).Value;

        if (killCount == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ????????????.");
            return;
        }

        int clearCount = GameBalance.bonusDungeonEnterCount - (int)ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonEnterCount).Value;

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"?????? <color=yellow>{killCount}</color>??? <color=yellow>{clearCount}???</color> ?????? ??????????\n{CommonString.GetItemName(Item_Type.Jade)} {killCount * GameBalance.bonusDungeonGemPerEnemy * (GameBalance.bandiPlusStageJadeValue * (int)Mathf.Floor(Mathf.Max(1000f, (float)ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value) + 2) / GameBalance.bandiPlusStageDevideValue) * clearCount}???\n{CommonString.GetItemName(Item_Type.Marble)} {killCount * GameBalance.bonusDungeonMarblePerEnemy * (GameBalance.bandiPlusStageMarbleValue * (int)Mathf.Floor(Mathf.Max(1000f, (float)ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value) + 2) / GameBalance.bandiPlusStageDevideValue) * clearCount}???", () =>
            {
                // enterButton.interactable = false;


                int rewardNumJade = (killCount * GameBalance.bonusDungeonGemPerEnemy) * (GameBalance.bandiPlusStageJadeValue * (int)Mathf.Floor(Mathf.Max(1000f, (float)ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value) + 2) / GameBalance.bandiPlusStageDevideValue) * clearCount;
                int rewardNumMarble = killCount * GameBalance.bonusDungeonMarblePerEnemy * (GameBalance.bandiPlusStageMarbleValue * (int)Mathf.Floor(Mathf.Max(1000f, (float)ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value) + 2) / GameBalance.bandiPlusStageDevideValue) * clearCount;
                ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonEnterCount).Value += clearCount;

                ServerData.goodsTable.GetTableData(GoodsTable.Jade).Value += rewardNumJade;
                ServerData.goodsTable.GetTableData(GoodsTable.MarbleKey).Value += rewardNumMarble;

                //????????? ??????
                List<TransactionValue> transactionList = new List<TransactionValue>();

                Param goodsParam = new Param();
                goodsParam.Add(GoodsTable.Jade, ServerData.goodsTable.GetTableData(GoodsTable.Jade).Value);
                goodsParam.Add(GoodsTable.MarbleKey, ServerData.goodsTable.GetTableData(GoodsTable.MarbleKey).Value);
                transactionList.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

                Param userInfoParam = new Param();
                userInfoParam.Add(UserInfoTable.bonusDungeonEnterCount, ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonEnterCount).Value);
                transactionList.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

                ServerData.SendTransaction(transactionList,
                    successCallBack: () =>
                    {
                        DailyMissionManager.UpdateDailyMission(DailyMissionKey.ClearBonusDungeon, 1);
                    },
                    completeCallBack: () =>
                    {
                        // enterButton.interactable = true;
                    });

                EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearBandit, clearCount);

                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{clearCount}??? ?????? ??????!\n{CommonString.GetItemName(Item_Type.Jade)} {rewardNumJade}???\n{CommonString.GetItemName(Item_Type.Marble)} {rewardNumMarble}??? ??????!", null);
                SoundManager.Instance.PlaySound("GoldUse");


            }, null);

    }//???

    public void OnClickDayofWeekReward()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.getDayOfWeek).Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"?????? ????????? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.DayOfWeekClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }


        var tabledata = TableManager.Instance.dayOfWeekDungeon.dataArray;


        float multipleValue = 0f;
        for (int i = 0; i < tabledata[GetDayOfweek()].Score.Length; i++)
        {
            //??????
            if (tabledata[GetDayOfweek()].Score[i] <= ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value)
            {
                multipleValue = tabledata[GetDayOfweek()].Rewardvalue[i];
            }
            //??????
            else
            {
                if (i == 0)
                {
                    multipleValue = 1f;
                }
                break;
            }
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score * multipleValue}??? ?????? ??????????", () =>
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.getDayOfWeek).Value = 1;


            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getDayOfWeek, ServerData.userInfoTable.TableDatas[UserInfoTable.getDayOfWeek].Value);



            ServerData.goodsTable.GetTableData(tabledata[GetDayOfweek()].Rewardstring).Value += score * multipleValue;
            Param goodsParam = new Param();
            goodsParam.Add(tabledata[GetDayOfweek()].Rewardstring, ServerData.goodsTable.GetTableData(tabledata[GetDayOfweek()].Rewardstring).Value);


            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            DailyMissionManager.UpdateDailyMission(DailyMissionKey.ClearBonusDungeon, 10);

            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearBandit, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName((Item_Type)tabledata[GetDayOfweek()].Rewardtype)} {score * multipleValue}??? ??????!", null);
            });
        }, null);
    }//???

    public void OnClickOniReward()
    {
        //????????? ??????
        int idx = 3;

        int currentEnterCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiEnterCount).Value;

        if (currentEnterCount >= GameBalance.dokebiEnterCount)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ????????? ????????? ??? ????????????.");
            return;
        }

        int defeatCount = 0;

        int clearCount = GameBalance.dokebiEnterCount - currentEnterCount;

        if (idx == 0)
        {
            defeatCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiKillCount0).Value;
        }
        else if (idx == 1)
        {
            defeatCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiKillCount1).Value;
        }
        else if (idx == 2)
        {
            defeatCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiKillCount2).Value;
        }
        else if (idx == 3)
        {
            defeatCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiKillCount3).Value;
        }

        if (defeatCount == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.Dokebi)} <color=yellow>{defeatCount}</color>?????? <color=yellow>{clearCount}???</color> ?????? ??????????", () =>
        {
            GuideMissionManager.UpdateGuideMissionClear(GuideMissionKey.ClearOni);

            int rewardNum = defeatCount;

            ServerData.goodsTable.GetTableData(GoodsTable.DokebiKey).Value += rewardNum * clearCount;

            ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiEnterCount).Value += clearCount;

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param goodsParam = new Param();

            goodsParam.Add(GoodsTable.DokebiKey, ServerData.goodsTable.GetTableData(GoodsTable.DokebiKey).Value);

            Param userInfoParam = new Param();

            userInfoParam.Add(UserInfoTable.dokebiEnterCount, ServerData.userInfoTable.GetTableData(UserInfoTable.dokebiEnterCount).Value);

            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));
            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearOni, clearCount);
            ServerData.SendTransaction(transactions, successCallBack: () =>
            {

                PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.Dokebi)} {rewardNum * clearCount}??? ??????!");

                //?????????
                SoundManager.Instance.PlaySound("Reward");
                //LogManager.Instance.SendLog("DokClear", $"{rewardNum}??? ?????? {ServerData.goodsTable.GetTableData(GoodsTable.DokebiKey).Value}");
            });
        }, null);
    }//???

    public void OnClickBaekguiReward()
    {

    }//???

    public void OnClickSonReward()
    {

    }

    public void OnClickGumgiReward()
    {
        if (ServerData.userInfoTable.TableDatas[UserInfoTable.getGumGi].Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SP)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.gumGiClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}??? ?????? ??????????\n<color=red>(?????? ????????? ?????? ??????)</color>\n{CommonString.GetItemName(Item_Type.DokebiTreasure)}??? ???????????? : {Utils.GetDokebiTreasureAddValue()}", () =>
        {
            ServerData.userInfoTable.TableDatas[UserInfoTable.getGumGi].Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.SwordPartial).Value += score + Utils.GetDokebiTreasureAddValue();

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getGumGi, ServerData.userInfoTable.TableDatas[UserInfoTable.getGumGi].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.SwordPartial, ServerData.goodsTable.GetTableData(GoodsTable.SwordPartial).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearSwordPartial, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                LogManager.Instance.SendLogType("GumGi", "_", score.ToString());
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SP)} {score + Utils.GetDokebiTreasureAddValue()}??? ??????!", null);
            });
        }, null);
    }

    public void OnClickHellFireReward()
    {

    }

    public void OnClickChunFlowerReward()
    {
        if (ServerData.userInfoTable.TableDatas[UserInfoTable.getFlower].Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.Cw)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.flowerClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}??? ?????? ??????????\n<color=red>(?????? ????????? ?????? ??????)</color>\n{CommonString.GetItemName(Item_Type.DokebiTreasure)}??? ???????????? : {Utils.GetDokebiTreasureAddValue()}", () =>
        {
            ServerData.userInfoTable.TableDatas[UserInfoTable.getFlower].Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.Cw).Value += score + Utils.GetDokebiTreasureAddValue();

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getFlower, ServerData.userInfoTable.TableDatas[UserInfoTable.getFlower].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.Cw, ServerData.goodsTable.GetTableData(GoodsTable.Cw).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));
            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearChunFlower, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.Cw)} {score + Utils.GetDokebiTreasureAddValue()}??? ??????!", null);
            });
        }, null);
    }

    public void OnClickSmithReward()
    {
        if (ServerData.userInfoTable.TableDatas[UserInfoTable.getSmith].Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SmithFire)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.smithClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}??? ?????? ??????????\n<color=red>(?????? ????????? ?????? ??????)</color>", () =>
        {
            ServerData.userInfoTable.TableDatas[UserInfoTable.getSmith].Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.SmithFire).Value += score;

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getSmith, ServerData.userInfoTable.TableDatas[UserInfoTable.getSmith].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.SmithFire, ServerData.goodsTable.GetTableData(GoodsTable.SmithFire).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                LogManager.Instance.SendLogType("Smith", "_", score.ToString());
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SmithFire)} {score}??? ??????!", null);
            });
        }, null);
    }

    public void OnClickGuildGumihoReward()
    {

    }

    public void OnClickTrainingReward()
    {
        RewardPopupManager.Instance.OnclickButton();
    }

    public void OnClickDokebiReward()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.getDokebiFire).Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.DokebiFire)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.DokebiFireClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}??? ?????? ??????????\n{CommonString.GetItemName(Item_Type.DokebiTreasure)}??? ???????????? : {Utils.GetDokebiTreasureAddValue()}", () =>
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.getDokebiFire).Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.DokebiFire).Value += score + Utils.GetDokebiTreasureAddValue();

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getDokebiFire, ServerData.userInfoTable.TableDatas[UserInfoTable.getDokebiFire].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.DokebiFire, ServerData.goodsTable.GetTableData(GoodsTable.DokebiFire).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearDokebiFire, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.DokebiFire)} {score + Utils.GetDokebiTreasureAddValue()}??? ??????!", null);
            });
        }, null);
    }

    public void OnClickSumiReward()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.getSumiFire).Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SumiFire)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.sumiFireClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}??? ?????? ??????????\n{CommonString.GetItemName(Item_Type.DokebiTreasure)}??? ???????????? : {Utils.GetDokebiTreasureAddValue()}", () =>
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.getSumiFire).Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value += score + Utils.GetDokebiTreasureAddValue();

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getSumiFire, ServerData.userInfoTable.TableDatas[UserInfoTable.getSumiFire].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.SumiFire, ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearSumiFire, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SumiFire)} {score + Utils.GetDokebiTreasureAddValue()}??? ??????!", null);
            });
        }, null);
    }

    private int GetDayOfweek()
    {
        var serverTime = ServerData.userInfoTable.currentServerTime;
        return (int)serverTime.DayOfWeek;
    }


    public void OnClickGetNewGachaButton()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.getRingGoods).Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.NewGachaEnergy)}??? ????????? ????????? ?????? ???????????????!");
            return;
        }

        //int amount = GameBalance.getRingGoodsAmount;
        int amount = GameBalance.getRingGoodsAmount * (int)Mathf.Floor(Mathf.Max(1, (float)ServerData.userInfoTable.GetTableData(UserInfoTable.relicKillCount).Value));

        if (amount == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("????????? ???????????? ???????????????.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{amount}??? ?????? ??????????", () =>
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.getRingGoods).Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.NewGachaEnergy).Value += amount;

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getRingGoods, ServerData.userInfoTable.TableDatas[UserInfoTable.getRingGoods].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.NewGachaEnergy, ServerData.goodsTable.GetTableData(GoodsTable.NewGachaEnergy).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.NewGachaEnergy)} {amount}??? ??????!", null);
            });
        }, null);
    }
}
