using Unity.Netcode;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (GUILayout.Button("Start Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUILayout.Button("Start Client"))
        {
            NetworkManager.Singleton.StartClient();
        }

        if (GUILayout.Button("Start Server"))
        {
            NetworkManager.Singleton.StartServer();
        }

        GUILayout.EndArea();
    }
}