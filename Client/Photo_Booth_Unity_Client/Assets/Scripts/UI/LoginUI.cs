using UnityEngine;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField emailInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private TMP_Text errorText;
    [SerializeField]
    private AuthService authService;
    [SerializeField]
    private Canvas registerUI;
    [SerializeField]
    private Canvas loginUI;

    public void ShowRegister()
    {
        loginUI.gameObject.SetActive(false);
        registerUI.gameObject.SetActive(true);
    }

    public void ShowLogin()
    {
        registerUI.gameObject.SetActive(false);
        loginUI.gameObject.SetActive(true);
    }
    
    public void Login()
    {
        errorText.text = "";

        StartCoroutine(authService.Login(emailInput.text, passwordInput.text, OnLoginFinished));
    }

    private void OnLoginFinished(bool success)
    {
        if (success)
        {
            errorText.gameObject.SetActive(false);
            Debug.Log("Logat cu succes!");
            SceneManager.LoadScene("Gallery");

        }
        else
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Email sau parola incorecta!";
        }


    }

}
