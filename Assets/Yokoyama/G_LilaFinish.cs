using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class G_LilaFinish : MonoBehaviour
{
    public GameObject witch;
    private PlayableDirector playableDirector;
    private F_HP playerHP;
    [SerializeField] GameObject player;
    P_PlayerMove playerMove;
    [SerializeField]Transform playerPos;
    [SerializeField]UnityEvent lilaDeathEvent = new UnityEvent();
    
    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playerHP = player.GetComponent<F_HP>();
        playerMove = player.GetComponent<P_PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FInishWitch()
    {
        player.transform.position = playerPos.position;
        playerHP.isInvincible = true;
        StartCoroutine(PlayTimeLine());
        
    }

    public IEnumerator PlayTimeLine()
    {
        Time.timeScale = 0f;
        
        playerMove.isMove = false;
        
        playableDirector.Play();
        for (float f = 0f; f < playableDirector.duration-2.0f; f += Time.unscaledDeltaTime)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        playerMove.isMove = true;
        lilaDeathEvent.Invoke();
        witch.SetActive(false);
        playerHP.isInvincible = false;
    }
}
