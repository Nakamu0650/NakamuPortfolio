using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class P_DamageManager : MonoBehaviour
{
    [HideInInspector]
    public int _currentDamage;
    private readonly int _maxDamage = 1000;

    /// <summary>
    /// About Current Damage Range Deal
    /// </summary>
    public int CurrentDamage
    {

        get => _currentDamage;
        set
        {
            if (value < 0)
                _currentDamage = 0;
            else if (value > _maxDamage)
                _currentDamage = _maxDamage;
            else
                _currentDamage = value;
        }
    }

    ///<summary>
    ///About Damage Calculation Deal
    ///(Atack + strength - defense) * Random(min 0.9, max 0.9) = _damage(Round down)
    ///</summary>
    ///<param name="_atk">ATTACK</param>
    ///<param name="_str">STRENGTH</param>
    ///<param name="_def">DEFENSE</param>
    public int DamageCalculation(int _atk, int _str, int _def)
    {
        var _random = Random.Range(0.9f, 1.0f);
        float _damage = (_atk + _str - _def) * _random;
        _currentDamage = (int)Math.Truncate(_damage);

        return _currentDamage;
    }
}
