using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FileReader : MonoBehaviour
{
    // Image reference
    [SerializeField] private GameObject GameLauncherPrefab;
    [SerializeField] private GameObject GameLauncherParent;

    private string PATH = Application.dataPath + "/../StudentProjects"; // Path to the folder containing the JSON file and images
    private const float REFRESH_INTERVAL = 5f; // Time interval in seconds to refresh the file


    private IEnumerator Start()
    {
        string configPath = Path.Combine(PATH, "conf.json");

        if (!File.Exists(configPath))
        {
            Debug.LogError("File not found: " + configPath);
            yield break;
        }

        JSON_Reader.Initialize(configPath);

        // Wait to ensure LoadJson() completes before accessing projects
        yield return new WaitForSeconds(1f);

        StartCoroutine(AutoRefresh());
    }



    private IEnumerator AutoRefresh()
    {
        while (true) // Infinite loop to keep refreshing the file
        {
            CheckForNewGames(); // Read the file content
            yield return new WaitForSeconds(REFRESH_INTERVAL); // Wait for the defined interval before refreshing again
        }
    }

    private void CheckForNewGames()
    {
        DestroyGameLaunchers(); // Destroy existing game launchers

        try
        {
            List<StudentProjects> studentProjects = JSON_Reader.GetStudentProjects();
            if (studentProjects != null && studentProjects.Count > 0)
            {
                // Create new game launchers
                foreach (StudentProjects project in studentProjects)
                {
                    NewGameLauncher(project);
                }
            }
            else
            {
                Debug.LogError("Failed to parse JSON or empty project list.");
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
        }
    }



    #region Helper Functions

    private (Sprite, float) LoadImage(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Image file not found: " + path);
            return (null, 1f);
        }

        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (texture.LoadImage(imageBytes))
        {
            texture.Apply();
            float aspectRatio = (float)texture.width / texture.height; // Calculate aspect ratio

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return (sprite, aspectRatio);
        }

        Debug.LogError("Failed to load texture from: " + path);
        return (null, 1f);
    }

    private void AdjustImageSize(Image img, float aspectRatio)
    {
        RectTransform rt = img.rectTransform;

        // Maintain width and adjust height accordingly
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x / aspectRatio);
    }

    private GameObject NewGameLauncher(StudentProjects project)
    {
        GameObject newGameLauncher = Instantiate(GameLauncherPrefab, GameLauncherParent.transform);
        newGameLauncher.name = project.name;

        Image image = newGameLauncher.GetComponent<Image>();
        TextMeshProUGUI text = newGameLauncher.GetComponentInChildren<TextMeshProUGUI>();

        image.sprite = LoadImage(Path.Combine(PATH, project.cover)).Item1;
        //AdjustImageSize(image, LoadImage(Path.Combine(PATH, project.cover)).Item2);
        text.text = project.name;

        newGameLauncher.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Launching: " + project.name);
            System.Diagnostics.Process.Start(Path.Combine(PATH, project.path));
        });
        newGameLauncher.SetActive(true);
        return newGameLauncher;
    }

    private void DestroyGameLaunchers()
    {
        foreach (Transform child in GameLauncherParent.transform)
        {
            Destroy(child.gameObject);
        }
    }



    #endregion Helper Functions
}
