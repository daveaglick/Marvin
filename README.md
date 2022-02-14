# Marvin
[Marvin is a robot](https://en.wikipedia.org/wiki/Marvin_the_Paranoid_Android) that helps Dave with mundane jobs like building projects and publishing releases. It's pretty customized to Dave's specific working environment and project layout but maybe you'll find parts of it helpful too.

## Add Marvin to a project

- Add a git submodule in the build directory: `git submodule add https://github.com/daveaglick/Marvin.git build`.
- Add the `build` directory to the root `.gitignore` file (otherwise all the Marvin files will also get committed to the parent repository).
- Create a `marvin.cmd` file in the root of the containing repository:
  ```
  @echo off
  cd "build"
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
  GitHubToken: => GITHUB_TOKEN
  NuGetApiKey: => DAVEAGLICK_NUGET_API_KEY
  ```
- Add the Marvin project to the solution (optional but recommended if making changes directly to Marvin).
- Add a version control mapping for the build folder to Rider (Settings -> Version Control -> Directory Mappings) and change the Git and Commit toolboxes to group by repository (optional but recommended if making changes directly to Marvin).

## Use Marvin

- `marvin`: builds, tests, and packs the project(s).
- `marvin deploy`: builds, tests, packs, and publishes the project(s).