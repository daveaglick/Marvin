using System;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;

namespace Marvin;

public class Program
{
    private static readonly NormalizedPath ArtifactsFolder = "artifacts";

    public static async Task<int> Main(string[] args) =>
        await Bootstrapper
            .Factory
            .CreateDefaultWithout(args, DefaultFeatures.Pipelines)
            .BuildConfiguration(builder => builder.AddSettingsFile(
                new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Combine("marvin")))
            .ConfigureFileSystem(x =>
            {
                x.RootPath = new NormalizedPath(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
                x.OutputPath = x.RootPath / ArtifactsFolder;
                x.InputPaths.Clear();
                x.InputPaths.Add(x.RootPath);
            })
            .ConfigureSettings(settings =>
            {
                settings.Add(Settings.IsBuildServer, settings.ContainsAnyKeys("GITHUB_ACTIONS", "TF_BUILD"));
                settings[Keys.CleanMode] = CleanMode.Full;
            })
            .AddPipeline<Pipelines.GetVersions>()
            .AddPipeline<Pipelines.AddLabels>()
            .AddPipelines((settings, pipelines) =>
            {
                foreach (ProjectSet projectSet in settings.GetProjectSets())
                {
                    pipelines.Add(new Pipelines.Build(projectSet));
                    pipelines.Add(new Pipelines.Test(projectSet));
                    pipelines.Add(new Pipelines.Pack(projectSet));
                    pipelines.Add(new Pipelines.Publish(projectSet));
                    pipelines.Add(new Pipelines.Deploy(projectSet));
                }
            })
            .RunAsync();
}