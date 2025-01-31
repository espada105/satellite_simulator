using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SimpleFileBrowser;

public class FileUploader : MonoBehaviour
{
    public Image targetImage; // UI에 표시할 Image 컴포넌트

    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
    }

    public void OpenFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select an Image", "Load");

        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0]; // 선택된 파일 경로
            StartCoroutine(LoadImage(filePath));
        }
        else
        {
            Debug.Log("File selection was canceled.");
        }
    }

    IEnumerator LoadImage(string filePath)
    {
        byte[] imageBytes = FileBrowserHelpers.ReadBytesFromFile(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        if (texture != null)
        {
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        }

        yield return null;
    }
}
