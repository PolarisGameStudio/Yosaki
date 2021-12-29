﻿using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGuildBossView : SingletonMono<UiGuildBossView>
{
    [SerializeField]
    private UiTwelveBossContentsView bossContentsView;

    public ObscuredInt rewardGrade = 0;

    void Start()
    {
        Initialize();
    }
    private void OnEnable()
    {
        this.transform.SetAsLastSibling();
    }

    private void Initialize()
    {
        bossContentsView.Initialize(TableManager.Instance.TwelveBossTable.dataArray[12]);
    }

    public void RecordGuildScoreButton()
    {
        bool canRecord = ServerData.userInfoTable.CanRecordGuildScore();

        if (canRecord == false)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "평일 오전3시~오전5시에는 점수를 추가할 수 없습니다!\n일요일은 오후11시~ 월요일 오전5시까지\n점수를 등록할수 없습니다!(랭킹 집계)", null);
            return;
        }

        if (ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage("오늘은 이미 점수를 추가했습니다.");
            return;
        }
        if (rewardGrade == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("추가할 점수가 없습니다.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{rewardGrade}점 점수를 추가합니까?\n<color=red>점수는 하루에 한번만 추가할 수 있습니다.</color>\n문파별로 최대 인원만큼만 추가 가능합니다.\n(매일 오전 5시 초기화)",
            () =>
            {
                var guildInfoBro = Backend.Social.Guild.GetMyGuildGoodsV3();

                if (guildInfoBro.IsSuccess())
                {
                    var returnValue = guildInfoBro.GetReturnValuetoJSON();

                    int addAmount = int.Parse(returnValue["goods"]["totalGoods10Amount"]["N"].ToString());

                    if (addAmount >= GameBalance.GuildMemberMax)
                    {
                        PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{GuildManager.Instance.myGuildName} 문파는 \n오늘 더이상 점수를 추가할 수 없습니다!\n<color=red>문파별로 최대 {GameBalance.GuildMemberMax}번만 추가 가능</color>\n<color=red>(매일 오전 5시 초기화)</color>", null);
                        return;
                    }
                }
                else
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "오류가 발생했습니다. 잠시후 다시 시도해 주세요.", null);
                }

                ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value = 1;

                List<TransactionValue> transactions = new List<TransactionValue>();

                Param userInfoParam = new Param();

                userInfoParam.Add(UserInfoTable.SendGuildPoint, ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value);

                transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

                ServerData.SendTransaction(transactions, successCallBack: () =>
                {
                    var bro2 = Backend.URank.Guild.ContributeGuildGoods(RankManager.Rank_Guild_Reset_Uuid, goodsType.goods10, 1);

                    if (bro2.IsSuccess())
                    {

                        var bro = Backend.URank.Guild.ContributeGuildGoods(RankManager.Rank_Guild_Uuid, goodsType.goods2, rewardGrade);

                        if (bro.IsSuccess())
                        {
                            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "점수 추가 완료!", null);

                            var time = ServerData.userInfoTable.currentServerTime;

                            UiGuildChatBoard.Instance.SendRankScore_System($"<color=yellow>{PlayerData.Instance.NickName}님이 {rewardGrade}점을 추가했습니다.({time.Month}월 {time.Day}일 {time.Hour}시)");

                        }
                        else
                        {
                            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"점수 추가에 실패했습니다\n월요일 오전 4시~오전 5시에는 갱신할 수 없습니다\n({bro.GetStatusCode()})", null);

                            ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value = 0;

                            List<TransactionValue> transactions2 = new List<TransactionValue>();

                            Param userInfoParam2 = new Param();

                            userInfoParam2.Add(UserInfoTable.SendGuildPoint, ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value);

                            transactions2.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam2));

                            ServerData.SendTransaction(transactions2, successCallBack: () =>
                            {

                            });
                        }
                    }
                    else
                    {
                        PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"점수 추가에 실패했습니다\n오전 4시~오전 5시에는 갱신할 수 없습니다\n({bro2.GetStatusCode()})", null);

                        ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value = 0;

                        List<TransactionValue> transactions2 = new List<TransactionValue>();

                        Param userInfoParam2 = new Param();

                        userInfoParam2.Add(UserInfoTable.SendGuildPoint, ServerData.userInfoTable.TableDatas[UserInfoTable.SendGuildPoint].Value);

                        transactions2.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam2));

                        ServerData.SendTransaction(transactions2, successCallBack: () =>
                        {

                        });
                    }



                });
            }, null);
    }

}
