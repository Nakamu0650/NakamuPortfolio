using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;

public class G_NemophilaHoldAttack : G_UmbrellaHoldAttack
{
    [SerializeField] GameObject cameraObject;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] F_AttackValue attackValue;
    [SerializeField] F_Param paramater;
    [SerializeField] float radius = 10f;
    [SerializeField] float spreadDuration = 0.5f;
    [SerializeField] Animations animations;

    private List<GameObject> hitEnemies;
    [SerializeField] UnityEvent OnHoldPlay,ShortGoal = new UnityEvent();
    bool isUsed = false;

    // Set is called before the first frame update
    public override void Set()
    {
    }


    public override void OnAttack()
    {
        base.OnAttack();
        playerMove.modelAnimator.SetFloat(animations.speed, 1f / animations.duration);
        playerMove.modelAnimator.SetTrigger(animations.onAnimation);
        StartCoroutine(OnAnimation());
        OnHoldPlay.Invoke();
        if(!isUsed)
        {
            ShortGoal.Invoke();
            isUsed = true;
        }
        G_BloomProvidenceAnalysisSystem.instance.data.nemophila++;
    }


    public IEnumerator OnAnimation()
    {
        playerMove.isKinematic = true;
        cameraObject.SetActive(true);
        yield return new WaitForSeconds(animations.throwUmbrellaClamp * animations.duration);
        var blend = animations.Summon();
        float raisingDuration = animations.duration * (animations.growClamp - animations.throwUmbrellaClamp);
        blend.obj.transform.DOMoveY(blend.obj.transform.position.y + animations.raiseHeight, raisingDuration).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(raisingDuration);
        blend.blend.ChangeBlendShapeAsNew(new float[] { 0f, 0f }, 0.25f, Ease.InQuart);
        S_SEManager.PlayPlayerNemophilaAttackSE(transform);
        StartCoroutine(AttackEnumerator());
        Instantiate(animations.effectPrefab, blend.obj.transform.position, Quaternion.Euler(90f, 0f, 0f));
        G_ShakeCamera.instance.ShakeCamera(2f);
        G_ShakeController.instance.ShakeController(0.3f, 0f, 1f);
        blend.obj.transform.DOMoveY(blend.obj.transform.position.y - 0.5f, 0.5f).SetEase(Ease.OutQuart);
        yield return new WaitForSeconds(animations.duration - (animations.duration * animations.growClamp));
        var sequence = DOTween.Sequence();
        playerMove.isKinematic = false;
        yield return new WaitForSeconds(1f);
        cameraObject.SetActive(false);
        sequence.Append(blend.obj.transform.DOScale(0f, 2f)).AppendCallback(() => Destroy(blend.obj));
    }

    public IEnumerator AttackEnumerator()
    {
        Vector3 position = playerMove.transform.position;
        hitEnemies = new List<GameObject>();

        playerMove.canMove = false;

        DOVirtual.Float(0f, radius, spreadDuration, f =>
        {
            var hits = Physics.OverlapSphere(position, f, hitLayer).Select(col => col.gameObject);
            var enemies = hits.Where(col => col.CompareTag("Enemy"));
            var hitFlowers = hits.Where(col => col.CompareTag("Flower")).Select(col => col.GetComponent<G_FlowerGroup>());

            Debug.Log("長さ"+enemies.ToArray().Length);
            foreach(var x in hits)
            {
                Debug.Log($"{x.name}:{x.CompareTag("Enemy")}");
            }

            foreach(GameObject enemy in enemies)
            {
                if (hitEnemies.Contains(enemy))
                {
                    return;
                }
                hitEnemies.Add(enemy);
                Debug.Log("呼ばれた");
                enemy.GetComponent<F_HP>().OnDamage(paramater.GetATK(), attackValue.attackValues[0], HanadayoRigidobody.GetKnockBackAxis(transform.position, enemy.transform.position));
            }
            foreach(G_FlowerGroup flower in hitFlowers)
            {
                flower.Bloom();
            }
        });

        yield return new WaitForSeconds(spreadDuration);

        playerMove.canMove = true;
    }

    [Serializable]
    private class Animations
    {
        public GameObject umbrellaPrefab;
        public GameObject effectPrefab;
        public Transform handTransform;
        public string onAnimation, speed;
        public float duration = 1f;
        public float raiseHeight = 3f;
        [Range(0f, 1f)] public float throwUmbrellaClamp;
        [Range(0f, 1f)] public float growClamp;

        public (GameObject obj, G_BlendShapeChanger blend) Summon()
        {
            var obj = Instantiate(umbrellaPrefab, handTransform.position, Quaternion.identity);
            var blend = obj.transform.GetChild(0).GetComponent<G_BlendShapeChanger>();
            return (obj, blend);
        }

    }

    
}
