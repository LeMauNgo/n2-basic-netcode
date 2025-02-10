using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class RoomManager : NetworkBehaviour
{
    [SerializeField] protected List<Room> rooms = new();
    private static Dictionary<ulong, Room> playerRoomMap = new Dictionary<ulong, Room>();

    public string roomNameInput = "Room_1234";
    public int maxPlayersInput = 2;

    [System.Serializable]
    public class Room
    {
        public string RoomID;
        public int MaxPlayers;
        public List<ulong> Players;

        public Room(string id, int maxPlayers)
        {
            RoomID = id;
            MaxPlayers = maxPlayers;
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

    public void CreateRoom()
    {
        CreateRoom(this.roomNameInput, this.maxPlayersInput);
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot create room. You are not connected to the server.");
            return;
        }

        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            CreateRoomOnServer(NetworkManager.Singleton.LocalClientId, roomName, maxPlayers);
        }
        else
        {
            CreateRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomName, maxPlayers);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(ulong clientId, string roomName, int maxPlayers)
    {
        CreateRoomOnServer(clientId, roomName, maxPlayers);
    }

    private void CreateRoomOnServer(ulong clientId, string roomName, int maxPlayers)
    {
        if (rooms.Exists(r => r.RoomID == roomName))
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' already exists.");
            return;
        }

        Room newRoom = new Room(roomName, maxPlayers);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        playerRoomMap[clientId] = newRoom;

        UpdateClientsRoomList();
        Debug.Log($"[{clientId}] Created room: {roomName} (Max Players: {maxPlayers})");
    }

    public void JoinRoom()
    {
        JoinRoom(this.roomNameInput);
    }

    public void JoinRoom(string roomName)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot join room. You are not connected to the server.");
            return;
        }

        if (playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }

        if (IsServer)
        {
            Room room = rooms.Find(r => r.RoomID == roomName);
            if (room == null)
            {
                Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Room '{roomName}' does not exist.");
                return;
            }

            JoinSpecificRoom(NetworkManager.Singleton.LocalClientId, roomName);
        }
        else
        {
            JoinRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomName);
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

        if (room.Players.Count >= room.MaxPlayers)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' is full.");
            return;
        }

        room.Players.Add(clientId);
        playerRoomMap[clientId] = room;
        UpdateClientsRoomList();
        Debug.Log($"[{clientId}] Joined room: {roomName} (Players: {room.Players.Count}/{room.MaxPlayers})");
    }

    public void LeaveRoom()
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Cannot leave room. You are not connected to the server.");
            return;
        }

        if (!playerRoomMap.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] You are not in any room.");
            return;
        }

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

    public void ShowRoomList()
    {
        Debug.Log("Current Rooms:");
        foreach (var room in rooms)
        {
            Debug.Log($"Room {room.RoomID} - Players: {room.Players.Count}/{room.MaxPlayers}");
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
