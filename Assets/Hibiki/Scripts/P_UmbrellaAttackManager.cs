using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class P_UmbrellaAttackManager : MonoBehaviour
{
    [SerializeField] GameObject umbrellaObject;
    [SerializeField] P_PlayerMove playerMove;
    [SerializeField] F_Param paramator;
    [SerializeField] F_AttackValue attackValue;
    [SerializeField] SerializedDictionary<G_Flower.FlowerList, G_UmbrellaHoldAttack> holdAttacks;
    [SerializeField] G_UmbrellaHoldAttack poisonicHoldAttack;
    [SerializeField] string umbrellaAttackAnimation;
    public List<G_Flower.FlowerList> useFlowerLists = new List<G_Flower.FlowerList>();
    public G_Flower.FlowerList activeFlower = G_Flower.FlowerList.None;
    public bool isPoisonic;

    public Events events;

    public static bool canAttack;

    private Collider umbrellaCollider;
    private P_Metamorphosis metamorphosis;

    private bool isActive;
    private bool isHold;

    protected float speedBuff;
    protected float strengthBuff;

    private void OnValidate()
    {
        if (holdAttacks != null)
        {
            useFlowerLists = new List<G_Flower.FlowerList>();
            useFlowerLists.Add(G_Flower.FlowerList.None);
            foreach(var flower in holdAttacks)
            {
                useFlowerLists.Add(flower.Key);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        isHold = false;
        canAttack = true;
        isPoisonic = false;

        umbrellaObject.SetActive(false);

        umbrellaCollider = umbrellaObject.GetComponent<Collider>();
        metamorphosis = P_Metamorphosis.instance;
    }

    public void OnAttack(Collider collider)
    {
        collider.GetComponent<F_HP>().OnDamage(Mathf.FloorToInt(paramator.GetATK() * strengthBuff), attackValue.attackValues[0], Vector3.zero);
        S_SEManager.PlayPlayerUmbrellaAttackHitSE(transform);
    }

    public void UmbrellaAttackState(bool activeSelf)
    {
        umbrellaObject.SetActive(activeSelf);
        isActive = activeSelf;
        if (activeSelf)
        {
            metamorphosis.SetUmbrellaOpen(false);
            playerMove.modelAnimator.SetTrigger(umbrellaAttackAnimation);
        }
    }
    public void UmbrellaAttackTrigger(bool enabled)
    {
        umbrellaCollider.enabled = enabled;
    }

    public void OnAttackButton(InputAction.CallbackContext context)
    {
        if (isActive) return;
        if (!canAttack) return;

        strengthBuff = playerMove.buffSystem.GetValue(G_BuffSystem.BuffType.AttackStrength);
        speedBuff = playerMove.buffSystem.GetValue(G_BuffSystem.BuffType.AttackSpeed);
        playerMove.modelAnimator.SetFloat(playerMove.attackSpeedAnimation, speedBuff);


        if (context.canceled)
        {
            if (isHold)
            {
                isHold = false;
                events.onHoldCancel.Invoke();

                if (isPoisonic)
                {
                    poisonicHoldAttack.OnAttackEnd();
                }
                else
                {
                    holdAttacks[activeFlower].OnAttackEnd();
                }

                return;
            }

            events.onAttack.Invoke();
            UmbrellaAttackState(true);
        }
        else if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (!isPoisonic && !CanHoldAttack())
            {
                return;
            }
            isHold = true;
            events.onHoldAttack.Invoke();

            if (isPoisonic)
            {
                poisonicHoldAttack.OnAttack();
            }
            else
            {
                holdAttacks[activeFlower].OnAttack();
            }

            //ActiveMetamorphosisAttackHold
        }
    }

    /// <summary>
    /// 花エネルギーが未選択じゃないかつ消費可能である時にtrueを返す
    /// </summary>
    /// <returns></returns>
    private bool CanHoldAttack()
    {
        if (activeFlower == G_Flower.FlowerList.None)
        {
            print("activeFlowerがNone");
            return false;
        }
        if (!useFlowerLists.Contains(activeFlower))
        {
            print($"リストに{activeFlower.ToString()}が含まれていない");
            return false;
        }
        if (!holdAttacks[activeFlower].CanUseEnergy())
        {
            print($"{activeFlower.ToString()}のエネルギーが足りない");
            return false;
        }
        return true;
    }

    [Serializable]
    public class Events
    {
        public UnityEvent onAttack, onHoldAttack, onHoldCancel;
    }
}
