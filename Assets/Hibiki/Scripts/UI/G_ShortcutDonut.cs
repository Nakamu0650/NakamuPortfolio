using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class G_ShortcutDonut : MonoBehaviour
{
    [Header("Absolute")]
    [SerializeField] RectTransform selector;
    [SerializeField] RectTransform selectingHighLight, arow,options;
    [SerializeField] TMP_Text examineText;
    [SerializeField] G_ShortcutDonutPreference preference;

    [Header("Adjjustment")]
    [SerializeField] string singletonCode = "";
    [SerializeField] List<G_ShortcutDonutElement> elements = new List<G_ShortcutDonutElement>();
    [SerializeField] List<UnityEvent> elementEvents = new List<UnityEvent>();
    [SerializeField] UnityEvent openDonutEvent = new UnityEvent(),closeDonutEvent = new UnityEvent();

    //Private
    public static bool isWaiting;
    private bool textShowing;
    private bool isCallWaiting;
    private bool isSingleton;
    private bool selected;
    private static Dictionary<string, G_ShortcutDonut> instances = new Dictionary<string, G_ShortcutDonut>();
    private RectTransform thisRect;
    private Vector2 rotateAxis;
    private int elementAmount=0;
    private float widthPerPiece=1f,anglePerPiece=180f;
    private float mouseSqrThreshold, gamePadSqrThreshold;
    private int selectingNum = -1;
    private float selectingTime = 0f;
    private int selectingNumForTime;
    private CanvasGroup selectorCanvasGroup, selectingHighLightCanvasGroup;
    private List<Image> images = new List<Image>();
    private readonly Dictionary<G_ShortcutDonutPreference.ShortcutDonutAngleMode, float> modeAngle = new Dictionary<G_ShortcutDonutPreference.ShortcutDonutAngleMode, float>
    {
        {G_ShortcutDonutPreference.ShortcutDonutAngleMode.TopCentor,1f },{G_ShortcutDonutPreference.ShortcutDonutAngleMode.TopJunction,0f }
    };

    private void OnValidate()
    {
        UpdateDonut();
    }

    private void Awake()
    {
        isSingleton = (singletonCode != "");
        if (isSingleton)
        {
            CheckSingleton();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        selectorCanvasGroup = selector.GetComponent<CanvasGroup>();
        selectingHighLightCanvasGroup= selectingHighLight.GetComponent<CanvasGroup>();
        SetDonut();

        isWaiting = false;
        isCallWaiting = false;
        selected = false;
        textShowing = false;
        rotateAxis = Vector2.zero;
        thisRect.localScale = Vector3.zero;
        selectingNumForTime = -1;
        selectingTime = 0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isCallWaiting) return;


        CanvasGroupAlpha(selected);

        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = (selectingNum == i) ? preference.selectingColor : preference.normalColor;
        }
        if (!selected)
        {
            if (textShowing)
            {
                textShowing = false;
                examineText.text = "";
                examineText.DOFade(0f, preference.alphaDuration);
                selectingTime = 0f;
            }
            return;
        }

        if (selectingNumForTime != selectingNum)
        {
            selectingNumForTime = selectingNum;
            selectingTime = 0f;
            S_SEManager.PlayPlayerEquipSE2(transform);
            G_ShakeController.instance.ShakeController(0f, 0.5f, 0.025f);
            
        }
        selectingTime += Time.unscaledDeltaTime;
        bool show = selectingTime >= preference.examineShowDuration;
        if (show&&!textShowing)
        {
            examineText.text = elements[selectingNum].elementName;
            examineText.DOFade(1f, preference.alphaDuration).SetUpdate(true);
            textShowing = true;
        }
        if(!show&&textShowing)
        {
            examineText.text = "";
            examineText.DOFade(0f, preference.alphaDuration).SetUpdate(true);
            textShowing = false;
        }

        float angle = Vector2ToAngle(rotateAxis);
        selector.rotation = Quaternion.Euler(0f, 0f, (360 - angle) + (anglePerPiece / 2f));
        int num = Mathf.RoundToInt((angle - (anglePerPiece / 2f)) / anglePerPiece);
        selectingNum = num % elementAmount;
        selectingHighLight.rotation = Quaternion.Lerp(selectingHighLight.rotation, Quaternion.Euler(0f, 0f, -num * anglePerPiece), Time.unscaledDeltaTime * preference.adurationSpeed);

    }
    private void UpdateDonut()
    {
        if (preference == null||elements.Count==0) return;
        thisRect = GetComponent<RectTransform>();

        elementAmount = elements.Count;
        widthPerPiece = 1f / Mathf.Max(1f, (float)elementAmount);

        selector.GetComponent<Image>().fillAmount = widthPerPiece;
        selectingHighLight.GetComponent<Image>().fillAmount = widthPerPiece;
        anglePerPiece = 360f * widthPerPiece;
        arow.rotation = Quaternion.Euler(0f, 0f, -0.5f * anglePerPiece);
        images.Clear();
        float centerAngle = -(modeAngle[preference.angleMode] - 1);
        for (int i = 0; i < options.childCount; i++)
        {
            SetImage(options.GetChild(i).transform, 0.4f,((2*i)+centerAngle)*Mathf.PI*widthPerPiece,elements[i].texture2D, preference.imageSize);
        }
    }

    private void SetDonut()
    {
        if (preference == null || elements.Count == 0) return;
        isSingleton = (singletonCode != "");
        thisRect = GetComponent<RectTransform>();

        float sizeDelta = thisRect.rect.x;

        mouseSqrThreshold = Mathf.Pow((preference.mouseThreshold * sizeDelta / 4f), 2);
        gamePadSqrThreshold = preference.gamePadThreshold * preference.gamePadThreshold;

        elementAmount = elements.Count;
        widthPerPiece = 1f / Mathf.Max(1f, (float)elementAmount);

        anglePerPiece = 360f * widthPerPiece;
        arow.rotation = Quaternion.Euler(0f, 0f, -0.5f * anglePerPiece);
        images.Clear();
        for (int i = 0; i < options.childCount; i++)
        {
            images.Add(options.GetChild(i).GetComponent<Image>());
        }
    }

    public void OpenDonut()
    {
        if (isWaiting) return;
        selectingHighLightCanvasGroup.alpha = 0f;
        selectorCanvasGroup.alpha = 0f;

        selectingNum = -1;
        isWaiting = true;
        isCallWaiting = true;

        thisRect.localScale = Vector3.zero;
        thisRect.DOScale(1f, preference.expansionDuration).SetEase(Ease.OutSine).SetUpdate(true);
        S_SEManager.PlayMenuOpenSE(transform);
        S_SEManager.PlayMenu(true);
        openDonutEvent.Invoke();
    }
    public void CloseDonut()
    {
        if (!isCallWaiting) return;
        if (selected)
        {
            if (selectingNum + 1 <= elementEvents.Count)
            {
                elementEvents[selectingNum].Invoke();
            }
            else
            {
                Debug.LogError("Selecting index \"" + selectingNum + "\" is out of Range");
            }
        }

        isWaiting = false;
        selected = false;
        isCallWaiting = false;

        thisRect.localScale = Vector3.one;
        thisRect.DOScale(0f, preference.expansionDuration).SetEase(Ease.OutSine).SetUpdate(true);
        S_SEManager.PlayMenuCloseSE(transform);
        S_SEManager.PlayMenu(false);
        closeDonutEvent.Invoke();
    }
    public void Cancel()
    {
        if (!isCallWaiting) return;
        isWaiting = false;
        selected = false;
        isCallWaiting = false;

        thisRect.localScale = Vector3.one;
        thisRect.DOScale(0f, preference.expansionDuration).SetEase(Ease.OutSine).SetUpdate(true);
        S_SEManager.PlayMenu(false);
        closeDonutEvent.Invoke();
    }

    //PlayerInputFunctions
    public void Rotate_GamePad(InputAction.CallbackContext context)
    {
        if (!isCallWaiting) return;
        Vector2 axis = context.ReadValue<Vector2>();



        if(axis.sqrMagnitude >= gamePadSqrThreshold)
        {
            rotateAxis = axis;
            selected = true;
        }
        else if(selected)
        {
            CloseDonut();
        }
    }
    public void Rotate_Mouse(InputAction.CallbackContext context)
    {
        if (!isCallWaiting) return;

        Vector2 centor = new Vector2(Screen.width, Screen.height)/2f;
        Vector2 point = context.ReadValue<Vector2>();
        Vector2 centorPosition = point - centor;
        if (centorPosition.sqrMagnitude >= mouseSqrThreshold)
        {
            rotateAxis = centorPosition.normalized;
            selected = true;
        }
        else
        {
            selected = false;
            selectingNum = -1;
        }
    }
    public void OnTrrigerButton(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            OpenDonut();
        }
        else if (context.canceled)
        {
            Cancel();
        }
    }

    private Image SetImage(Transform _transform,float _length,float _angle,Sprite _sprite,float _imageSize)
    {
        RectTransform _rect = _transform.GetComponent<RectTransform>();
        Image _image = _transform.GetComponent<Image>();
        _rect.sizeDelta = Vector2.one * _imageSize;
        Vector2 anchor = (new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle))*_length)+(Vector2.one*0.5f);
        _rect.anchorMin = anchor-(Vector2.one*_imageSize);
        _rect.anchorMax = anchor+(Vector2.one * _imageSize);
        _image.sprite = _sprite;
        return (_image);
    }

    private void CanvasGroupAlpha(bool isSelect)
    {
        float _alpha = selectorCanvasGroup.alpha;
        _alpha = Mathf.Clamp01(_alpha + ((isSelect ? 1 : -1) * Time.unscaledDeltaTime/ preference.alphaDuration));
        selectorCanvasGroup.alpha = _alpha;
        selectingHighLightCanvasGroup.alpha = _alpha;
    }

    private void CheckSingleton()
    {
        if (instances[singletonCode] == null)
        {
            instances.Add(singletonCode, this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static float Vector2ToAngle(Vector2 vector)
    {
        float _angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        if (_angle < 0)
        {
            _angle += 360f;
        }
        return _angle;
    }
}

