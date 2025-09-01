using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class F_Enemy : MonoBehaviour
{
    protected F_HP hp;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected F_AttackValue attackValue;
    [SerializeField] protected int energyAmount;

    // Start is called before the first frame update
    protected void Start()
    {
        SetHPEvent();
    }

    public virtual void OnDamaged()
    {
        P_CameraMove_Alpha.instance.SetLastAttackEnemy(transform);
        P_CameraMove_Alpha.instance.AddAngryEnemy(transform);
    }

    public virtual void OnKilled()
    {
        S_SEManager.PlayEnemyDeatheSE(transform);

        var receiver = G_PoisonicEnergyReciever.instance;
        receiver.GetEnergy(energyAmount);


        P_CameraMove_Alpha.instance.DeleteLastAttackEnemy();

        P_CameraMove_Alpha.instance.RemoveAngryEnemy(transform);
    }

    private void SetHPEvent()
    {
        hp = GetHP();
        hp.damagedEvents.AddListener(OnDamaged);
        hp.killedEvents.AddListener(OnKilled);
    }

    protected F_HP GetHP()
    {
        if (hp == null)
        {
            hp = GetComponent<F_HP>();
        }
        return hp;
    }

    [Serializable]
    public class FuzzyGrade
    {
        public string name;
        public List<AnimationCurve> fuzzyCurves;

        public float GetEvaluate(params float[] values)
        {
            int i = 0;
            int count = Mathf.Min(fuzzyCurves.Count, values.Length);
            float v = 1f;
            foreach(var fuzzy in fuzzyCurves)
            {
                if (i == count)
                {
                    break;
                }
                v *= fuzzy.Evaluate(values[i]);
                i++;
            }

            return v;
        }

        public static FuzzyGrade[] FuzziesToArray(params FuzzyGrade[] fuzzies)
        {
            return fuzzies;
        }

        public static float[] ParamatersToArray(params float[] paramaters)
        {
            return paramaters;
        }
    }
}
