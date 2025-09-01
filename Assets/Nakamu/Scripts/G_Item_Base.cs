using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class G_Item_Base : MonoBehaviour
{
    [SerializeField] protected G_ItemParameters itemParam;
    [SerializeField] protected G_Flower.FlowerList flower;
    [SerializeField] protected InputAction modifire;
    [SerializeField] protected UnityEvent onPressButton;

    protected G_FlowerEnergyReceiver receiver;
    [SerializeField] protected float coolTime;
    protected bool useCoolTime;
    protected float nowCoolTime;
    protected bool isModifiring;

    public void Start()
    {
        isModifiring = false;
        coolTime = (float)itemParam.coolTime;
        useCoolTime = (coolTime != 0f);
        nowCoolTime = 0f;
        receiver = G_FlowerEnergyReceiver.instance;

        modifire.started += OnModifire;
        modifire.canceled += OnModifire;
        modifire.Enable();
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

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (isModifiring) return;

        if (useCoolTime && (nowCoolTime != 0)) return;

        if (context.performed)
        {
            OnPressButton();
            
            if (useCoolTime)
            {
                nowCoolTime = coolTime;
            }
        }
    }

    private void OnModifire(InputAction.CallbackContext context)
    {
        if (isModifiring)
        {
            isModifiring = !context.canceled;
        }
        else
        {
            isModifiring = context.started;
        }
    }


    public virtual void OnPressButton()
    {
        onPressButton.Invoke();
    }
    
    //About UseEnergy Method
    public bool UseEnergy(int amount)
    {
        return receiver.UseEnergy(flower, amount);
    }
}
