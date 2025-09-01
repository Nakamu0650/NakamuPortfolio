using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_PotsUI : MonoBehaviour
{
    [SerializeField] protected SerializedDictionary<G_Flower.FlowerList, G_FlowerEnergyPot> energyPots;

    protected G_FlowerEnergyReceiver receiver;
    // Start is called before the first frame update
    void Start()
    {
        receiver = G_FlowerEnergyReceiver.instance;
        receiver.onGetEnergy.AddListener(OnGetEnergy);
        receiver.onUseEnergy.AddListener(OnUseEnergy);

        SetFlowers();

        Set();
    }

    public virtual void Set()
    {

    }

    public void SetFlowers()
    {
        foreach (var pot in energyPots)
        {
            pot.Value.SetFlower(receiver, pot.Key);
        }
    }

    public virtual void OnGetEnergy(G_Flower.FlowerList flower)
    {
        if (energyPots.ContainsKey(flower))
        {
            energyPots[flower].FixEnergy(receiver, flower);
        }
    }

    public virtual void OnUseEnergy(G_Flower.FlowerList flower)
    {
        if (energyPots.ContainsKey(flower))
        {
            energyPots[flower].FixEnergy(receiver, flower);
        }
    }

}
