using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



public class AuthService : MonoBehaviour
{
    public IEnumerator Login(string email, string password, System.Action<bool> callback)
    {
        var loginData = new LoginRequest
        {
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(loginData);

        using var request = new UnityWebRequest(ApiClient.BaseUrl + "/auth/login", "POST");

        byte[] body = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            ApiClient.Token = response.token;

            Debug.Log("Login Successful");
            Debug.Log("Token" + ApiClient.Token);

            callback?.Invoke(true);

        }
        else
        {
            Debug.LogError("Login failed: " + request.responseCode + " " + request.downloadHandler.text);
        }

        callback?.Invoke(true);




    }

    public IEnumerator Register(string username, string email, string password, System.Action<bool> callback)
    {
        var registerData = new RegisterRequest
        {
            username = username,
            email = email,
            password = password

        };

        string json = JsonUtility.ToJson(registerData);

        using var request = new UnityWebRequest(ApiClient.BaseUrl + "/auth/register","POST");

        byte[] body = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        //Debug Pentru adaugarea unei noi inregistrari in DB
        Debug.Log("HTTP status: " + request.responseCode);
        Debug.Log("Response body: " + request.downloadHandler.text);
        Debug.Log("Result: " + request.result);

        if (request.responseCode >= 200 && request.responseCode < 300)
        {
            Debug.Log("Register Successful");
            callback?.Invoke(true);
        }
        else
        {
            Debug.Log("Register Failed: " + request.responseCode + " " + request.downloadHandler.text);
        }
        callback?.Invoke(false);

    }
}