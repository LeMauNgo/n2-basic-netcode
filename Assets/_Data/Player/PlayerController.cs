using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 moveInput;

    void Update()
    {
        if (!IsOwner) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ);

        SendMovementToServerServerRpc(moveInput);
    }

    [ServerRpc]
    private void SendMovementToServerServerRpc(Vector3 moveDirection, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out var client))
        {
            if (client.PlayerObject.TryGetComponent(out PlayerController player))
            {
                player.transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
        }
    }
}
