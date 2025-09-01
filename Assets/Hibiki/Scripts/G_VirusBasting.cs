using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G_VirusBasting : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] G_SeasonManager seasonManager;

    public void OnButtonDown()
    {
        Time.timeScale = 0f;
    }
    public void OnButtonUp()
    {
        Time.timeScale = 1f;
    }
    public void Reboot()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void WarpToStart()
    {
        P_PlayerMove.instance.ReturnToSafeGroundPoint();
    }
    public void ChangeNextSeason()
    {
        seasonManager.GoNextSeason();
    }
}
