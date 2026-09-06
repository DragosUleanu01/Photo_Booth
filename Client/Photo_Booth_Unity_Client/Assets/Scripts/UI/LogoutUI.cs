using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutUI : MonoBehaviour
{
    public void Logout()
    {
        
        ApiClient.Token = "";

        
        SceneManager.LoadScene("Login");
    }
}