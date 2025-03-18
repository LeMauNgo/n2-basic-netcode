using UnityEngine;

public class ShipCtrl : SaiBehaviour
{
    [SerializeField] protected ShipHitPoints hitPoints;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadHitPoints();
    }

    protected virtual void LoadHitPoints()
    {
        if (this.hitPoints != null) return;
        this.hitPoints = GetComponentInChildren<ShipHitPoints>();
        Debug.LogWarning(transform.name + ": LoadNetworkObject", gameObject);
    }
}
