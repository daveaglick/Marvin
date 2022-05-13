# Marvin
[Marvin is a robot](https://en.wikipedia.org/wiki/Marvin_the_Paranoid_Android) that helps Dave with mundane jobs like building projects and publishing releases. It also contains common artifacts. It's pretty customized to Dave's specific working environment and project layout but maybe you'll find parts of it helpful too.

## Adding Marvin

- Add a git submodule in the `marvin` directory: `git submodule add https://github.com/daveaglick/Marvin.git marvin`.
- Add the `marvin` and `cache` directories to the root `.gitignore` file (otherwise all the Marvin files and it's build cache will also get committed to the parent repository).

## Other Things To Do

- Add a version control mapping for the `marvin` folder to Rider (Settings -> Version Control -> Directory Mappings) and change the Git and Commit toolboxes to group by repository.
- Copy the `copy/marvin.cmd` file to the root of the repository.
- Copy the `copy/marvin.yml` file to the root of the repository and replace `ProjectName` placeholders.
- Copy the `copy/Directory.Build.props` file to the root of the repository and replace `ProjectName` placeholders (pay attention to settings further down in the file like `PackageProjectUrl`). Delete any obsolete elements from existing `.csproj` files that are covered in the common `Directory.Build.props`.
- Add a `icon.png` to the root of the repository (or remove those entries from the `Directory.Build.props` file).
- Add a `<Description>` element to either the common `Directory.Build.props` file or each `.csproj` file.
- Copy the `copy/.editorconfig` file to the repository root.
- Copy the `copy/tests/.editorconfig` file to the `tests` directory.
- Copy the files from `copy/workflows` to `.github/workflows`.
- Copy the `FUNDING.yml` file to `.github`.
- Copy a valid `davidglick.pfx` file from one of the existing repositories to the root of the repository (this file should not be checked in).

## Using Marvin

Use the `marvin.cmd` command added above:
- `marvin`: builds, tests, and packs the project(s).
- `marvin deploy`: builds, tests, packs, and publishes the project(s).
- `marvin addlabels -s GitHubOwner=owner -s GitHubName=name`: adds default labels to the GitHub project (but will not delete any existing ones).