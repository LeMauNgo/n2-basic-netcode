using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomManager roomManager = (RoomManager)target;

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Start the game to use the buttons.", MessageType.Info);
            return;
        }

        roomManager.roomNameInput = EditorGUILayout.TextField("Room Name", roomManager.roomNameInput);

        if (GUILayout.Button("Create Room"))
        {
            roomManager.CreateRoom();
        }

        if (GUILayout.Button("Join Room"))
        {
            roomManager.JoinRoom();
        }

        if (GUILayout.Button("Leave Room"))
        {
            roomManager.LeaveRoom();
        }

        if (GUILayout.Button("Show Room List"))
        {
            roomManager.ShowRoomList();
        }
    }
}
