using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class UploadPanelUI : MonoBehaviour
{
    [SerializeField] 
    private TMP_InputField subjectInput;
    [SerializeField] 
    private TMP_InputField passwordInput;

    [SerializeField] 
    private TMP_Text selectedFileText;

    [SerializeField] 
    private Button selectImageButton;
    [SerializeField] 
    private Button uploadButton;
    [SerializeField] 
    private Button closeButton;

    [SerializeField] 
    private ImageService imageService;

    [SerializeField] 
    private GalleryUI galleryUI;

    private string selectedFilePath;

    private void Start()
    {
        selectImageButton.onClick.AddListener(SelectImage);
        uploadButton.onClick.AddListener(Upload);
        closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        subjectInput.text = "";
        passwordInput.text = "";

        selectedFilePath = null;
        selectedFileText.text = "No image selected";

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void SelectImage()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        if (paths.Length == 0)
        {
            return;

        }

        selectedFilePath = paths[0];
        selectedFileText.text = System.IO.Path.GetFileName(selectedFilePath);

        Debug.Log("Selected image: " + selectedFilePath);

    }

    private void Upload()
    {
        if (string.IsNullOrEmpty(selectedFilePath))
        {
            Debug.LogWarning("Select an image first.");
            return;
        }

        if (string.IsNullOrWhiteSpace(subjectInput.text))
        {
            Debug.LogWarning("Enter a subject.");
            return;
        }

        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            Debug.LogWarning("Enter an encryption password.");
            return;
        }

        StartCoroutine(
            imageService.UploadImage(
                selectedFilePath,
                subjectInput.text,
                passwordInput.text,
                OnUploadFinished
            )
        );
    }

    private void OnUploadFinished(bool success)
    {
        if (!success)
            return;

        Close();
        galleryUI.RefreshGallery();
    }
}