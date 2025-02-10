using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public class RoomManager : NetworkBehaviour
{
    [SerializeField] protected List<Room> rooms = new List<Room>();
    private static Dictionary<ulong, Room> playerRoomMap = new Dictionary<ulong, Room>();

    public string roomNameInput = "Room_1234";

    [System.Serializable]
    public class Room
    {
        public string RoomID;
        public List<ulong> Players;

        public Room(string id)
        {
            RoomID = id;
            Players = new List<ulong>();
        }
    }

    [System.Serializable]
    private class RoomListWrapper
    {
        public List<Room> Rooms;

        public RoomListWrapper(List<Room> rooms)
        {
            Rooms = rooms;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RequestRoomDataServerRpc();
        }
    }

    [ContextMenu("Create Room")]
    public void CreateRoom()
    {
        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            CreateRoomOnServer(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }
        else
        {
            CreateRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(ulong clientId, string roomName)
    {
        CreateRoomOnServer(clientId, roomName);
    }

    private void CreateRoomOnServer(ulong clientId, string roomName)
    {
        if (rooms.Exists(r => r.RoomID == roomName))
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' already exists.");
            return;
        }

        Room newRoom = new Room(roomName);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        playerRoomMap[clientId] = newRoom;

        UpdateClientsRoomList();
        Debug.Log($"[{clientId}] Created room: {roomName}");
    }

    [ContextMenu("Join Room")]
    public void JoinRoom()
    {
        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            JoinSpecificRoom(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }
        else
        {
            JoinRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinRoomServerRpc(ulong clientId, string roomName)
    {
        JoinSpecificRoom(clientId, roomName);
    }

    private void JoinSpecificRoom(ulong clientId, string roomName)
    {
        Room room = rooms.Find(r => r.RoomID == roomName);
        if (room == null)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' not found.");
            return;
        }

        room.Players.Add(clientId);
        playerRoomMap[clientId] = room;
        UpdateClientsRoomList();
        Debug.Log($"[{clientId}] Joined room: {roomName}");
    }

    [ContextMenu("Leave Room")]
    public void LeaveRoom()
    {
        if (IsServer)
        {
            RemovePlayerFromRoom(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            LeaveRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveRoomServerRpc(ulong clientId)
    {
        RemovePlayerFromRoom(clientId);
    }

    private void RemovePlayerFromRoom(ulong clientId)
    {
        if (!playerRoomMap.TryGetValue(clientId, out Room room))
        {
            Debug.LogWarning($"[{clientId}] Not in any room.");
            return;
        }

        room.Players.Remove(clientId);
        playerRoomMap.Remove(clientId);
        Debug.Log($"[{clientId}] Left room: {room.RoomID}");

        if (room.Players.Count == 0)
        {
            Debug.Log($"Room {room.RoomID} is now empty and will be removed.");
            rooms.Remove(room);
        }

        UpdateClientsRoomList();
    }

    [ContextMenu("Show Room List")]
    public void ShowRoomList()
    {
        Debug.Log("Current Rooms:");
        foreach (var room in rooms)
        {
            Debug.Log($"Room {room.RoomID} - Players: {string.Join(", ", room.Players)}");
        }
    }

    private void UpdateClientsRoomList()
    {
        string json = JsonUtility.ToJson(new RoomListWrapper(rooms));
        SendRoomDataClientRpc(json);
    }

    [ClientRpc]
    private void SendRoomDataClientRpc(string json)
    {
        RoomListWrapper wrapper = JsonUtility.FromJson<RoomListWrapper>(json);
        rooms = wrapper.Rooms;
        Debug.Log("Updated room list from server.");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRoomDataServerRpc(ServerRpcParams rpcParams = default)
    {
        UpdateClientsRoomList();
    }
}
