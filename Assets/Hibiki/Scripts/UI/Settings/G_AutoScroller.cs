using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_AutoScroller : MonoBehaviour
{
    [SerializeField] RectTransform content;

    [SerializeField] float scrollSpeed = 500f;

    private GameManager gameManager;
    private Transform contentChild;

    private RectTransform contentChildRect;
    private RectTransform thisRect;

    private Vector2 contentDefaultPosition = Vector2.zero;

    // Start is called before the first frame update
    private void Start()
    {
        contentDefaultPosition = content.anchoredPosition;
        thisRect = GetComponent<RectTransform>();
    }

    //Initialize the location when it is reopened.
    private void OnEnable()
    {
        if(contentDefaultPosition != Vector2.zero)
        {
            content.anchoredPosition = contentDefaultPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Execute only when scrolling is required
        float height = thisRect.rect.height;
        float contentHeight = content.rect.height;
        if(contentHeight <= height) { return; }

        //Not to be executed when not a child of its own.
        Transform newContentChild = GetContentChild(GetGameManager().GetSelected().transform);
        if (!newContentChild) { return; }

        //New GetComponent when the selected object is updated.
        if (contentChild != newContentChild)
        {
            contentChild = newContentChild;
            contentChildRect = newContentChild.GetComponent<RectTransform>();
        }

        float childPositionY = contentChildRect.anchoredPosition.y;
        float childHalfHeight = contentChildRect.rect.height / 2f;
        float contentPositionY = content.anchoredPosition.y;

        float maxPosition = -contentPositionY;
        float minPosition = -contentPositionY - height;

        if(minPosition > (childPositionY - childHalfHeight))
        {
            //Selected selectable is too high.
            content.anchoredPosition += Vector2.up * Time.unscaledDeltaTime * scrollSpeed;
        }
        else if((childPositionY + childHalfHeight) > maxPosition)
        {
            //Selected selectable is too low.
            content.anchoredPosition -= Vector2.up * Time.unscaledDeltaTime * scrollSpeed;
        }
    }

    private GameManager GetGameManager()
    {
        if (!gameManager)
        {
            gameManager = GameManager.instance;
            if (!gameManager)
            {
                Debug.LogError("GameManager does not exsist", this);
                return null;
            }
        }
        return gameManager;
    }

    /// <summary>
    /// Returns the child object of the content from the parent-child relationship of the descendant. If it is not its own child, null is returned.
    /// </summary>
    /// <param name="grandchild">Assigns an object to be a descendant</param>
    /// <returns></returns>
    private Transform GetContentChild(Transform grandchild)
    {

        if (!grandchild.IsChildOf(content)) { return null; }

        Transform parent = grandchild.parent;
        while (true)
        {
            if(content == parent.parent)
            {
                return parent;
            }
            else
            {
                parent = parent.parent;
            }
        }
    }


}
