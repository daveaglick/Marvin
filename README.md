# Marvin
[Marvin is a robot](https://en.wikipedia.org/wiki/Marvin_the_Paranoid_Android) that helps Dave with mundane jobs like building projects and publishing releases. It's pretty customized to Dave's specific working environment and project layout but maybe you'll find parts of it helpful too.

## Adding Marvin

- Add a git submodule in the `marvin` directory: `git submodule add https://github.com/daveaglick/Marvin.git marvin`.
- Add the `marvin` directory to the root `.gitignore` file (otherwise all the Marvin files will also get committed to the parent repository).
- Create a `marvin.cmd` file in the root of the containing repository:
  ```
  @echo off
  cd "marvin"
  dotnet run -- %*
  set exitcode=%errorlevel%
  cd %~dp0
  exit /b %exitcode%
  ```
- Create a `marvin.yml` file in the root of the containing repository:
  ```
  ProjectSets:
    - Name: Buildalyzer
      SourceProjects: "src/*/*.csproj"
      TestProjects: "tests/*/*.csproj"
      GitHubOwner: "daveaglick"
      GitHubName: "Buildalyzer"
  GitHubToken: => GetString("GITHUB_TOKEN")
  NuGetApiKey: => GetString("DAVEAGLICK_NUGET_API_KEY")
  ```
- Add a version control mapping for the `marvin` folder to Rider (Settings -> Version Control -> Directory Mappings) and change the Git and Commit toolboxes to group by repository.

## Using Marvin

Use the `marvin.cmd` command added above:
- `marvin`: builds, tests, and packs the project(s).
- `marvin deploy`: builds, tests, packs, and publishes the project(s).