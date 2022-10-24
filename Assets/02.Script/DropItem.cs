﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//순서 절대바뀌면안됨
public enum Item_Type
{
    Gold,
    Jade,
    GrowthStone,
    Memory,
    Ticket,
    Marble,
    Dokebi,
    SkillPartion,
    WeaponUpgradeStone,
    PetUpgradeSoul,
    YomulExchangeStone,
    Songpyeon,
    TigerBossStone,
    Relic,
    RelicTicket,
    RabitBossStone,
    DragonBossStone,
    Event_Item_0,
    StageRelic,
    SnakeStone,
    PeachReal,
    HorseStone,
    SheepStone,
    MonkeyStone,
    MiniGameReward,
    GuildReward,
    CockStone,
    DogStone,
    SulItem,
    PigStone,
    SmithFire,
    FeelMulStone,

    Asura0,
    Asura1,
    Asura2,
    Asura3,
    Asura4,

    Event_Item_1,
    Asura5,
    Aduk,

    SinSkill0,
    SinSkill1,
    SinSkill2,
    SinSkill3,
    LeeMuGiStone,
    ZangStone,
    SP,//검조각
    Hae_Norigae,
    Hae_Pet,
    Sam_Norigae,
    Sam_Pet,

    Indra0,
    Indra1,
    Indra2,
    IndraWeapon,
    KirinNorigae,
    Kirin_Pet,
    IndraPower,
    Event_Item_Summer,
    NataWeapon,
    RabitNorigae,
    RabitPet,
    NataSkill,
    OrochiWeapon,
    OrochiSkill,
    OrochiTooth0,
    OrochiTooth1,
    DogNorigae,
    DogPet,

    MihoWeapon,
    MihoNorigae,
    ChunMaNorigae,
    ChunMaPet,
    Hel,
    Ym,
    YeaRaeWeapon,
    YeaRaeNorigae,

    GangrimWeapon,
    GangrimNorigae,
    GangrimSkill,
    //이덕춘 두루마리
    du,

    Sun0,
    Sun1,
    Sun2,
    Sun3,
    Sun4,
    HaeWeapon,
    Fw,
    Cw,
    ChunNorigae0,
    ChunNorigae1,
    ChunNorigae2,
    ChunSun0,
    ChunSun1,
    ChunSun2,
    Event_Fall,
    Event_Fall_Gold,
    ChunNorigae3,
    ChunNorigae4,
    FoxMaskPartial,

    RankFrame1 = 100,
    RankFrame2 = 101,
    RankFrame3 = 102,
    RankFrame4 = 103,
    RankFrame5 = 104,
    RankFrame6_20 = 105,
    RankFrame21_100 = 106,
    RankFrame101_1000 = 107,
    RankFrame1001_10000 = 108,

    RankFrame1_relic = 200,
    RankFrame2_relic = 201,
    RankFrame3_relic = 202,
    RankFrame4_relic = 203,
    RankFrame5_relic = 204,
    RankFrame6_20_relic = 205,
    RankFrame21_100_relic = 206,
    RankFrame101_1000_relic = 207,
    RankFrame1001_10000_relic = 208,

    RankFrame1_miniGame = 300,
    RankFrame2_miniGame = 301,
    RankFrame3_miniGame = 302,
    RankFrame4_miniGame = 303,
    RankFrame5_miniGame = 304,
    RankFrame6_20_miniGame = 305,
    RankFrame21_100_miniGame = 306,
    RankFrame101_1000_miniGame = 307,
    RankFrame1001_10000_miniGame = 308,

    RankFrame1_guild = 400,
    RankFrame2_guild = 401,
    RankFrame3_guild = 402,
    RankFrame4_guild = 403,
    RankFrame5_guild = 404,
    RankFrame6_20_guild = 405,
    RankFrame21_100_guild = 406,
    RankFrame101_1000_guild = 407,

    //신규
    RankFrame1guild_new = 600,
    RankFrame2guild_new = 601,
    RankFrame3guild_new = 602,
    RankFrame4guild_new = 603,
    RankFrame5guild_new = 604,
    RankFrame6_20_guild_new = 605,
    RankFrame21_50_guild_new = 606,
    RankFrame51_100_guild_new = 607,

    RankFrame1_boss_new = 700,
    RankFrame2_boss_new = 701,
    RankFrame3_boss_new = 702,
    RankFrame4_boss_new = 703,
    RankFrame5_boss_new = 704,
    RankFrame6_10_boss_new = 705,
    RankFrame10_30_boss_new = 706,
    RankFrame30_50boss_new = 707,
    RankFrame50_70_boss_new = 708,
    RankFrame70_100_boss_new = 709,
    RankFrame100_200_boss_new = 710,
    RankFrame200_500_boss_new = 711,
    RankFrame500_1000_boss_new = 712,
    RankFrame1000_3000_boss_new = 713,

    MagicStoneBuff = 500,

    RankFrame1_relic_hell = 800,
    RankFrame2_relic_hell = 801,
    RankFrame3_relic_hell = 802,
    RankFrame4_relic_hell = 803,
    RankFrame5_relic_hell = 804,
    RankFrame6_20_relic_hell = 805,
    RankFrame21_100_relic_hell = 806,
    RankFrame101_1000_relic_hell = 807,
    RankFrame1001_10000_relic_hell = 808,

    RankFrame1_2_war_hell = 809,
    RankFrame3_5_war_hell = 810,
    RankFrame6_20_war_hell = 811,
    RankFrame21_50_war_hell = 812,
    RankFrame51_100_war_hell = 813,
    RankFrame101_1000_war_hell = 814,
    RankFrame1001_10000_war_hell = 815,

    //1000~1100 무기
    weapon0 = 1000,
    weapon1 = 1001,
    weapon2 = 1002,
    weapon3 = 1003,
    weapon4 = 1004,
    weapon5 = 1005,
    weapon6 = 1006,
    weapon7 = 1007,
    weapon8 = 1008,
    weapon9 = 1009,
    weapon10 = 1010,
    weapon11 = 1011,
    weapon12 = 1012,
    weapon13 = 1013,
    weapon14 = 1014,
    weapon15 = 1015,
    weapon16 = 1016,
    //
    weapon37 = 1037,
    weapon38 = 1038,
    weapon39 = 1039,
    weapon40 = 1040,
    weapon41 = 1041,
    weapon42 = 1042,
    //

    //2000~2100 마도서
    magicBook0 = 2000,
    magicBook1 = 2001,
    magicBook2 = 2002,
    magicBook3 = 2003,
    magicBook4 = 2004,
    magicBook5 = 2005,
    magicBook6 = 2006,
    magicBook7 = 2007,
    magicBook8 = 2008,
    magicBook9 = 2009,
    magicBook10 = 2010,
    magicBook11 = 2011,
    //3000~3100스킬
    skill0 = 3000,
    skill1 = 3001,
    skill2 = 3002,
    skill3 = 3003,
    skill4 = 3004,
    skill5 = 3005,
    skill6 = 3006,
    skill7 = 3007,
    skill8 = 3008,
    skill9 = 3009,
    skill10 = 3010,
    skill11 = 3011,
    //코스튬 테이블 키값임 대소문자 변경X
    costume0 = 1201,
    costume1 = 1202,
    costume2 = 1203,
    costume3 = 1204,
    costume4 = 1205,
    costume5 = 1206,
    costume6 = 1207,
    costume7 = 1208,
    costume8 = 1209,
    costume9 = 1210,
    costume10 = 1211,
    costume11 = 1212,
    costume12 = 1213,
    costume13 = 1214,
    costume14 = 1215,
    costume15 = 1216,
    costume16 = 1217,
    costume17 = 1218,
    costume18 = 1219,
    costume19 = 1220,
    costume20 = 1221,
    costume21 = 1222,
    costume22 = 1223,

    costume23 = 1224,
    costume24 = 1225,
    costume25 = 1226,
    costume26 = 1227,

    costume27 = 1228,
    costume28 = 1229,
    costume29 = 1230,

    costume30 = 1231,
    costume31 = 1232,

    costume32 = 1233,
    costume33 = 1234,
    costume34 = 1235,
    costume35 = 1236,
    costume36 = 1237,
    costume37 = 1238,
    costume38 = 1239,
    costume39 = 1240,
    costume40 = 1241,
    costume41 = 1242,

    costume42 = 1243,
    costume43 = 1244,

    costume44 = 1245,
    costume45 = 1246,
    costume46 = 1247,
    costume47 = 1248,
    costume48 = 1249,
    costume49 = 1250,
    costume50 = 1251,
    costume51 = 1252,
    costume52 = 1253,
    costume53 = 1254,

    costume54 = 1255,
    costume55 = 1256,

    costume56 = 1257,
    costume57 = 1258,
    costume58 = 1259,

    costume59 = 1260,
    costume60 = 1261,
    costume61 = 1262,

    costume62 = 1263,
    costume63 = 1264,
    costume64 = 1265,
    costume65 = 1266,

    pet0 = 1301,
    pet1 = 1302,
    pet2 = 1303,
    pet3 = 1304,



    gumiho0 = 5000,
    gumiho1 = 5001,
    gumiho2 = 5002,
    gumiho3 = 5003,
    gumiho4 = 5004,
    gumiho5 = 5005,
    gumiho6 = 5006,
    gumiho7 = 5007,
    gumiho8 = 5008,

    //지옥 전용템
    h0 = 6000,
    h1 = 6001,
    h2 = 6002,
    h3 = 6003,
    h4 = 6004,
    h5 = 6005,
    h6 = 6006,
    h7 = 6007,
    h8 = 6008,
    h9 = 6009,

    //천계 칠션녀 전용템
    c0 = 7000,
    c1 = 7001,
    c2 = 7002,
    c3 = 7003,
    c4 = 7004,
    c5 = 7005,
    c6 = 7006,

    ChunWeapon0 = 7007,
    ChunPet0 = 7008,
    ChunWeapon1 = 7009,
    ChunPet1 = 7010,

    RecommendWeapon0 = 8000,
    RecommendWeapon1 = 8001,
    RecommendWeapon2 = 8002,
    RecommendWeapon3 = 8003,
    RecommendWeapon4 = 8004,
}

public class DropItem : PoolItem
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private SpriteRenderer icon;

    private const float gravity = 2f;

    private Item_Type type;
    private float amount;

    private static WaitForSeconds dropItemLifeTime = new WaitForSeconds(120.0f);

    private new void OnDisable()
    {
        base.OnDisable();
        this.gameObject.layer = LayerMasks.DropItemSpawnedLayerMask;
    }

    private void OnEnable()
    {
        Spawned();
    }

    public void Initialize(Item_Type type, float amount)
    {
        this.type = type;

        this.amount = amount + amount * PlayerStats.GetDropAmountAddValue();

        SetIcon();
        SetLifeTime();
    }

    private void SetLifeTime()
    {
        StartCoroutine(LifeRoutine());
    }

    private IEnumerator LifeRoutine()
    {
        yield return dropItemLifeTime;
        this.gameObject.SetActive(false);
    }

    private void SetIcon()
    {
        icon.sprite = CommonUiContainer.Instance.GetItemIcon(type);
    }

    private void Spawned()
    {
        rb.gravityScale = 0f;
        collider.isTrigger = true;
        this.gameObject.layer = LayerMasks.DropItemSpawnedLayerMask;
    }

    private void WhenDropAnimEnd()
    {
        collider.isTrigger = false;
        rb.gravityScale = gravity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMasks.PlatformLayerMask)
        {
            rb.gravityScale = 0f;
            this.gameObject.layer = LayerMasks.DropItemLayerMask;
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(Tags.Player) || collision.gameObject.tag.Equals(Tags.Pet))
        {
            WhenItemTriggered();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(Tags.Player) || collision.gameObject.tag.Equals(Tags.Pet))
        {
            WhenItemTriggered();
        }
    }

    private void WhenItemTriggered()
    {
        this.gameObject.SetActive(false);

        ApplyItemData();
    }
    private void ApplyItemData()
    {
        switch (type)
        {
            case Item_Type.GrowthStone:
                {
                    ServerData.goodsTable.GetMagicStone(amount);
                }
                break;
            case Item_Type.Marble:
                {
                    ServerData.goodsTable.GetMarble(amount);
                }
                break;
        }
    }

}
