using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class G_SeasonClock : MonoBehaviour
{
    [SerializeField] RectTransform clockHand;
    [SerializeField] ChangeSprite changeSeason;
    [SerializeField] float changeSeasonDuration = 1f;
    public bool useClockHand;
    private RectTransform rect;
    private G_SeasonManager seasonManager;

    [Serializable]
    private class ChangeSprite
    {
        public RectTransform clockBackParent;
        public Image[] clockBackImages = new Image[2];
        public Sprite[] seasonSprites = new Sprite[4];
        [HideInInspector] public float parentMoveDistance;
        public int beforeSeasonNumber = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        seasonManager = G_SeasonManager.instance;
        changeSeason.beforeSeasonNumber = G_SeasonManager.seasonNumber;
        changeSeason.parentMoveDistance = changeSeason.clockBackParent.rect.height;
        SetSeason(-1, changeSeason.beforeSeasonNumber);
        changeSeason.clockBackParent.anchoredPosition = new Vector2(0f, changeSeason.parentMoveDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (!useClockHand) return;
        RotateClockHand(G_SeasonManager.seasonTime / seasonManager.seasonLengthes[G_SeasonManager.seasonNumber]);
    }

    public void OnChangeSeason()
    {
        int afterSeason = G_SeasonManager.seasonNumber;
        SetSeason(changeSeason.beforeSeasonNumber, afterSeason);
        changeSeason.clockBackParent.anchoredPosition = Vector2.zero;
        changeSeason.clockBackParent.DOAnchorPosY(changeSeason.parentMoveDistance, changeSeasonDuration).SetEase(Ease.OutBounce);
        changeSeason.beforeSeasonNumber = afterSeason;
    }

    public void SetSeason(int beforeNum, int afterNum)
    {
        changeSeason.clockBackImages[0].sprite = (beforeNum != -1) ? changeSeason.seasonSprites[beforeNum] : null;
        changeSeason.clockBackImages[1].sprite = (afterNum != -1) ? changeSeason.seasonSprites[afterNum] : null;
    }

    public void RotateClockHand(float clamp)
    {
        clockHand.eulerAngles = new Vector3(0, 0, -360f * clamp);
    }

}
