using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, 0, moveZ));
    }
}