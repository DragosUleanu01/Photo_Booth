using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageCardUI : MonoBehaviour
{
    [SerializeField]
    private RawImage preview;

    [SerializeField]
    private TMP_Text subjectText;

    [SerializeField]
    private Button actionsButton;

    private ImageFileDTO imageData;

    private ActionsPanelUI actionsPanel;

    public void Setup(
        ImageFileDTO image,
        ActionsPanelUI panel)
    {
        imageData = image;
        actionsPanel = panel;

        subjectText.text = image.subject;

        actionsButton.onClick.AddListener(OpenActions);
    }

    private void OpenActions()
    {
        actionsPanel.Open(imageData, this);
        Debug.Log("Actions for image ID: " + imageData.id);
    }

    public void SetPreview(Texture2D texture)
    {
        preview.texture = texture;
    }


}