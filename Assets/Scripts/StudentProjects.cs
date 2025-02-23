using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StudentProjects
{
    public string name;
    public string cover;
    public string path;
}

[System.Serializable]
public class StudentProjectsWrapper
{
    public List<StudentProjects> projects;
}
