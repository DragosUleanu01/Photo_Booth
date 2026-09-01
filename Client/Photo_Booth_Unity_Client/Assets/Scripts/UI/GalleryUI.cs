using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GalleryUI : MonoBehaviour
{
    [SerializeField]
    private ImageService imageService;
    [SerializeField]
    private Transform contentParent;
    [SerializeField]
    private GameObject imageCardPrefab;
    [SerializeField]
    private ActionsPanelUI actionsPanel;
    [SerializeField]
    private GalleryUI galleryUI;
    [SerializeField]
    private TMP_Dropdown subjectDropdown;

    private ImageFileDTO[] allImages;

    private void Start()
    {
        subjectDropdown.onValueChanged.AddListener(FilterBySubject);
        StartCoroutine(imageService.GetImages(OnImagesLoaded));
    }

    private void OnImagesLoaded(ImageFileDTO[] images)
    {

        if (images == null)
        {
            Debug.LogError("Images could not be loaded.");
            return;
        }

        allImages = images;

        PopulateSubjectDropdown(images);
        DisplayImages(images);

    }

    private void DisplayImages(ImageFileDTO[] images)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var image in images)
        {
            GameObject card = Instantiate(
                imageCardPrefab,
                contentParent
            );

            ImageCardUI cardUI =
                card.GetComponent<ImageCardUI>();

            cardUI.Setup(image, actionsPanel);
        }
    }

    private void PopulateSubjectDropdown(ImageFileDTO[] images)
    {
        subjectDropdown.ClearOptions();

        List<string> options = new List<string>
    {
        "All"
    };

        foreach (var image in images)
        {
            if (!string.IsNullOrWhiteSpace(image.subject) &&
                !options.Contains(image.subject))
            {
                options.Add(image.subject);
            }
        }

        subjectDropdown.AddOptions(options);
    }

    private void FilterBySubject(int index)
    {
        if (allImages == null)
            return;

        string selectedSubject =
            subjectDropdown.options[index].text;

        if (selectedSubject == "All")
        {
            DisplayImages(allImages);
            return;
        }

        ImageFileDTO[] filteredImages =
            System.Array.FindAll(
                allImages,
                image => image.subject == selectedSubject
            );

        DisplayImages(filteredImages);
    }


    //Refresh galerie fara oprirea programului
    public void RefreshGallery()
    {
        StartCoroutine(RefreshGalleryCoroutine());
    }

    private IEnumerator RefreshGalleryCoroutine()
    {
        // Sterge cardurile existente
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        // Aduce noile imagini
        yield return null;

        yield return StartCoroutine(imageService.GetImages(OnImagesLoaded));
    }
}
