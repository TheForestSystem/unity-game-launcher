using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JSON_Reader
{
    private static string configPath;
    private static List<StudentProjects> studentProjects;

    public static void Initialize(string path)
    {
        configPath = path;
        LoadJson();
    }


    private static void LoadJson()
    {
        if (!File.Exists(configPath))
        {
            Debug.LogError($"Config file not found at: {configPath}");
            return;
        }

        string json = File.ReadAllText(configPath);

        // Wrap the array in a JSON object so Unity's JsonUtility can parse it
        string wrappedJson = $"{{\"projects\":{json}}}";

        StudentProjectsWrapper wrapper = JsonUtility.FromJson<StudentProjectsWrapper>(wrappedJson);

        if (wrapper == null)
        {
            Debug.LogError("JSON parsing returned null!");
            return;
        }

        if (wrapper.projects == null || wrapper.projects.Count == 0)
        {
            Debug.LogError("Parsed JSON but the list is empty.");
            return;
        }

        studentProjects = wrapper.projects;
        Debug.Log($"Loaded {studentProjects.Count} projects.");
    }



    public static List<StudentProjects> GetStudentProjects()
    {
        string msg = studentProjects != null ? 
            $"Returning {studentProjects.Count} projects." : "No projects loaded.";

        Debug.Log(msg);

        return studentProjects;
    }

    public static List<StudentProjects> Refresh()
    {
        LoadJson();
        return studentProjects;
    }
}
