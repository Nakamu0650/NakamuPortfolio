using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class G_PoisonicHoldAttack : G_UmbrellaHoldAttack
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] Vector3 attackPivot1, attackPivot2;
    [SerializeField] Paramaters paramaters;
    [SerializeField] Durations durations;
    [SerializeField] ExplosionValue explosion;
    [SerializeField] Animations animations;

    private bool isAttacking;
    // Set is called before the first frame update
    public override void Set()
    {
        isAttacking = false;
    }

    public override void OnAttack()
    {
        base.OnAttack();
        if (isAttacking)
        {
            return;
        }
        StartCoroutine(attack());

        IEnumerator attack()
        {
            isAttacking = true;
            playerMove.canMove = false;
            playerMove.isKinematic = true;
            float multiply = 1f / durations.attack;
            playerMove.modelAnimator.SetFloat(animations.speed, multiply);
            playerMove.modelAnimator.SetTrigger(animations.animation);

            yield return new WaitForSeconds(durations.attack * durations.attack1Clamp);

            var evilSplits = new List<G_EvilSplit>();
            S_SEManager.PlayPlayerPoisionAttackSE1(transform);
            evilSplits.Add(Instantiate(attackPrefab, transform.position + transform.TransformDirection(attackPivot1), transform.rotation).GetComponent<G_EvilSplit>());
            evilSplits[0].transform.localEulerAngles += new Vector3(0f, 0f, 90f);
            evilSplits[0].speed = paramaters.firstSpeed;

            yield return new WaitForSeconds(durations.attack * (durations.attack2Clamp - durations.attack1Clamp));
            S_SEManager.PlayPlayerPoisionAttackSE2(transform);
            evilSplits.Add(Instantiate(attackPrefab, transform.position + transform.TransformDirection(attackPivot2), transform.rotation).GetComponent<G_EvilSplit>());

            evilSplits.ForEach(split =>
            {
                try
                {
                    split.onHitGround.AddListener(Explosion);
                    StartCoroutine(WaitExplosion(split.gameObject));

                }
                catch
                {
                    Debug.LogError("消すより前に消えてたのでエラーが出たよ");
                }
                split.speed = paramaters.fireSpeed;
            });

            yield return new WaitForSeconds((durations.attack - (durations.attack * (durations.attack1Clamp + durations.attack2Clamp))));
            isAttacking = false;
            playerMove.canMove = true;
            playerMove.isKinematic = false;
        }
    }

    private IEnumerator WaitExplosion(GameObject attack)
    {
        yield return new WaitForSeconds(explosion.waitDuration);
        Explosion(attack.transform);
        Destroy(attack);
    }

    public void Explosion(Transform attack)
    {
        S_SEManager.PlayPlayerPoisionAttackSE3(transform);
        Instantiate(explosion.prefab, attack.position, Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = explosion.radius * Vector3.one;
        var enemies = Physics.OverlapSphere(attack.position, explosion.radius, explosion.enemyLayer).Where(hit => hit.gameObject.CompareTag("Enemy")).Select(hit => hit.gameObject.GetComponent<F_HP>());
        foreach(F_HP enemy in enemies)
        {
            enemy.OnDamage(explosion.param.GetATK(), explosion.attackValue.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(attack.position, enemy.transform.position));
        }
    }

    [Serializable]
    private class Durations
    {
        public float attack1Clamp, attack2Clamp;
        public float attack;
        public float delete;
    }

    [Serializable]
    private class Paramaters
    {
        public float firstSpeed, fireSpeed;
    }

    [Serializable]
    private class Animations
    {
        public string animation, speed;
    }

    [Serializable]
    private class ExplosionValue
    {
        public GameObject prefab;
        public LayerMask enemyLayer;
        public F_Param param;
        public F_AttackValue attackValue;
        public float radius;
        public float waitDuration;
    }
}
