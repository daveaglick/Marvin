using System.Collections.Generic;
using Statiq.Common;

namespace Marvin;

public class ProjectSet
{
    public ProjectSet(IDocument document)
    {
        Name = document.GetString(nameof(Name));
        RootPath = document.GetPath(nameof(RootPath), NormalizedPath.Empty);
        SourceProjects = document.GetString(nameof(SourceProjects));
        TestProjects = document.GetString(nameof(TestProjects));
        GitHubOwner = document.GetString(nameof(GitHubOwner));
        GitHubName = document.GetString(nameof(GitHubName));
        ProjectSetDependencies = document.GetList<string>(nameof(ProjectSetDependencies));
    }

    public string Name { get; }

    public NormalizedPath RootPath { get; }

    public string SourceProjects { get; }

    public string TestProjects { get; }

    public string GitHubOwner { get; }

    public string GitHubName { get; }

    public IReadOnlyList<string> ProjectSetDependencies { get; }
}