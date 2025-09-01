using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class G_WarpCotton : MonoBehaviour
{
    private Vector3 playerLocalPosition;
    private P_PlayerMove playerMove;
    private Transform player;
    private bool isPlaying;
    private bool playerIn;
    [SerializeField] SplineAnimate cottonSplineAnimate;
    [SerializeField] Transform cottonTransform;
    [SerializeField] Transform cottonModelTransform;
    [SerializeField] GameObject cameraObject;
    [SerializeField] SplineContainer spline;
    [Header("変更可能な値")]
    [SerializeField] float playerApproachSeconds = 0.5f;
    [SerializeField] float cottonRisingSeconds = 2f;
    [SerializeField] float cameraRemoveSeconds = 1;
    // Start is called before the first frame update
    void Start()
    {
        cottonSplineAnimate.enabled = false;
        cottonTransform.localPosition = Vector3.zero;
        isPlaying = false;
        playerIn = false;
    }

    private void Update()
    {
        if (playerIn&& Input.GetKeyDown(KeyCode.F) && !isPlaying)
        {
            if (!playerMove)
            {
                playerMove = player.GetComponent<P_PlayerMove>();
            }
            playerMove.canMove = false;
            cameraObject.SetActive(true);
            StartCoroutine(RideCotton());
        }
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            player.transform.position = cottonTransform.position + playerLocalPosition;
            cottonModelTransform.position = cottonTransform.position;
        }
    }

    public void OnExamine(Transform _player)
    {
        if (!isPlaying)
        {
            if (!playerMove)
            {
                playerMove = _player.GetComponent<P_PlayerMove>();
                player = _player;
            }
            playerMove.canMove = false;
            cameraObject.SetActive(true);
            StartCoroutine(RideCotton());
        }
    }

    IEnumerator RideCotton()
    {
        player.DOMove(new Vector3(cottonTransform.position.x, player.transform.position.y, cottonTransform.position.z), playerApproachSeconds);
        yield return new WaitForSeconds(playerApproachSeconds);
        playerLocalPosition = player.transform.position - cottonTransform.position;
        isPlaying = true;
        cottonTransform.DOLocalMove(spline.Spline.EvaluatePosition(0f), cottonRisingSeconds).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(cottonRisingSeconds);

        cottonSplineAnimate.enabled = true;
        yield return null;
        cottonSplineAnimate.Play();

        yield return new WaitUntil(() => ((cottonSplineAnimate.NormalizedTime==1f)||Input.GetKey(KeyCode.F)||playerMove.isFlying));

        playerMove.canMove = true;
        playerMove.rb.velocity = Vector3.zero;
        isPlaying = false;
        cameraObject.SetActive(false);

        cottonSplineAnimate.Pause();
        cottonSplineAnimate.enabled = false;
        
        cottonModelTransform.DOLocalMoveY(20f, cameraRemoveSeconds).SetEase(Ease.InSine);
        cottonModelTransform.DOScale(Vector3.zero, cameraRemoveSeconds);
        yield return new WaitForSeconds(cameraRemoveSeconds);

        cottonSplineAnimate.NormalizedTime = 0f;
        cottonTransform.localPosition = Vector3.zero;
        cottonTransform.localScale = Vector3.one;
        cottonModelTransform.localPosition = Vector3.zero;
        cottonModelTransform.localScale = Vector3.one;
    }
}
