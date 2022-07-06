await Bootstrapper
    .Factory
    .CreateDefaultWithout(args, DefaultFeatures.Pipelines)
    .BuildConfiguration(builder => builder.AddSettingsFile(
        new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Combine("marvin")))
    .ConfigureFileSystem(x =>
    {
        x.RootPath = new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
        x.OutputPath = x.RootPath / Marvin.Settings.ArtifactsFolder;
        x.InputPaths.Clear();
        x.InputPaths.Add(x.RootPath);
    })
    .ConfigureSettings(settings =>
    {
        settings.Add(Keys.IgnoreExternalDestinations, true);
        settings.Add(Marvin.Settings.IsBuildServer, settings.ContainsAnyKeys("GITHUB_ACTIONS", "TF_BUILD"));
        settings[Keys.CleanMode] = CleanMode.Full;
    })
    .AddPipeline<Marvin.Pipelines.GetVersions>()
    .AddPipeline<Marvin.Pipelines.AddLabels>()
    .AddPipelines((settings, pipelines) =>
    {
        foreach (ProjectSet projectSet in settings.GetProjectSets())
        {
            pipelines.Add(new Marvin.Pipelines.Build(projectSet));
            pipelines.Add(new Marvin.Pipelines.Test(projectSet));
            pipelines.Add(new Marvin.Pipelines.Pack(projectSet));
            pipelines.Add(new Marvin.Pipelines.Publish(projectSet));
            pipelines.Add(new Marvin.Pipelines.Deploy(projectSet));
        }
    })
    .RunAsync();