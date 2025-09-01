using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SW : MonoBehaviour
{

    private bool isApperance;
    [SerializeField] GameObject cube;
    [SerializeField] Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        isApperance = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (G_SeasonManager.isChangedSeason&&G_SeasonManager.season == G_SeasonManager.Season.Winter)
        {
            isApperance=true;
            Instantiate(cube, spawnPoint, Quaternion.identity);
        }
    }
}
