using Statiq.Common;
using Statiq.Core;

namespace Marvin.Pipelines
{
    public class Pack : Pipeline, INamedPipeline
    {
        private readonly ProjectSet _projectSet;

        public Pack(ProjectSet projectSet)
        {
            _projectSet = projectSet.ThrowIfNull(nameof(projectSet));

            Dependencies.Add($"{nameof(Test)}-{projectSet.Name}");

            if (projectSet.ProjectSetDependencies is object)
            {
                foreach (string dependency in projectSet.ProjectSetDependencies)
                {
                    Dependencies.Add($"{nameof(Pack)}-{dependency}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ThrowExceptionIf(Config.ContainsSettings("DAVIDGLICK_CERTPASS").IsFalse(), "DAVIDGLICK_CERTPASS setting missing"),
                new ReadFiles(_projectSet.RootPath.Combine(_projectSet.SourceProjects).FullPath),
                new ExecuteConfig(Config.FromContext(context =>
                    new StartProcess("dotnet")
                        .WithArgument("pack")
                        .WithArgument("--no-build")
                        .WithArgument("--no-restore")
                        .WithVersions(context)
                        .WithArgument("-o", Config.FromContext(ctx => ctx.FileSystem.GetOutputPath(projectSet.Name).FullPath), true)
                        .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                        .WithParallelExecution(false)
                        .LogOutput())),
                new ReadFiles(Config.FromContext(ctx => ctx.FileSystem.GetOutputPath($"{projectSet.Name}/*.nupkg").FullPath)),
                new StartProcess("nuget")
                    .WithArgument("sign")
                    .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                    .WithArgument("-CertificatePath", Config.FromContext(ctx => ctx.FileSystem.GetRootFile("davidglick.pfx").Path.FullPath), true)
                    .WithArgument("-CertificatePassword", Config.FromSetting("DAVIDGLICK_CERTPASS"), true)
                    .WithArgument("-Timestamper", "http://timestamp.digicert.com", true)
                    .WithArgument("-NonInteractive")
                    .WithParallelExecution(false)
                    .HideArguments(true)
                    .LogOutput()
                    .KeepContent(),
                new ExecuteIf(
                    Config.ContainsSettings(nameof(Settings.CopyPackagesTo)),
                    new SetDestination(Config.FromDocument((doc, ctx) =>
                        NormalizedPath.Combine(ctx.GetPath(nameof(Settings.CopyPackagesTo)), doc.Source.FileName))),
                    new LogMessage(Config.FromDocument(doc => $"Writing package to {doc.Destination.FullPath}")),
                    new WriteFiles())
            };
        }

        public string PipelineName => $"{nameof(Pack)}-{_projectSet.Name}";
    }
}