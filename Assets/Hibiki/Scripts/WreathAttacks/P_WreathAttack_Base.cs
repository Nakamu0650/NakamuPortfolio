using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Linq;

public class P_WreathAttack_Base : MonoBehaviour
{
    public static Vector2 sensitivity;

    [SerializeField] protected Transform player;
    [SerializeField] protected P_PlayerMove playerMove;
    [SerializeField] protected F_Param playerParam;
    [SerializeField] protected F_AttackValue attackValue;
    [SerializeField] protected G_Flower.FlowerList flower;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected UnityEvent onPressedButton, onHoldButton, onActivate;
    [SerializeField] protected float coolTime = 1f;
    [SerializeField] protected float searchEnemyDadius = 3f;
    [SerializeField] Vector3 searchEnemyDirection = Vector3.forward;

    public bool isActive = false;
    private bool isHold = false;
    private bool isCanceled = false;
    protected G_FlowerEnergyReceiver receiver;
    protected P_PlayerSelectAttackWreath selector;
    protected GameManager gameManager;
    protected float nowCoolTime = 0f;
    protected float speedBuff;
    protected float strengthBuff;

    protected bool useCoolTime;

    // Start is called before the first frame update
    public void Start()
    {
        isHold = false;
        isCanceled = false;
        useCoolTime = (coolTime != 0f);
        nowCoolTime = 0f;
        speedBuff = 1f;
        strengthBuff = 1f;
        gameManager = GameManager.instance;
        receiver = G_FlowerEnergyReceiver.instance;
        selector = P_PlayerSelectAttackWreath.instance;
    }

    public void Update()
    {
        if (!useCoolTime) return;

        if (nowCoolTime > 0f)
        {
            nowCoolTime -= Time.deltaTime;
            if (nowCoolTime < 0f)
            {
                nowCoolTime = 0f;
            }
        }
    }

    //Call with InputAction
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (!isActive) return;
        if (!P_UmbrellaAttackManager.canAttack) return;

        if (isCanceled)
        {
            isCanceled = false;
            return;
        }

        if (useCoolTime && (nowCoolTime != 0)) return;

        strengthBuff = playerMove.buffSystem.GetValue(G_BuffSystem.BuffType.AttackStrength);
        speedBuff = playerMove.buffSystem.GetValue(G_BuffSystem.BuffType.AttackSpeed);
        playerMove.modelAnimator.SetFloat(playerMove.attackSpeedAnimation, speedBuff);

        if (context.canceled)
        {
            if (isHold)
            {
                OnActivate();
            }
            else
            {
                if (!playerMove.GetCanAttack()) return;
                isHold = false;
                OnPressButton();
            }
            if (useCoolTime)
            {
                nowCoolTime = coolTime;
            }
        }
        else if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            isHold = true;
            OnHoldButton();
        }
    }

    /// <summary>
    /// Called when the specified button is pressed for less than the specified time.
    /// </summary>
    public virtual void OnPressButton()
    {
        onPressedButton.Invoke();
    }

    /// <summary>
    /// Called when the specified button is pressed for the specified time.
    /// </summary>
    public virtual void OnHoldButton()
    {
        onHoldButton.Invoke();
    }

    /// <summary>
    /// Called when the long press ends.
    /// You can be called at any time.(You must write "base.OnActivate();")
    /// </summary>
    public virtual void OnActivate()
    {
        isHold = false;
        onActivate.Invoke();
    }

    public bool UseEnergy(int amount)
    {
        return selector.UseEnergy(flower, amount);
    }

    protected void CancelHold()
    {
        isHold = false;
        isCanceled = true;
    }

    protected Collider GetNearestEnemy()
    {
        Vector3 pos = transform.position + (searchEnemyDadius * transform.TransformDirection(searchEnemyDirection));
        var enemies = Physics.OverlapSphere(pos, searchEnemyDadius, enemyLayer);
        if(enemies.Length == 0)
        {
            return null;
        }
        return enemies.OrderBy(enemy => Vector3.SqrMagnitude(transform.position - enemy.transform.position)).FirstOrDefault();
    }

    protected void MoveToNearestEnemy(float duration)
    {
        var nearestEnemy = GetNearestEnemy();
        if (!nearestEnemy)
        {
            return;
        }

        StartCoroutine(Move());

        IEnumerator Move()
        {
            var startPos = transform.position;
            var endPos = nearestEnemy.ClosestPoint(startPos);
            for(float f = 0f; f < 1f; f += Time.fixedDeltaTime / duration)
            {
                Vector3 velocity = Vector3.Lerp(startPos, endPos, Mathf.Sin(Mathf.PI * 0.5f * f));
                player.transform.position = velocity;
                yield return new WaitForFixedUpdate();
            }
        }
    }


    

    
}
