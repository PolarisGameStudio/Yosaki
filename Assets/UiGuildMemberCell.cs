﻿using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiGuildMemberCell : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nickName;
    [SerializeField]
    private TextMeshProUGUI lastLogin;
    [SerializeField]
    private TextMeshProUGUI donateAmount;
    [SerializeField]
    private TextMeshProUGUI grade;
    public GuildMemberInfo guildMemberInfo { get; private set; }

    public GameObject kickButton;

    [SerializeField]
    private GameObject donatedObject;

    public enum GuildGrade
    {
        Member, ViceMaster, Master
    }

    public class GuildMemberInfo
    {
        public string nickName { get; private set; }
        public GuildGrade guildGrade { get; private set; }
        public string lastLogin { get; private set; }
        public string gamerIndate { get; private set; }

        public int donateGoods { get; private set; }

        public bool todayDonated { get; private set; } = false;


        public GuildMemberInfo(string nickName, string position, string lastLogin, string gamerIndate, int donateGoods, bool todayDonated)
        {
            this.nickName = nickName;

            switch (position)
            {
                case "master": { guildGrade = GuildGrade.Master; } break;
                case "member": { guildGrade = GuildGrade.Member; } break;
                default: { guildGrade = GuildGrade.ViceMaster; } break;
            }
            this.lastLogin = lastLogin;

            this.gamerIndate = gamerIndate;

            this.donateGoods = donateGoods;

            this.todayDonated = todayDonated;
        }
    }

    public void Initialize(GuildMemberInfo guildMemberInfo)
    {
        this.guildMemberInfo = guildMemberInfo;

        nickName.SetText($"{guildMemberInfo.nickName.Replace(CommonString.IOS_nick, "")}");

        donateAmount.SetText($"{Utils.ConvertBigNum(guildMemberInfo.donateGoods)}점 추가");

        if (guildMemberInfo.lastLogin.Length >= 16)
        {
            DateTime loginTime = DateTime.Parse(guildMemberInfo.lastLogin);
            //loginTime = loginTime.AddHours(9);

            lastLogin.gameObject.SetActive(true);
            lastLogin.SetText($"{loginTime.Year}년 {loginTime.Month}월 {loginTime.Day}일 {loginTime.Hour}시 {loginTime.Minute}분 마지막 로그인");
        }
        else
        {
            lastLogin.gameObject.SetActive(false);
        }

        donatedObject.gameObject.SetActive(guildMemberInfo.todayDonated);


        RefreshKickButton();

        grade.SetText($"({CommonString.GetGuildGradeName(this.guildMemberInfo.guildGrade)})");

        switch (this.guildMemberInfo.guildGrade)
        {
            case GuildGrade.Member:
                grade.color = Color.white;
                break;
            case GuildGrade.ViceMaster:
                grade.color = Color.yellow;
                break;
            case GuildGrade.Master:
                grade.color = Color.magenta;
                break;
        }

        //)
    }

    public void RefreshKickButton()
    {
        if (UiGuildMemberList.Instance.GetMyGuildGrade() == GuildGrade.Member)
        {
            kickButton.SetActive(false);
        }
        else if (UiGuildMemberList.Instance.GetMyGuildGrade() == GuildGrade.ViceMaster)
        {
            kickButton.SetActive(PlayerData.Instance.NickName != guildMemberInfo.nickName && guildMemberInfo.guildGrade == GuildGrade.Member);
        }
        else if (UiGuildMemberList.Instance.GetMyGuildGrade() == GuildGrade.Master)
        {
            kickButton.SetActive(PlayerData.Instance.NickName != guildMemberInfo.nickName);
        }
    }

    public void OnClickKickButton()
    {
        PopupManager.Instance.ShowYesNoPopup("알림", $"({guildMemberInfo.nickName})유저를 추방 하시겠습니다?", () =>
        {
            var bro = Backend.Social.Guild.ExpelMemberV3(guildMemberInfo.gamerIndate);

            if (bro.IsSuccess())
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "추방 했습니다", null);
                UiGuildMemberList.Instance.RemovePlayer(guildMemberInfo.nickName);

            }
            else
            {
                PopupManager.Instance.ShowConfirmPopup("오류", $"권한이 없거나 이미 탈퇴한 유저 입니다.\n{bro.GetStatusCode()}", null);
            }
        }, null);

    }

}
