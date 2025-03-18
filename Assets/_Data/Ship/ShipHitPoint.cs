using UnityEngine;

public class ShipHitPoint : SaiBehaviour
{
    [SerializeField] protected bool isHit = false;
    public bool IsHit => isHit;

    public virtual void Hit()
    {
        this.isHit = true;
    }
}
