﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using BackEnd;
using System.Linq;

public class UiSkillCell : MonoBehaviour
{
    [SerializeField]
    private Image skillIcon;

    [SerializeField]
    private TextMeshProUGUI skillName;

    [SerializeField]
    private TextMeshProUGUI skillDescription;

    [SerializeField]
    private Button setSlotButton;

    [SerializeField]
    private TextMeshProUGUI slotButtonDesc;

    [SerializeField]
    private TextMeshProUGUI levelDescription;

    [SerializeField]
    private Button removeInSlotButton;

    private SkillTableData skillData;

    private Action<int> onClickSlotSettingButton;

    private bool isSubscribed = false;

    private string lvTextFormat = "LV:{0}/{1}";

    private Action<SkillTableData> showDescriptionPopup;

    public void Initialize(SkillTableData skillData, Action<int> onClickSlotSettingButton, Action<SkillTableData> showDescriptionPopup)
    {
        this.showDescriptionPopup = showDescriptionPopup;

        this.skillData = skillData;

        skillIcon.sprite = CommonResourceContainer.GetSkillIconSprite(skillData);

        this.skillName.SetText($"{skillData.Skillname}({skillData.Skilltype})");
        this.skillName.color = CommonUiContainer.Instance.itemGradeColor[skillData.Skillgrade];

        this.skillDescription.SetText(skillData.Skilldesc);

        this.onClickSlotSettingButton = onClickSlotSettingButton;

        if (isSubscribed == false)
        {
            isSubscribed = true;
            Subscribe();
        }
    }
    public void ShowSkillDescriptionPopup()
    {
        showDescriptionPopup?.Invoke(skillData);
    }

    private void Subscribe()
    {
        DatabaseManager.skillServerTable.TableDatas[SkillServerTable.SkillAwakeNum][skillData.Id].AsObservable().Subscribe(CheckUnlock).AddTo(this);

        //스킬 각성시
        DatabaseManager.skillServerTable.TableDatas[SkillServerTable.SkillAwakeNum][skillData.Id].AsObservable().Subscribe(WhenSkillAwake).AddTo(this);

        //스킬 레벨업시
        DatabaseManager.skillServerTable.TableDatas[SkillServerTable.SkillLevel][skillData.Id].AsObservable().Subscribe(WhenSkillUpgraded).AddTo(this);


        DatabaseManager.skillServerTable.whenSelectedSkillIdxChanged.AsObservable().Subscribe(WhenSelectedSkillIdxChanged).AddTo(this);
    }

    private void WhenSkillAwake(int awakeNum)
    {
        RefreshSkillLvText();
    }

    private void WhenSkillUpgraded(int skillLevel)
    {
        RefreshSkillLvText();
    }

    private void WhenSelectedSkillIdxChanged(List<ReactiveProperty<int>> skillList)
    {
        SetRemoveButton(DatabaseManager.skillServerTable.AlreadyEquipedSkill(skillData.Id));
    }

    private void RefreshSkillLvText()
    {
        int skillLevel = DatabaseManager.skillServerTable.GetSkillCurrentLevel(skillData.Id);
        int maxLevel = DatabaseManager.skillServerTable.GetSkillMaxLevel(skillData.Id);

        levelDescription.SetText(string.Format(lvTextFormat, skillLevel, maxLevel));
    }


    private void CheckUnlock(int currentLevel)
    {
        int maxLevel = DatabaseManager.skillServerTable.GetSkillMaxLevel(skillData.Id);
        setSlotButton.interactable = maxLevel > 0;

        if (setSlotButton.interactable == false)
        {
            slotButtonDesc.SetText($"미습득");
            SetRemoveButton(false);
        }
        else
        {
            slotButtonDesc.SetText("슬롯에 등록");


            SetRemoveButton(DatabaseManager.skillServerTable.AlreadyEquipedSkill(skillData.Id));
        }
    }

    public void OnClickSlotSettingButton()
    {
        //슬롯팝업
        onClickSlotSettingButton.Invoke(skillData.Id);
    }

    public void OnClickRemoveButton()
    {
        DatabaseManager.skillServerTable.RemoveSkillInEquipList(skillData.Id);

        if (AutoManager.Instance.IsAutoMode)
        {
            AutoManager.Instance.ResetSkillQueue();
        }
    }

    private void SetRemoveButton(bool onOff)
    {
        removeInSlotButton.gameObject.SetActive(onOff);
    }

}
