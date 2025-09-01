using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G_BuffUIElement : MonoBehaviour
{
    [SerializeField] Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
