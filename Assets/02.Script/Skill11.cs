﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Skill11 : SkillBase
{
    private float jumpPower = 50f;
    private bool attractEnemy = true;
    public Skill11()
    {
        attractEnemy = true;
        jumpPower = 50f;
        damageApplyInterval = new WaitForSeconds(0.07f);
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Vector3 overlapCircleOrigin = playerTr.position + playerSkillCaster.GetSkillCastingPosOffset(skillInfo);

        Vector3 rayDirection = Vector3.up;

        var hitEnemies = playerSkillCaster.GetEnemiesInBoxcast(playerTr.position, rayDirection, 15, skillInfo.Targetrange).Select(e => e.collider).ToArray();

        //발동 이펙트
        //+이펙트

        //이동제한있을경우
        playerSkillCaster.SetMoveRestriction(skillInfo.Movedelay);

        //데미지
        float damage = GetSkillDamage(skillInfo);

        //승천
        PlayerMoveController.Instance.AddForce(Vector3.up, jumpPower);

        //데미지적용
        for (int i = 0; i < hitEnemies.Length && i < skillInfo.Targetcount; i++)
        {
            PlayerSkillCaster.Instance.StartCoroutine(playerSkillCaster.ApplyDamage(hitEnemies[i], skillInfo, damage, damageApplyInterval));

            if (attractEnemy && hitEnemies[i].transform.tag.Equals(Tags.Boss) == false)
            {
                hitEnemies[i].transform.localPosition = new Vector3(playerTr.position.x, hitEnemies[i].transform.localPosition.y, hitEnemies[i].transform.localPosition.z);

                var hitObject = hitEnemies[i].gameObject.GetComponentInChildren<EnemyHitObject>();
                if (hitObject != null)
                {
                    hitObject.SetTriggerRoutine();
                }

            }
        }
    }
}
