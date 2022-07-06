using System;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Core;

namespace Marvin.Pipelines
{
    public class Build : Pipeline, INamedPipeline
    {
        private const int RestoreRetries = 40;

        private const int RestoreDelaySeconds = 30;

        private readonly ProjectSet _projectSet;

        public Build(ProjectSet projectSet)
        {
            _projectSet = projectSet.ThrowIfNull(nameof(projectSet));

            Dependencies.Add(nameof(GetVersions));

            if (projectSet.ProjectSetDependencies is object)
            {
                foreach (string dependency in projectSet.ProjectSetDependencies)
                {
                    Dependencies.Add($"{nameof(Build)}-{dependency}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ReadFiles(_projectSet.RootPath.Combine(_projectSet.SourceProjects).FullPath),
                new RetryModules(
                    new ExecuteConfig(Config.FromContext(context =>
                        new StartProcess("dotnet")
                            .WithArgument("restore")
                            .WithArgument("--no-cache")
                            .WithVersions(context)
                            .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                            .WithParallelExecution(false)
                            .LogErrors(false)
                            .LogOutput())))
                    .WithRetries(RestoreRetries)
                    .WithSleepDuration(x => TimeSpan.FromSeconds(x * RestoreDelaySeconds))
                    .WithFailureMessage(
                        x => $"Error restoring packages, retry {x} of {RestoreRetries} in {RestoreDelaySeconds} seconds...",
                        LogLevel.Warning),
                new ExecuteConfig(Config.FromContext(context =>
                    new StartProcess("dotnet")
                        .WithArgument("build")
                        .WithVersions(context)
                        .WithArgument("-p:ContinuousIntegrationBuild=\"true\"")
                        .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                        .WithParallelExecution(false)
                        .LogOutput()))
            };
        }

        public string PipelineName => $"{nameof(Build)}-{_projectSet.Name}";
    }
}