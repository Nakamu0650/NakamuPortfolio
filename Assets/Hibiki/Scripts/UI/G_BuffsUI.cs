using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_BuffsUI : MonoBehaviour
{
    [SerializeField] GameObject elementPrefab;
    [SerializeField] SerializedDictionary<G_BuffSystem.BuffType, Sprite> buffSprites = new SerializedDictionary<G_BuffSystem.BuffType, Sprite>();

    private Dictionary<G_Buff, RectTransform> activeBuffs;
    private G_BuffSystem system;

    private void OnValidate()
    {
        foreach(G_BuffSystem.BuffType type in System.Enum.GetValues(typeof(G_BuffSystem.BuffType)))
        {
            buffSprites.TryAdd(type, null);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        activeBuffs = new Dictionary<G_Buff, RectTransform>();
        system = G_BuffSystem.instance;
        system.onAddBuff.AddListener(OnAddBuff);
        system.onRemoveBuff.AddListener(OnRemoveBuff);
    }

    public void OnAddBuff(G_BuffSystem.BuffType type, G_Buff buff)
    {
        if(buff.buffName == "Accelerator" || buff.buffName == "BloomWindEnergyBuff")
        {
            return;
        }
        var element = GetUIComponents(Instantiate(elementPrefab, transform));
        element.uiElement.SetImage(buffSprites[type]);
        activeBuffs.Add(buff, element.rect);
    }
    public void OnRemoveBuff(G_BuffSystem.BuffType type, G_Buff buff)
    {
        if (buff.buffName == "Accelerator" || buff.buffName == "BloomWindEnergyBuff")
        {
            return;
        }
        Destroy(activeBuffs[buff].gameObject);
        activeBuffs.Remove(buff);
    }

    private (RectTransform rect, G_BuffUIElement uiElement)GetUIComponents(GameObject obj)
    {
        return (obj.GetComponent<RectTransform>(), obj.GetComponent<G_BuffUIElement>());
    }
}
