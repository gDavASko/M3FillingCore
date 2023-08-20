using System.ComponentModel;
using UnityEngine;

public interface ICover: IPoolable<ICover>
{
    void DealDamage();
}