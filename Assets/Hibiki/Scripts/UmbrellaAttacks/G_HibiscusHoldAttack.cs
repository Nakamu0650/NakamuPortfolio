using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class G_HibiscusHoldAttack : G_UmbrellaHoldAttack
{
    [SerializeField] AttackValues values;
    [SerializeField] F_AttackValue attackValue;
    [SerializeField] F_Param param;

    [SerializeField] string numberAnimation, speedAnimation;

    private Dictionary<GameObject, F_HP> enemiesMemory;
    private Dictionary<Transform, Vector3> takeEnemies;

    public override void Set()
    {
        base.Set();
        enemiesMemory = new Dictionary<GameObject, F_HP>();
        takeEnemies = new Dictionary<Transform, Vector3>();
    }

    public override void OnAttack()
    {
        base.OnAttack();
        StartCoroutine(Attack());

        G_BloomProvidenceAnalysisSystem.instance.data.hibiscus++;
    }

    private IEnumerator Attack()
    {
        playerMove.isKinematic = true;
        SetSpeed(1f / values.beforeGapDuration);
        SetNumber(1);
        yield return new WaitForSeconds(values.beforeGapDuration);
        playerMove.canMove = false;
        playerMove.isKinematic = false;
        takeEnemies = new Dictionary<Transform, Vector3>();
        S_SEManager.PlayPlayerHibiscusAttackSE(transform);
        float time = 0f;
        int hitcount = 0;
        int count = 0;
        SetSpeed(values.rotateSpeed);
        SetNumber(2);
        for (float f = 0f; f <= values.attackDuration; f += Time.fixedDeltaTime)
        {
            if(time >= values.attackSpan)
            {
                time = 0f;
                var enemies = GetNearEnemies();
                count++;
                foreach (var enemy in enemies)
                {
                    enemy.OnDamage(param.GetATK(), attackValue.attackValues[0], Vector3.zero);
                    hitcount++;
                    if (!takeEnemies.ContainsKey(enemy.transform))
                    {
                        takeEnemies.Add(enemy.transform, (enemy.transform.position - transform.position));
                    }
                }
            }
            Vector3 velocity = playerMove.transform.forward * values.pushPower;
            float deceraton = playerMove.SlopeDeceleration(velocity).deceleration;
            velocity *= deceraton;
            playerMove.rb.velocity = new Vector3(velocity.x, playerMove.rb.velocity.y, velocity.z);
            foreach (var takeEnemy in takeEnemies)
            {
                takeEnemy.Key.position = (transform.position + takeEnemy.Value);
            }
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerMove.isKinematic = true;
        SetSpeed(1f / values.afterGapDuration);
        SetNumber(0);
        yield return new WaitForSeconds(values.afterGapDuration);
        playerMove.canMove = true;
        playerMove.isKinematic = false;
    }

    private void SetSpeed(float speed)
    {
        playerMove.modelAnimator.SetFloat(speedAnimation, speed);
    }
    private void SetNumber(int num)
    {
        playerMove.modelAnimator.SetInteger(numberAnimation, num);
    }


    private F_HP[] GetNearEnemies()
    {
        var hits = Physics.OverlapSphere(transform.position, values.radius, values.enemyLayer);

        return hits.Where(hit => hit.CompareTag("Enemy")).Select(hit => GetEnemyHP(hit.gameObject)).ToArray();
    }

    private F_HP GetEnemyHP(GameObject obj)
    {
        if (enemiesMemory.ContainsKey(obj))
        {
            return enemiesMemory[obj];
        }
        else
        {
            F_HP hp = obj.GetComponent<F_HP>();
            enemiesMemory.Add(obj, hp);
            return hp;
        }
    }

    [Serializable]
    private class AttackValues
    {
        public LayerMask enemyLayer;
        public float radius;
        public float beforeGapDuration, afterGapDuration;
        public float attackDuration;
        public float attackSpan;
        public float pushPower;
        public float rotateSpeed = 2f;
    }
}
