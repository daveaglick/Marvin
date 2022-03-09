# Marvin
[Marvin is a robot](https://en.wikipedia.org/wiki/Marvin_the_Paranoid_Android) that helps Dave with mundane jobs like building projects and publishing releases. It also contains common artifacts. It's pretty customized to Dave's specific working environment and project layout but maybe you'll find parts of it helpful too.

## Adding Marvin

- Add a git submodule in the `marvin` directory: `git submodule add https://github.com/daveaglick/Marvin.git marvin`.
- Add the `marvin` directory to the root `.gitignore` file (otherwise all the Marvin files will also get committed to the parent repository).

## Other Things To Do

- Add a version control mapping for the `marvin` folder to Rider (Settings -> Version Control -> Directory Mappings) and change the Git and Commit toolboxes to group by repository.
- Copy the `copy/marvin.cmd` file to the root of the repository.
- Copy the `copy/marvin.yml` file to the root of the repository and replace `ProjectName` placeholders.
- Copy the `copy/Directory.Build.props` file to the root of the repository and replace `ProjectName` placeholders (pay attention to settings further down in the file like `PackageProjectUrl`). Delete any obsolete elements from existing `.csproj` files that are covered in the common `Directory.Build.props`.
- Copy the `copy/.editorconfig` file to the repository root.
- Copy the `copy/tests/.editorconfig` file to the `tests` directory.
- Copy the files from `copy/workflows` to `.github/workflows`.

## Using Marvin

Use the `marvin.cmd` command added above:
- `marvin`: builds, tests, and packs the project(s).
- `marvin deploy`: builds, tests, packs, and publishes the project(s).
- `marvin addlabels -s GitHubOwner=owner -s GitHubName=name`: adds default labels to the GitHub project (but will not delete any existing ones).