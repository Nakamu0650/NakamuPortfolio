using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

[RequireComponent(typeof(F_HP))]
[RequireComponent(typeof(Rigidbody))]
public class G_BossBase : MonoBehaviour
{
    [SerializeField] protected GameObject modelObject;
    [SerializeField] Slider hpSlider;
    [SerializeField] protected Move move;
    [SerializeField] protected Warp warp;
    [SerializeField] protected Animation anim;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected F_Param param;
    public Transform player;
    protected Transform cameraTransform;
    protected bool isContact;
    protected Rigidbody rb;
    protected F_HP hp;
    private Vector3 startPos;
    private Vector3 moveVelocity;

    [Serializable]
    protected class Magic
    {
        public float chantingTime;
        public float afterAttackWait;
        public float minWaitTime, maxWaitTime;

        public float GetWaitTime()
        {
            return UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        }
    }
    [Serializable]
    protected class Move
    {
        public float moveSpeed;
        public float rotateSpeed;
    }
    [Serializable]
    protected class Warp : Magic
    {
        public GameObject prefab;
        public List<Transform> warpTransforms;
        public float invisibleTime;
    }
    [Serializable]
    protected class Animation
    {
        public Animator animator;
        public string move, warp, moveSign;
    }

    // Start is called before the first frame update
    public void Start()
    {
        isContact = false;
        startPos = transform.position;
        cameraTransform = Camera.main.transform;
        moveVelocity = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        P_CameraMove_Alpha.instance.SetLastAttackEnemy(transform);
        P_CameraMove_Alpha.instance.AddAngryEnemy(transform);
    }

    public virtual void OnBattleStart()
    {
        isContact = true;
    }

    public virtual void OnKilled()
    {
        G_BloomProvidenceAnalysisSystem.instance.data.lilaKilled = true;
        StopAllCoroutines();
    }

    public void OnDamage()
    {
        float percent = GetHP().GetHPPercent();
        DOVirtual.Float(hpSlider.value, percent, 0.5f, value => hpSlider.value = value);
    }

    public IEnumerator MoveFoward(Vector3 direction, float duration)
    {
        GetRigidbody().isKinematic = false;
        for(float f = 0f; f < 1f; f += Time.fixedDeltaTime / duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z)), Time.fixedDeltaTime * move.rotateSpeed);
            moveVelocity = move.moveSpeed * transform.forward;
            MoveTo();
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator MoveBack(Vector3 direction, float duration)
    {
        GetRigidbody().isKinematic = false;
        for (float f = 0f; f < 1f; f += Time.fixedDeltaTime / duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z)), Time.fixedDeltaTime * move.rotateSpeed);
            moveVelocity = -move.moveSpeed * transform.forward;
            MoveTo();
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator WarpToGround()
    {
        Vector3 warpPos = warp.warpTransforms[UnityEngine.Random.Range(0, warp.warpTransforms.Count)].position;
        Vector3 playerPosition = player.position;


        anim.animator.SetTrigger(anim.warp);
        yield return new WaitForSeconds(warp.chantingTime);
        Instantiate(warp.prefab, transform.position + Vector3.one, Quaternion.identity);
        S_SEManager.PlayLila_WarpSE2(transform);
        modelObject.SetActive(false);
        yield return new WaitForSeconds(warp.invisibleTime);
        Quaternion afterRot = Quaternion.Euler(0f, Quaternion.LookRotation(playerPosition - warpPos, transform.up).eulerAngles.y, 0f);
        modelObject.SetActive(true);
        transform.position = warpPos;
        transform.rotation = afterRot;
        Instantiate(warp.prefab, transform.position + Vector3.one, Quaternion.identity);
        S_SEManager.PlayLila_WarpSE(transform);
        yield return new WaitForSeconds(warp.afterAttackWait + warp.GetWaitTime());
    }
    private void MoveTo()
    {
        //Animation Set
        Vector2 axis = new Vector2(moveVelocity.x, moveVelocity.z);
        float magnitude = axis.magnitude;
        if (magnitude == 0f)
        {
            anim.animator.SetFloat(anim.move, 0f);
        }
        else
        {
            anim.animator.SetFloat(anim.move, Mathf.InverseLerp(0f, move.moveSpeed, magnitude));
            Vector2 direction = new Vector2(transform.forward.x, transform.forward.z).normalized;
            float dot = Vector2.Dot(axis.normalized, direction);
            anim.animator.SetFloat(anim.moveSign, Mathf.Sign(dot));
        }


        moveVelocity = new Vector3(moveVelocity.x, GetRigidbody().velocity.y, moveVelocity.z);
        GetRigidbody().velocity = moveVelocity;
        moveVelocity = Vector3.zero;
    }

    protected F_HP GetHP()
    {
        if (!hp)
        {
            hp = GetComponent<F_HP>();
        }
        return hp;
    }

    protected Rigidbody GetRigidbody()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }
        return rb;
    }

}
