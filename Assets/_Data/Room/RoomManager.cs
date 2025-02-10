using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class RoomManager : NetworkBehaviour
{
    private static List<Room> rooms = new List<Room>();
    private static Dictionary<ulong, Room> playerRoomMap = new Dictionary<ulong, Room>();

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
            CreateRoomOnServer(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            CreateRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(ulong clientId)
    {
        CreateRoomOnServer(clientId);
    }

    private void CreateRoomOnServer(ulong clientId)
    {
        string roomId = "Room_" + Random.Range(1000, 9999);
        Room newRoom = new Room(roomId);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        playerRoomMap[clientId] = newRoom;

        Debug.Log($"[{clientId}] Created room: {roomId}");
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
            JoinAvailableRoom(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            JoinRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinRoomServerRpc(ulong clientId)
    {
        JoinAvailableRoom(clientId);
    }

    private void JoinAvailableRoom(ulong clientId)
    {
        foreach (var room in rooms)
        {
            if (room.Players.Count < 4)
            {
                room.Players.Add(clientId);
                playerRoomMap[clientId] = room;
                Debug.Log($"[{clientId}] Joined room: {room.RoomID}");
                return;
            }
        }

        Debug.LogWarning($"[{clientId}] No available room to join.");
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
            Debug.LogWarning($"[{clientId}] Not in any room (Server check).");
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
}
