using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkEventManager : MonoBehaviour
{
    // Custom events
    public UnityEvent OnHostStarted;       // Triggered when Host starts successfully
    public UnityEvent OnServerStarted;     // Triggered when Server starts successfully
    public UnityEvent OnClientConnected;   // Triggered when Client connects successfully
    public UnityEvent OnClientJoined;      // Triggered when a new Client joins the Server

    private void Start()
    {
        // Subscribe to NetworkManager events
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when the object is destroyed
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    // Handle Server started event
    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host started successfully!");
            OnHostStarted?.Invoke(); // Trigger Host success event
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server started successfully!");
            OnServerStarted?.Invoke(); // Trigger Server success event
        }
    }

    // Handle Client connected event
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Successfully connected to the server!");
            OnClientConnected?.Invoke(); // Trigger Client connection success event
        }
        else
        {
            Debug.Log($"New client joined with ID: {clientId}");
            OnClientJoined?.Invoke(); // Trigger new Client joined event
        }
    }
}