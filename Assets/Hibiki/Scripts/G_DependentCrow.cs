using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class G_DependentCrow : MonoBehaviour
{
    [SerializeField] G_CrowAnimator animator;
    [SerializeField] F_AttackValue attackValue;
    [SerializeField] F_Param param;
    [SerializeField] float moveSpeed = 20f,swoopSpeed = 20f,rushSpeed=30f,rotationSpeed = 10f, sqrReachedThreshold=0.25f,giveUpDuration=10f,swoopGroundDistance,groundMinDistance,groundMaxDistance;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] Transform hitPoint;
    [SerializeField] float hitRadius = 0.5f;
    private Transform player;
    private bool isAttacked;
    [HideInInspector] public bool goAttack=false;

    public void Setting(Transform _player)
    {
        player = _player;
        isAttacked = false;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        animator.SetRise(1f);
        while (!goAttack)
        {
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        animator.SetRise(0f);
        bool isHit = false;
        while (!isHit)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.down), Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * swoopSpeed * Time.deltaTime;
            Ray ray = new Ray(transform.position, Vector3.down);
            var hits = Physics.RaycastAll(ray, swoopGroundDistance).Where(hit => hit.collider.gameObject.CompareTag("Ground")).ToArray();
            isHit = ((hits.Length != 0)||(transform.position.y<=0f));
            yield return null;
        }

        float time = 0f;
        Vector3 playerPosition = player.position;
        while ((Vector3.SqrMagnitude(playerPosition - transform.position) >= sqrReachedThreshold) && (time <= giveUpDuration))
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            var hits = Physics.RaycastAll(ray, groundMaxDistance).Where(hit => hit.collider.gameObject.CompareTag("Ground")).ToArray();
            Vector3 direction = (playerPosition - transform.position).normalized;
            if (hits.Where(hit => hit.distance < groundMinDistance).ToArray().Length >= 1)
            {
                direction += Vector3.up*0.25f;
                direction.Normalize();
            }
            else if (hits.Length == 0)
            {
                direction += Vector3.down * 0.25f;
                direction.Normalize();
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed * 10f);
            transform.position += transform.forward * rushSpeed * Time.deltaTime;

            if (!isAttacked)
            {
                var cols = Physics.OverlapSphere(hitPoint.position, hitRadius, hitLayer).Where(col => col.gameObject.CompareTag("Player"));
                foreach (var col in cols)
                {
                    col.gameObject.GetComponent<F_HP>().OnDamage(param.GetATK(), attackValue.attackValues[0], Random.onUnitSphere);
                    isAttacked = true;
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        while (transform.position.y <= 300f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.up), rotationSpeed * Time.fixedDeltaTime/20f);
            transform.position += transform.forward * rushSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
