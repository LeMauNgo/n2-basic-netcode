using System.Collections.Generic;
using UnityEngine;

public class ShipHitPoints : SaiBehaviour
{
    [SerializeField] protected List<ShipHitPoint> hitPoints;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadHitPoints();
    }

    protected virtual void LoadHitPoints()
    {
        if (this.hitPoints.Count > 0) return;
        var foundHitPoints = transform.GetComponentsInChildren<ShipHitPoint>();
        this.hitPoints = new List<ShipHitPoint>(foundHitPoints);
        Debug.LogWarning(transform.name + ": LoadNetworkObject", gameObject);
    }

    public virtual bool IsDead()
    {
        foreach (ShipHitPoint hitPoint in this.hitPoints)
        {
            if (!hitPoint.IsHit) return false;
        }
        return true;
    }
}
