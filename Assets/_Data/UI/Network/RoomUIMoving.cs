using UnityEngine;

public class RoomUIMoving : UIMoving
{
    protected override void LoadPointA()
    {
        if (this.start != Vector3.zero) return;
        this.start = new Vector3(-749, 262, 0);
        Debug.LogWarning(transform.name + ": LoadPointA", gameObject);
    }

    protected override void LoadPointB()
    {
        if (this.end != Vector3.zero) return;
        this.end = new Vector3(-450, 262, 0);
        Debug.LogWarning(transform.name + ": LoadPointB", gameObject);
    }
}
