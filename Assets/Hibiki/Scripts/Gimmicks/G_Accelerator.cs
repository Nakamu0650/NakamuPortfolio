using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class G_Accelerator : MonoBehaviour
{
    [SerializeField] float accelerateTime = 1f;
    [SerializeField] float acceleratePower = 1f;
    [SerializeField] public List<Renderer> modelRenderer = new List<Renderer>();
    [HideInInspector] public bool isPassed;

    private static int lastID = 0;
    // Start is called before the first frame update
    void Start()
    {
        isPassed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            G_BuffSystem buff = other.gameObject.GetComponent<G_BuffSystem>();
            if (buff)
            {
                S_SEManager.PlayAcceleratorSE(other.transform);
                S_SEManager.PlayAcceleratorComboSE(other.transform);
                //G_TrackRecordManager.instance.AchiveTrackRecord("Accelerator");

                buff.DeleteBuffFromId(G_BuffSystem.BuffType.MoveSpeed, lastID);
                buff.AddBuff(G_BuffSystem.BuffType.MoveSpeed, new G_Buff("Accelerator", out lastID, acceleratePower, accelerateTime));

                transform.DORotate(new Vector3(0f, 0f, 720f), 1f,RotateMode.LocalAxisAdd).SetEase(Ease.OutQuint);
                isPassed = true;
            }
        }
    }
}
