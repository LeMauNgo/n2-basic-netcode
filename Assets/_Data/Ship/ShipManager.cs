using System.Collections.Generic;
using UnityEngine;

public class ShipManager : SaiBehaviour
{
    [SerializeField] protected List<ShipCtrl> shipPrefabs;

    protected override void Start()
    {
        base.Start();
        this.HideAll();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadShipPrefabs();
    }

    protected virtual void LoadShipPrefabs()
    {
        if (this.shipPrefabs.Count > 0) return;
        var ships = GetComponentsInChildren<ShipCtrl>();
        this.shipPrefabs = new List<ShipCtrl>(ships);
        Debug.LogWarning(transform.name + ": LoadShipPrefabs", gameObject);
    }

    protected virtual void HideAll()
    {
        foreach(ShipCtrl ship in this.shipPrefabs)
        {
            ship.SetActive(false);
        }
    }
}
