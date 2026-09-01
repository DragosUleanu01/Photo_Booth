using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionsPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private Button viewButton;
    [SerializeField] private Button duplicateButton;
    [SerializeField] private Button filterButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button closeButton;

    private ImageFileDTO selectedImage;
    
    [SerializeField]
    private ImageService imageService;

    private ImageCardUI selectedCard;
    [SerializeField]
    private GalleryUI galleryUI;

    [SerializeField] 
    private TMP_Dropdown filterDropdown;

    public void Open(
    ImageFileDTO image,
    ImageCardUI card)
    {
        selectedImage = image;
        selectedCard = card;

        titleText.text = image.subject;
        passwordInput.text = "";

        gameObject.SetActive(true);
    }

    public void Close()
    {
        selectedImage = null;
        selectedCard = null;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        viewButton.onClick.AddListener(ViewImage);
        duplicateButton.onClick.AddListener(DuplicateImage);
        filterButton.onClick.AddListener(ApplyFilter);
        deleteButton.onClick.AddListener(DeleteImage);
    }

    private void ViewImage()
    {
        if (selectedImage == null)
            return;

        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogWarning("Enter encryption password.");
            return;
        }

        StartCoroutine(
            imageService.DecryptImage(
                selectedImage.id,
                password,
                OnImageDecrypted
            )
        );
    }

    private void OnImageDecrypted(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Image could not be decrypted.");
            return;
        }

        selectedCard.SetPreview(texture);

        Close();
    }
    private void DuplicateImage()
    {
        if (selectedImage == null)
            return;

        StartCoroutine(
            imageService.DuplicateImage(
                selectedImage.id,
                OnImageDuplicated
            )
        );
    }
    private void OnImageDuplicated(bool success)
    {
        if (!success)
            return;

        Close();
        galleryUI.RefreshGallery();
    }

    private void DeleteImage()
    {
        if (selectedImage == null)
            return;

        StartCoroutine(
            imageService.DeleteImage(
                selectedImage.id,
                OnImageDeleted
            )
        );
    }
    private void OnImageDeleted(bool success)
    {
        if (!success)
            return;

        Close();

        galleryUI.RefreshGallery();
    }

    private void ApplyFilter()
    {
        if (selectedImage == null)
            return;

        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(password))
        {
            Debug.LogWarning("Enter encryption password.");
            return;
        }

        string selectedFilter =
            filterDropdown.options[filterDropdown.value].text;

        StartCoroutine(
            imageService.ApplyFilter(
                selectedImage.id,
                selectedFilter,
                password,
                OnFilterApplied
            )
        );
    }
    private void OnFilterApplied(bool success)
    {
        if (!success)
            return;

        Close();
        galleryUI.RefreshGallery();
    }



}
