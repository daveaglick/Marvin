namespace Marvin;

public static class Settings
{
    public const string ProjectSets = nameof(ProjectSets);
    public const string IsBuildServer = nameof(IsBuildServer);
    public const string NuGetApiKey = nameof(NuGetApiKey);
    public const string GitHubToken = nameof(GitHubToken);

    // Used for repo-specific pipelines like AddLabels
    public const string GitHubOwner = nameof(GitHubOwner);
    public const string GitHubName = nameof(GitHubName);
}