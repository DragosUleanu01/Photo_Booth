using TMPro;
using UnityEngine;

public class RegisterUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInput;
    [SerializeField]
    private TMP_InputField emailInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private TMP_Text messageText;
    [SerializeField]
    private AuthService authService;

    public void Register()
    {
        messageText.text = "";
        StartCoroutine(authService.Register(usernameInput.text, emailInput.text, passwordInput.text, OnRegisterFinished));
    }

    private void OnRegisterFinished(bool success)
    {
        if (success)
        {
            messageText.gameObject.SetActive(false);
            messageText.text = "Cont creat cu success";
        }
        else
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Inregistrarea a esuat";
        }
    }


}
