using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
public class G_PoisonicLake : MonoBehaviour
{
    [SerializeField] F_AttackValue sustainedDamage,explosionDamage;
    [SerializeField] F_Param paramater;
    [SerializeField] GameObject ballObj,explosionObj,attackDecalObject, predictionDecalObject;
    [SerializeField] float lifeTime = 10f,destroyDuration=0.5f;
    [SerializeField] float radius = 3f, explosionRadius = 6f;
    [SerializeField] float damageSpan = 0.5f;
    [SerializeField] float gravityScale = -9.81f;
    [SerializeField] float explosionExpansionDuration = 0.1f;
    private bool onDamage;
    private F_HP playerHP;

    private void OnValidate()
    {
        transform.localScale = radius * Vector3.one;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //Falling
        onDamage = false;
        predictionDecalObject.SetActive(true);
        attackDecalObject.SetActive(false);
        predictionDecalObject.transform.localScale = Vector3.zero;
        predictionDecalObject.transform.DOScale(explosionRadius / 2f, destroyDuration).SetEase(Ease.OutSine);
        for(float g = 0; (Physics.OverlapSphere(transform.position, 0.1f).Where(value => value.gameObject.CompareTag("Ground")).ToArray().Length == 0); g += gravityScale*Time.fixedDeltaTime)
        {
            transform.position += Vector3.up * g;
            yield return new WaitForFixedUpdate();
            if (transform.position.y < -63f) { Destroy(gameObject); }
        }

        //Explosion
        explosionObj.SetActive(true);
        ballObj.SetActive(false);
        predictionDecalObject.SetActive(false);
        attackDecalObject.SetActive(true);
        S_SEManager.PlayLila_ExplosionPoisionSE(transform);
        Collider[] explosionCols = Physics.OverlapSphere(transform.position, explosionRadius).Where(value => value.gameObject.CompareTag("Player")).ToArray();
        explosionObj.transform.DOScale(explosionRadius * Vector3.one, explosionExpansionDuration).SetEase(Ease.OutExpo);
        if (explosionCols.Length != 0)
        {
            if (!playerHP) {playerHP = explosionCols[0].gameObject.GetComponent<F_HP>();}
            playerHP.OnDamage(paramater.GetATK(), explosionDamage.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(transform.position, playerHP.transform.position));
        }

        //Sustained
        int damageNum = 0;
        for (float f = 0,span = damageSpan; f < lifeTime; f += Time.deltaTime)
        {
            if (onDamage)
            {
                if (span >= damageSpan)
                {
                    span = 0f;
                    playerHP.OnDamage(paramater.GetATK(), sustainedDamage.attackValues[0], Vector3.zero);
                    if (sustainedDamage.attackValues.Length > 1)
                    {
                        damageNum = Mathf.Clamp(damageNum + 1, 0, sustainedDamage.attackValues.Length - 1);
                    }
                }
                span += Time.fixedDeltaTime;
            }
            else { span = damageSpan; }
            yield return new WaitForFixedUpdate();
        }

        attackDecalObject.transform.DOScale(Vector3.zero, destroyDuration);
        transform.DOScale(Vector3.zero, destroyDuration);
        DOVirtual.DelayedCall(destroyDuration, () => Destroy(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onDamage = true;
            if (!playerHP)
            {
                playerHP = other.gameObject.GetComponent<F_HP>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onDamage = false;
        }
    }

}
