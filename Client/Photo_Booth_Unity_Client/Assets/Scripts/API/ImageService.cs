using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class DecryptRequest
{
    public string encryptionPassword;
}

public class ImageService : MonoBehaviour
{
    public IEnumerator GetImages(
    System.Action<ImageFileDTO[]> callback)
    {
        string url = ApiClient.BaseUrl + "/image";

        using var request = UnityWebRequest.Get(url);

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("GET Images Status: " + request.responseCode);
        Debug.Log("GET Images Response: " + request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            var images = JsonConvert.DeserializeObject<ImageFileDTO[]>(request.downloadHandler.text);
            Debug.Log("Parsed images: " + (images == null ? "NULL" : images.Length.ToString()));
            callback?.Invoke(images);
        }
        
        //Debug Failed to Get Images
        else
        {
            Debug.LogError(
         "GetImages failed\n" +
         "URL: " + url + "\n" +
         "Status: " + request.responseCode + "\n" +
         "Error: " + request.error + "\n" +
         "Result: " + request.result + "\n" +
         "Response: " + request.downloadHandler.text
     );

            callback?.Invoke(null);
        }
    }

    public IEnumerator DecryptImage(
    int id,
    string password,
    Action<Texture2D> callback)
    {
        string url = ApiClient.BaseUrl + $"/image/{id}/decrypt";

        DecryptRequest body = new DecryptRequest
        {
            encryptionPassword = password
        };

        string json = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("Decrypt Status: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] imageBytes = request.downloadHandler.data;

            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(imageBytes))
            {
                callback?.Invoke(texture);
            }
            else
            {
                Debug.LogError("Could not load image bytes.");
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError(
                "Decrypt failed: " +
                request.responseCode +
                " " +
                request.downloadHandler.text
            );

            callback?.Invoke(null);
        }
    }

    public IEnumerator DuplicateImage(
       int id,
       Action<bool> callback)
    {
        string url = ApiClient.BaseUrl + $"/image/{id}/duplicate";

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("Duplicate Status: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image duplicated successfully.");
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError(
                "Duplicate failed: " +
                request.responseCode +
                " " +
                request.downloadHandler.text
            );

            callback?.Invoke(false);
        }
    }

    public IEnumerator DeleteImage(
    int id,
    Action<bool> callback)
    {
        string url = ApiClient.BaseUrl + $"/image/{id}";

        using UnityWebRequest request = UnityWebRequest.Delete(url);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("Delete Status: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image deleted successfully.");
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError(
                "Delete failed: " +
                request.responseCode +
                " " +
                request.downloadHandler.text
            );

            callback?.Invoke(false);
        }
    }

    public IEnumerator ApplyFilter(
    int id,
    string filter,
    string password,
    Action<bool> callback)
    {
        string url = ApiClient.BaseUrl + $"/image/{id}/filter";

        FilterRequest body = new FilterRequest
        {
            filter = filter,
            encryptionPassword = password
        };

        string json = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("Filter Status: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Filter applied successfully.");
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError(
                "Filter failed: " +
                request.responseCode +
                " " +
                request.downloadHandler.text
            );

            callback?.Invoke(false);
        }
    }

    public IEnumerator UploadImage(
    string filePath,
    string subject,
    string password,
    Action<bool> callback)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        string fileName = System.IO.Path.GetFileName(filePath);

        List<IMultipartFormSection> formData =
            new List<IMultipartFormSection>();

        formData.Add(
            new MultipartFormFileSection(
                "file",
                fileData,
                fileName,
                GetContentType(filePath)
            )
        );

        formData.Add(
            new MultipartFormDataSection(
                "subject",
                subject
            )
        );

        formData.Add(
            new MultipartFormDataSection(
                "encryptionPassword",
                password
            )
        );

        string url = ApiClient.BaseUrl + "/image/upload";

        using UnityWebRequest request =
            UnityWebRequest.Post(url, formData);

        request.SetRequestHeader(
            "Authorization",
            "Bearer " + ApiClient.Token
        );

        yield return request.SendWebRequest();

        Debug.Log("Upload Status: " + request.responseCode);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful:");
            Debug.Log(request.downloadHandler.text);

            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError(
                "Upload failed: " +
                request.responseCode +
                " " +
                request.downloadHandler.text
            );

            callback?.Invoke(false);
        }
    }

    private string GetContentType(string filePath)
    {
        string extension =
            System.IO.Path.GetExtension(filePath).ToLower();

        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";

            case ".png":
                return "image/png";

            default:
                return "application/octet-stream";
        }
    }


}
        
        
        




