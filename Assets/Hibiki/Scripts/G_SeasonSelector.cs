using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G_SeasonSelector : MonoBehaviour
{
    private G_SeasonManager seasonManager;
    [SerializeField] G_SkyController skyController;
    [SerializeField] G_SeasonManager.Season changeSeason;
    [SerializeField] UnityEvent intoIsland;
    [SerializeField] bool onlyOnece = false;

    private static P_PlayerMove playerMove;

    private bool isChanged;

    void Start()
    {
        seasonManager = G_SeasonManager.instance;
        isChanged = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(onlyOnece && isChanged)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (!playerMove)
            {
                playerMove = other.gameObject.GetComponent<P_PlayerMove>();
            }
            isChanged = true;
            StartCoroutine(enumerator());
        }

        IEnumerator enumerator()
        {
            yield return new WaitUntil(() => !playerMove.isJumping);
            yield return new WaitForSeconds(0.5f);
            if ((G_SeasonManager.season != changeSeason) && (G_SeasonManager.season != G_SeasonManager.Season.Winter))
            {
                seasonManager.SetSeasonSeemless(changeSeason,true);
                skyController.ChangeSeason(G_SeasonManager.SeasonToInt(changeSeason));
            }
            intoIsland.Invoke();
        }
    }
}
