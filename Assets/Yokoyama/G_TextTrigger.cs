using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_TextTrigger : MonoBehaviour
{
    G_TutorialManager tutorialManager;
    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = G_TutorialManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(enumerator());
        }

        IEnumerator enumerator()
        {
            yield return new WaitForSeconds(0.5f);
            if (G_SeasonManager.season == G_SeasonManager.Season.Spring)
            {
                
            }
            
        }
    }
}
