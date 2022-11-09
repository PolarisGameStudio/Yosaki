﻿using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTopRankerCell : MonoBehaviour
{
    [SerializeField]
    private SkeletonGraphic costumeGraphic;
    [SerializeField]
    private SkeletonGraphic petGraphic;
    [SerializeField]
    private TextMeshProUGUI nickName;
    [SerializeField]
    private TextMeshProUGUI rankText;

    [SerializeField]
    private BoneFollowerGraphic boneFollowerGraphic_Mask;

    [SerializeField]
    private Image weapon;
    [SerializeField]
    private Image magicBook;

    [SerializeField]
    private Image mask;

    [SerializeField]
    private Image horn;


    [SerializeField]
    private GameObject yomulObject;

    [SerializeField]
    private GameObject newWeaponEffect;

    [SerializeField]
    private List<GameObject> norigaeEffects;

    [SerializeField]
    private TextMeshProUGUI guildName;

    [SerializeField]
    private TextMeshProUGUI topRankerScoreText;

    [SerializeField]
    private TextMeshProUGUI fightText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private GameObject partyRaidRecommendButton;

    [SerializeField]
    private GameObject leaveObject;

    [SerializeField]
    private bool isPartyRaidPlayer = false;

    public string recNickName { get; private set; }

    [SerializeField]
    private BoneFollowerGraphic boneFollowerGraphic_Weapon;

    public void UpdatePartyRaidScore(double topRankerScore = 0, bool fightEnd = false)
    {
        if (topRankerScoreText != null)
        {
            topRankerScoreText.SetText(Utils.ConvertBigNum(topRankerScore));
        }

        if (fightText != null)
        {
            fightText.color = fightEnd ? Color.yellow : Color.red;
            fightText.SetText(fightEnd ? "전투완료" : "전투중");
        }
    }

    public void SetLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.SetText($"LV:{level}");
        }
    }

    public void Initialize(string nickName, string rankText, int costumeId, int petId, int weaponId, int magicBookId, int gumgiIdx, string guildName, int maskIdx,int hornIdx)
    {
        this.recNickName = nickName;
        this.nickName.SetText(nickName);
        this.rankText.SetText(rankText);
        SetCostumeSpine(costumeId);
        SetPetSpine(petId);

        this.guildName.gameObject.SetActive(string.IsNullOrEmpty(guildName) == false);
        this.guildName.SetText($"({guildName})");

        weapon.gameObject.SetActive(weaponId != -1);

        yomulObject.SetActive(weaponId == 20);

        newWeaponEffect.SetActive(weaponId == 22);

        if (maskIdx != -1)
        {
            mask.gameObject.SetActive(true);
            mask.sprite = CommonResourceContainer.GetMaskSprite(maskIdx);
        }
        else
        {
            mask.gameObject.SetActive(false);
        }
        
        if (hornIdx != -1)
        {
            horn.gameObject.SetActive(true);
            horn.sprite = CommonResourceContainer.GetHornSprite(hornIdx);
        }
        else
        {
            horn.gameObject.SetActive(false);
        }

        if (weaponId != -1)
        {
            weapon.sprite = CommonResourceContainer.GetWeaponSprite(weaponId);

            if (gumgiIdx < CommonUiContainer.Instance.weaponEnhnaceMats.Count)
            {
                weapon.material = CommonUiContainer.Instance.weaponEnhnaceMats[gumgiIdx];
            }
            else
            {
                weapon.material = CommonUiContainer.Instance.weaponEnhnaceMats[0];
            }
        }

        if (isPartyRaidPlayer == false)
        {
            //부채
            if (weaponId >= 37 && weaponId <= 41)
            {
                weapon.transform.position = new Vector3(-9f, 117.7f, 0);
                weapon.transform.rotation = Quaternion.Euler(0f, 0f, 133.9f);
            }
            else
            {
                weapon.transform.position = new Vector3(-1.4f, 128.4f, 0);
                weapon.transform.rotation = Quaternion.Euler(0f, 0f, 94f);

            }
        }
        else
        {
            //부채
            if (weaponId >= 37 && weaponId <= 41)
            {
                weapon.transform.localPosition = new Vector3(-1.5f, 46f, 0);
                weapon.transform.localRotation = Quaternion.Euler(0f, 0f, -382.72f);
            }
            else
            {
                weapon.transform.localPosition = new Vector3(71.9f, -65.6f, 0);
                weapon.transform.localRotation = Quaternion.Euler(0f, 0f, -270.1f);
            }
        }



        magicBook.gameObject.SetActive(magicBookId != -1);

        if (magicBookId != -1)
        {
            magicBook.sprite = CommonResourceContainer.GetMagicBookSprite(magicBookId);

            if (magicBookId < 16)
            {
                norigaeEffects.ForEach(e => e.SetActive(false));
            }
            else
            {
                int effectIdx = magicBookId % 4;

                if (magicBookId == 20)
                {
                    effectIdx = 4;
                }

                for (int i = 0; i < norigaeEffects.Count; i++)
                {
                    norigaeEffects[i].SetActive(i == effectIdx);
                }
            }
        }


        //십만대산용
        if (partyRaidRecommendButton != null)
        {
            if (GameManager.contentsType != GameManager.ContentsType.PartyRaid_Guild)
            {
                bool isMyNickName = Utils.GetOriginNickName(PlayerData.Instance.NickName).Equals(nickName);

                partyRaidRecommendButton.SetActive(!isMyNickName);
            }
            else
            {
                partyRaidRecommendButton.SetActive(false);
            }
        }

        if (leaveObject != null)
        {
            leaveObject.SetActive(false);
        }

        if (boneFollowerGraphic_Weapon != null)
        {
            boneFollowerGraphic_Weapon.SetBone("bone15");
        }
    }

    public void OnPlayerLeftInPartyRaid()
    {
        partyRaidRecommendButton.SetActive(false);
        leaveObject.SetActive(true);
    }

    public void OnClickRecommendButton()
    {
        if (GameManager.contentsType == GameManager.ContentsType.PartyRaid_Guild)
        {
            PopupManager.Instance.ShowAlarmMessage($"대산군에서는 추천을 하실 수 없습니다.");
            return;
        }

        PopupManager.Instance.ShowAlarmMessage($"{recNickName}님을 추천 합니다.");

        PartyRaidManager.Instance.NetworkManager.SendRecommend(recNickName);

        partyRaidRecommendButton.SetActive(false);


    }

    private void SetPetSpine(int idx)
    {

        if (idx == -1)
        {
            petGraphic.gameObject.SetActive(false);
            return;
        }


        if (idx <= 14)
        {
            petGraphic.startingAnimation = "walk";
            petGraphic.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(44.9f, 74.7f, 0.7f);
        }
        else if (idx == 15)
        {
            petGraphic.startingAnimation = "idel";
            petGraphic.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(44.9f, 138.2f, 0.7f);
        }
        else if (idx == 16 || idx == 19)
        {
            petGraphic.startingAnimation = "idle";
            petGraphic.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(42.1f, 144.6f, 0.7f);
        }
        else if (idx == 17)
        {
            petGraphic.startingAnimation = "idle";
            petGraphic.transform.localScale = new Vector3(0.3f, 0.3f, 0.14f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(49.4f, 141f, 0.0f);
        }
        else if (idx == 18)
        {
            petGraphic.startingAnimation = "idle";
            petGraphic.transform.localScale = new Vector3(0.3f, 0.3f, 0.14f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(49.4f, 141f, 0.0f);
        }
        //개
        else if (idx == 20)
        {
            petGraphic.startingAnimation = "walk";
            petGraphic.transform.localScale = new Vector3(-0.2f, 0.2f, 0.14f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(58.1f, 144.6f, 0.0f);
        }
        //고양이
        else if (idx == 21 || idx == 22 || idx == 23)
        {
            petGraphic.startingAnimation = "walk";
            petGraphic.transform.localScale = new Vector3(0.2f, 0.2f, 0.14f);
            petGraphic.GetComponent<RectTransform>().anchoredPosition = new Vector3(58.1f, 144.6f, 0.0f);
        }

        petGraphic.gameObject.SetActive(true);
        petGraphic.Clear();
        petGraphic.skeletonDataAsset = CommonUiContainer.Instance.petCostumeList[idx];
        petGraphic.Initialize(true);
        petGraphic.SetMaterialDirty();

    }

    private void SetCostumeSpine(int idx)
    {
        costumeGraphic.Clear();
        costumeGraphic.skeletonDataAsset = CommonUiContainer.Instance.costumeList[idx];
        costumeGraphic.Initialize(true);
        costumeGraphic.SetMaterialDirty();

        boneFollowerGraphic_Mask.SetBone("bone21");
        //boneFollowerGraphic.SetBone("Weapon1");
    }
}
