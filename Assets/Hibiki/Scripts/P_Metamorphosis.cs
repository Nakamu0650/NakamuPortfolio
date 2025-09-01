using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class P_Metamorphosis : MonoBehaviour
{
    public static P_Metamorphosis instance;
    [SerializeField] P_UmbrellaAttackManager umbrellaAttackManager;
    [SerializeField] SkinnedMeshRenderer bodyMeshRenderer, hairMeshRenderer, faceMeshRenderer;
    [SerializeField] float openUmbrellaDuration, closeUmbrellaDuration;

    [SerializeField] GameObject umbrellaParent;
    [SerializeField] SerializedDictionary<G_Flower.FlowerList, ChangeContents> cloathObjects = new SerializedDictionary<G_Flower.FlowerList, ChangeContents>();
    [SerializeField] PoisonicMetamorphosis poisonic;
    [SerializeField] UnityEvent MetamorphosisEvent = new UnityEvent();
    [SerializeField] UnityEvent MetamorphosisPoisionEvent = new UnityEvent();
    private readonly G_Flower.FlowerList[] cloathFlowers = new G_Flower.FlowerList[5]
    {
        G_Flower.FlowerList.None, G_Flower.FlowerList.CherryBlossoms, G_Flower.FlowerList.nemophila, G_Flower.FlowerList.hibiscus, G_Flower.FlowerList.gentian
    };
    private G_BuffSystem buffSystem;
    private G_FlowerEnergyReceiver energyReceiver;
    private P_PlayerMove playerMove;
    private G_Flower.FlowerList activeFlower;

    private ChangeContents activeContents;

    private bool onChange = false;

    private void OnValidate()
    {
        foreach(var flower in cloathFlowers)
        {
            cloathObjects.TryAdd(flower, new ChangeContents());
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        buffSystem = GetComponent<G_BuffSystem>();
        energyReceiver = GetComponent<G_FlowerEnergyReceiver>();
        playerMove = P_PlayerMove.instance;
        DisactiveAllCloath();
        ChageFlowerCloath(G_Flower.FlowerList.None);
        umbrellaAttackManager.activeFlower = G_Flower.FlowerList.None;
    }

    public void OnMetamorphosisClick_CherryBlossoms(InputAction.CallbackContext context)
    {
        var type = G_Flower.FlowerList.CherryBlossoms;
        if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (energyReceiver.HaveWreath(type))
            {
                
                ChageFlowerCloath(type);                
            }
        }
    }

    public void OnMetamorphosisClick_Nemophila(InputAction.CallbackContext context)
    {
        var type = G_Flower.FlowerList.nemophila;
        if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (energyReceiver.HaveWreath(type))
            {
                
                ChageFlowerCloath(type);
            }
        }
    }

    public void OnMetamorphosisClick_Hibiscus(InputAction.CallbackContext context)
    {
        var type = G_Flower.FlowerList.hibiscus;
        if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (energyReceiver.HaveWreath(type))
            {
               
                ChageFlowerCloath(type);              
            }
        }
    }

    public void OnMetamorphosisClick_Gentian(InputAction.CallbackContext context)
    {
        var type = G_Flower.FlowerList.gentian;
        if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (energyReceiver.HaveWreath(type))
            {
                ChageFlowerCloath(type);
            }
        }
    }

    public void OnMetamorphosisClick_Poisonic(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!playerMove.GetCanAttack()) return;
            if (poisonic.reciever.UseSkill(true))
            {
                StartCoroutine(ChangePoisonicCloath());
            }
        }
    }

    public void OpenUmbrella(bool changeActiveSelf = false)
    {
        umbrellaParent.SetActive(true);
        activeContents.OpenUmbrella(openUmbrellaDuration);
    }
    public void CloseUmbrella(bool changeActiveSelf = false)
    {
        activeContents.CloseUmbrella(closeUmbrellaDuration);

        IEnumerator wait()
        {
            yield return new WaitForSeconds(closeUmbrellaDuration);
            umbrellaParent.SetActive(false);
        }
    }

    public void SetUmbrellaOpen(bool open)
    {
        activeContents.SetUmbrella(open);
    }

    public void ChangeUmbrella(bool open)
    {
        if (open)
        {
            OpenUmbrella();
        }
        else
        {
            CloseUmbrella();
        }
    }

    private void ChageFlowerCloath(G_Flower.FlowerList flowerType)
    {
        activeFlower = flowerType;
        MetamorphosisEvent.Invoke();
        ActiveCloath(cloathObjects[flowerType]);
        umbrellaAttackManager.activeFlower = flowerType;
    }

    private void ActiveCloath(ChangeContents contents)
    {
        switch (contents.timelinePlay)
        {
            case ChangeContents.MetamorPerformance.None:
                {
                    SetCloath(contents);
                    break;
                }
            case ChangeContents.MetamorPerformance.All:
                {
                   
                    StartCoroutine(ChangeWithTimeline(contents));
                    break;
                }
            case ChangeContents.MetamorPerformance.Once:
                {
                    if (contents.isShowd)
                    {
                        SetCloath(contents);
                    }
                    else
                    {
                        if (activeFlower == G_Flower.FlowerList.CherryBlossoms)
                        {
                            Debug.Log("cherry");
                            S_SEManager.PlayPlayerMetamorphosisCherryBlossamSE(transform);
                        }
                        else if (activeFlower == G_Flower.FlowerList.nemophila)
                        {
                            Debug.Log("nemophila");
                            S_SEManager.PlayPlayerMetamorphosisNemophilaSE(transform);
                        }
                        else if (activeFlower == G_Flower.FlowerList.hibiscus)
                        {
                            Debug.Log("hibiscus");
                            S_SEManager.PlayPlayerMetamorphosisHibiscusSE(transform);
                        }
                        else if (activeFlower == G_Flower.FlowerList.gentian)
                        {
                            Debug.Log("gentian");
                            S_SEManager.PlayPlayerMetamorphosisGentianSE(transform);
                        }
                        StartCoroutine(ChangeWithTimeline(contents));
                    }

                    break;
                }
        }
    }

    private void SetCloath(ChangeContents contents)
    {
        DisactiveAllCloath();
        activeContents = contents;
        bodyMeshRenderer.material = contents.bodyMaterial;
        hairMeshRenderer.material = contents.hairMaterial;
        faceMeshRenderer.material = contents.faceMaterial;

        if (contents.cloath)
        {
            contents.cloath.SetActive(true);
        }
        if (contents.umbrella)
        {
            contents.umbrella.SetActive(true);
        }

        foreach (var buff in contents.buffs)
        {
            buffSystem.AddBuff(buff.buffType, buff.buff);
        }
    }

    private void DisactiveCloath(ChangeContents contents)
    {
        if (contents.cloath)
        {
            contents.cloath.SetActive(false);
        }
        if (contents.umbrella)
        {
            contents.umbrella.SetActive(false);
        }

        foreach (var buff in contents.buffs)
        {
            buffSystem.DeleteBuffFromId(buff.buffType, buff.buff.id);
        }
    }

    private void DisactiveAllCloath()
    {
        foreach (var cloathObject in cloathObjects)
        {
            DisactiveCloath(cloathObject.Value);
        }
        DisactiveCloath(poisonic.contents);
    }

    private IEnumerator ChangePoisonicCloath()
    {
        umbrellaAttackManager.isPoisonic = true;
        ActiveCloath(poisonic.contents);
        S_SEManager.PlayPlayerMetamorphosisPoisionSE(transform);
        MetamorphosisPoisionEvent.Invoke();
        yield return new WaitForSeconds(poisonic.changeTime);

        umbrellaAttackManager.isPoisonic = false;
        DisactiveCloath(poisonic.contents);
        ChageFlowerCloath(activeFlower);
    }

    public void OnChangeCloth()
    {
        onChange = true;
    }

    private IEnumerator ChangeWithTimeline(ChangeContents contents)
    {
        Time.timeScale = 0f;
        playerMove.canJump = false;
        contents.timeline.Play();
        contents.isShowd = true;

        yield return new WaitUntil(() => onChange);

        onChange = false;

        SetCloath(contents);

        yield return new WaitUntil(() => contents.timeline.state != PlayState.Playing);
        playerMove.canJump = true;
        Time.timeScale = 1f;
    }

    [Serializable]
    private class ChangeContents
    {
        public enum MetamorPerformance { None, All, Once};

        public PlayableDirector timeline;
        public MetamorPerformance timelinePlay = MetamorPerformance.None;
        [HideInInspector] public bool isShowd = false;
        public List<Buff> buffs = new List<Buff>();
        public GameObject cloath;
        public GameObject umbrella;
        [SerializeField] G_BlendShapeChanger umbrellaBlend;
        public Material bodyMaterial, hairMaterial, faceMaterial;


        public void OpenUmbrella(float duration)
        {
            if(umbrellaBlend == null)
            {
                return;
            }

            umbrellaBlend.ChangeBlendShape("Open", duration);
        }
        public void CloseUmbrella(float duration)
        {
            if (umbrellaBlend == null)
            {
                return;
            }

            umbrellaBlend.ChangeBlendShape("Close", duration);
        }

        public void SetUmbrella(bool open)
        {
            if (umbrellaBlend == null)
            {
                return;
            }
            umbrellaBlend.SetBlendShape(open ? "Open" : "Close");
        }

        [Serializable]
        public class Buff
        {
            public G_BuffSystem.BuffType buffType;
            public G_Buff buff;
        }
    }

    [Serializable]
    private class PoisonicMetamorphosis
    {
        public G_PoisonicEnergyReciever reciever;
        public ChangeContents contents;
        public float changeTime;
    }

}
