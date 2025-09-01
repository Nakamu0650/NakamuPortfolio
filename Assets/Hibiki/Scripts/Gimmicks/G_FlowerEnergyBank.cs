using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(G_ExaminableGimmick))]
public class G_FlowerEnergyBank : MonoBehaviour
{
    //Absolute Methods to select in unity.
    [Header("Absolute")]
    [SerializeField] GameObject examiningCamera;
    [SerializeField] CanvasGroup[] hideCanvases;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] string[] disableActionMapsName, enableActionMapsName;

    //Variable Methods
    [Header("Variable")]
    [SerializeField] float canvasFadeSpeed = 0.25f;

    // FlowerEnergies which are deposited with hugeTree are managed this method.
    [HideInInspector] public Dictionary<G_Flower.FlowerList, int> energyAmounts = new Dictionary<G_Flower.FlowerList, int>();

    private bool cancelButtonOnClick = false;
    private bool isShowing = false;


    // Awake is used to initialize any variables or game state before the game starts.
    void Awake()
    {
        InitiallizeFlowerEnergies();
        cancelButtonOnClick = false;
        isShowing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        examiningCamera.SetActive(false);

        playerInput.actions.FindActionMap("Cheat", true).Enable();
    }

    // To initialize flowerEnergies which are deposited with hugeTree.
    private void InitiallizeFlowerEnergies()
    {
        energyAmounts = new Dictionary<G_Flower.FlowerList, int>();
        foreach (G_Flower.FlowerList flower in Enum.GetValues(typeof(G_Flower.FlowerList)))
        {
            energyAmounts.Add(flower, 0);
        }
    }

    //Call when examine button onClick in collider.
    public void OnGimmick()
    {
        StartCoroutine(enumerator());

        IEnumerator enumerator()
        {
            //Start
            GimmickStart();

            //Update
            while (!cancelButtonOnClick)
            {
                yield return null;
            }

            //Escape
            GimmickEscape();
        }

        void GimmickStart()
        {
            examiningCamera.SetActive(true);
            isShowing = true;
            G_SeasonManager.seasonTimeScale = 0f;
            foreach (var canvas in hideCanvases)
            {
                canvas.DOFade(0f, canvasFadeSpeed);
            }
            foreach (string mapName in enableActionMapsName)
            {
                playerInput.actions.FindActionMap(mapName, true).Enable();
            }
            foreach (string mapName in disableActionMapsName)
            {
                playerInput.actions.FindActionMap(mapName, true).Disable();
            }
        }

        void GimmickEscape()
        {
            examiningCamera.SetActive(false);
            isShowing = false;
            G_SeasonManager.seasonTimeScale = 1f;
            cancelButtonOnClick = false;
            foreach (var canvas in hideCanvases)
            {
                canvas.DOFade(1f, canvasFadeSpeed);
            }
            foreach (string mapName in enableActionMapsName)
            {
                playerInput.actions.FindActionMap(mapName, true).Disable();
            }
            foreach (string mapName in disableActionMapsName)
            {
                playerInput.actions.FindActionMap(mapName, true).Enable();
            }
        }
    }

    //PlayerInput Functions
    public void OnCancelButton(InputAction.CallbackContext context)
    {
        if (!isShowing) return;
        cancelButtonOnClick = context.performed;
    }
}
