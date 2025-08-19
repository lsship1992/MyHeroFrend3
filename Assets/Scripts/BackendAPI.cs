using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BackendAPI : MonoBehaviour
{
    private const string API_URL = "https://your-server.com/api"; // ������ �� ���� URL

    // �������� ������ � �����
    public static IEnumerator SendWaveData(int wave, string playerId)
    {
        WWWForm form = new WWWForm();
        form.AddField("wave", wave);
        form.AddField("player_id", playerId);

        using (UnityWebRequest request = UnityWebRequest.Post(API_URL + "/wave", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                Debug.Log($"Wave {wave} data sent!");
            }
        }
    }

    // ��������� ����������
    public static IEnumerator FetchLeaderboard(Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL + "/leaderboard"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }
}