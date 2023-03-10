using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using static NetworkManager;
using System;
using Photon.Pun;
using BackEnd;

public class PartyRaidResultPopup : SingletonMono<PartyRaidResultPopup>
{
    [SerializeField]
    private TextMeshProUGUI totalText;

    [SerializeField]
    private GameObject recordButton;

    [SerializeField]
    private GameObject leaveOnlyButton;

    [SerializeField]
    private GameObject leaveOnlyButton_Tower;

    [SerializeField]
    private GameObject rewardChartButton;

    private void UpdateButton()
    {
        rewardChartButton.SetActive(PartyRaidManager.Instance.NetworkManager.IsGuildBoss());

        if (PartyRaidManager.Instance.NetworkManager.IsNormalBoss() == true)
        {
            recordButton.SetActive(true);
            leaveOnlyButton.SetActive(false);
        }
        else if (PartyRaidManager.Instance.NetworkManager.IsGuildBoss())
        {
            recordButton.SetActive(PhotonNetwork.IsMasterClient);
            leaveOnlyButton.SetActive(!PhotonNetwork.IsMasterClient);
        }
        else if (PartyRaidManager.Instance.NetworkManager.IsPartyTowerBoss() || PartyRaidManager.Instance.NetworkManager.IsPartyTower2Boss())
        {
            recordButton.SetActive(false);
            leaveOnlyButton.SetActive(false);
        }

        if (OnlineTowerManager.Instance != null)
        {
            var onlineTowerManager = OnlineTowerManager.Instance.GetComponent<OnlineTowerManager>();

            if (onlineTowerManager != null && onlineTowerManager.allPlayerEnd)
            {
                leaveOnlyButton_Tower.SetActive(true);
            }
        }

        if (PartyRaidManager.Instance.NetworkManager.IsPartyTower2Boss()) 
        {
            leaveOnlyButton_Tower.SetActive(true);
        }
    }

    public void ExitButtonActive()
    {
        Debug.LogError("ExitButtonActive");
        leaveOnlyButton_Tower.SetActive(true);
    }

    private void OnDisable()
    {
        leaveOnlyButton_Tower.SetActive(false);
    }


    private void OnEnable()
    {
        UpdateScoreBoard();

        UpdateButton();
    }

    public void OnClickRegistScoreButton()
    {
        if (PartyRaidManager.Instance.NetworkManager.IsGuildBoss() == false)
        {
            PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"????????? ???????????? ?????????????\n<color=red>(??????)?????? ??????????????? ????????? ????????? ????????? ?????? ?????????.\n??? : {Utils.ConvertBigNum(PartyRaidManager.Instance.NetworkManager.GetTotalScore())}???", () =>
            {

                RecordPartyRaidScore();
                LeaveRoom();

            }, () => { });
        }
        else
        {
            int totalScore = GetGuildPartyTotalScore();

            PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{totalScore}?????? ???????????? ?????????????\n<color=red>(??????)?????? ??????????????? ????????? ????????? ?????? ???????????? ?????? ?????????.", () =>
            {

                RecordGuildRaidScore();
                LeaveRoom();

            }, () => { });
        }


    }

    private void RecordPartyRaidScore()
    {
        double totalScore = PartyRaidManager.Instance.NetworkManager.GetTotalScore();

        //?????? ?????? ??????

        var twelveBossTable = TableManager.Instance.TwelveBossTable.dataArray[55];

        var serverData = ServerData.bossServerTable.TableDatas[twelveBossTable.Stringid];


        /////

        if ((string.IsNullOrEmpty(ServerData.etcServerTable.TableDatas[EtcServerTable.chunmaTopScore].Value)) ||
            double.Parse(ServerData.etcServerTable.TableDatas[EtcServerTable.chunmaTopScore].Value) < totalScore)
        {
            ServerData.etcServerTable.TableDatas[EtcServerTable.chunmaTopScore].Value = totalScore.ToString();
            //????????????
            RankManager.Instance.UpdateChunmaTop(totalScore);
        }
        else
        {
            Debug.LogError("????????? ??? ??????");
            return;
        }

        ServerData.etcServerTable.UpdateData(EtcServerTable.chunmaTopScore);
        

        if (string.IsNullOrEmpty(serverData.score.Value) == false)
        {
            if (totalScore < double.Parse(serverData.score.Value))
            {
                //return;
            }
            else
            {

                serverData.score.Value = totalScore.ToString();

                ServerData.bossServerTable.UpdateData(twelveBossTable.Stringid);
            }
        }
        else
        {


            serverData.score.Value = totalScore.ToString();

            ServerData.bossServerTable.UpdateData(twelveBossTable.Stringid);
        }
    }

    private int GetGuildPartyTotalScore()
    {
        double totalScore = PartyRaidManager.Instance.NetworkManager.GetTotalScore();

        int ret = -1;

        var tableData = TableManager.Instance.TwelveBossTable.dataArray[74];

        for (int i = 0; i < tableData.Rewardcut.Length; i++)
        {
            if (totalScore < tableData.Rewardcut[i])
            {
                ret = i;
                break;
            }
        }

        if (ret == -1)
        {
            ret = tableData.Rewardcut.Length;
        }

        return ret + 30;
    }

    private void RecordGuildRaidScore()
    {
        bool isRankUpdateTime = ServerData.userInfoTable.IsRankUpdateTime();

#if UNITY_EDITOR
        isRankUpdateTime = true;
#endif
        if (isRankUpdateTime == false)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "??????4??? ~ 5???????????? ????????? ???????????? ??? ????????????.", null);
            return;
        }

        int totalScore = GetGuildPartyTotalScore();

        if (totalScore == 0)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "????????? ????????? ????????????.", null);
            return;
        }

        recordButton.gameObject.SetActive(false);


        if (totalScore == 0)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "????????? ????????? ????????????.", null);
            recordButton.gameObject.SetActive(true);
            return;
        }

        var guildInfoBro = Backend.Social.Guild.GetMyGuildGoodsV3();

        if (guildInfoBro.IsSuccess())
        {
            var returnValue = guildInfoBro.GetReturnValuetoJSON();

            int currentScore = int.Parse(returnValue["goods"]["totalGoods7Amount"]["N"].ToString());

            int interval = totalScore - currentScore;

            if (interval > 0)
            {
                var bro2 = Backend.URank.Guild.ContributeGuildGoods(RankManager.Rank_GangChul_Guild_Boss_Uuid, goodsType.goods7, interval);

                if (bro2.IsSuccess())
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "?????? ?????? ??????!", null);
                    recordButton.gameObject.SetActive(true);
                    return;
                }
                else
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"?????? ????????? ??????????????????\n?????? ?????? ????????? ????????????.\n({bro2.GetStatusCode()})", null);
                    recordButton.gameObject.SetActive(true);
                    return;
                }
            }
            //????????????
            else
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"??????????????? ???????????? ???????????????\n???????????? {currentScore}???", null);
                recordButton.gameObject.SetActive(true);
                return;
            }

        }
        else
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "????????? ??????????????????. ????????? ?????? ????????? ?????????.", null);
            recordButton.gameObject.SetActive(true);
            return;
        }
    }

    public void LeaveRoom()
    {

        //?????? ?????? ??????
        PartyRaidManager.Instance.OnClickCloseButton();

        //????????? ????????????
        GameManager.Instance.LoadNormalField();
    }

    public void UpdateScoreBoard()
    {
        PartyRaidManager.Instance.NetworkManager.UpdateResultScore();

        totalText.SetText($"?????? ????????? : { Utils.ConvertBigNum(PartyRaidManager.Instance.NetworkManager.GetTotalScore())}");
    }
}
