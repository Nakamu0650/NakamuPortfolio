using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G_SettingsHightLayout : MonoBehaviour
{
    void OnValidate()
    {
        Set();
    }

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    private void Set()
    {
        float space = GetComponent<VerticalLayoutGroup>().spacing;
        float height = 0f;
        foreach(Transform child in transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            height += rect.rect.height;
            height += space;
        }
        height -= space;
        var thisRect = GetComponent<RectTransform>();
        thisRect.sizeDelta = new Vector2(thisRect.sizeDelta.x, height);

        List<Selectable> selectables = new List<Selectable>();
        foreach (Transform child in transform)
        {
            var select = child.GetComponent<G_Settings>().selectable;
            selectables.Add(select);
        }

        int length = selectables.Count;
        if(length > 1)
        {
            for (int i = 0; i < length; i++)
            {
                var navigation = new Navigation { mode = Navigation.Mode.Explicit };

                if (i == 0)
                {
                    navigation.selectOnDown = selectables[1];
                }
                else if (i == (length - 1))
                {
                    navigation.selectOnUp = selectables[i - 1];
                }
                else
                {
                    navigation.selectOnUp = selectables[i - 1];
                    navigation.selectOnDown = selectables[i + 1];
                }

                selectables[i].navigation = navigation;
            }
        }
        else if(length == 1)
        {
            selectables[0].navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }
}
