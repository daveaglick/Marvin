using Statiq.Common;
using Statiq.Core;

namespace Marvin.Pipelines
{
    public class Test : Pipeline, INamedPipeline
    {
        private readonly ProjectSet _projectSet;

        public Test(ProjectSet projectSet)
        {
            _projectSet = projectSet.ThrowIfNull(nameof(projectSet));

            Dependencies.Add($"{nameof(Build)}{projectSet.Name}");

            if (projectSet.ProjectSetDependencies is object)
            {
                foreach (string dependency in projectSet.ProjectSetDependencies)
                {
                    Dependencies.Add($"{nameof(Test)}{dependency}");
                }
            }

            ProcessModules = new ModuleList
            {
                new ReadFiles(_projectSet.RootPath.Combine(_projectSet.TestProjects).FullPath),
                new ExecuteConfig(Config.FromContext(context =>
                    new StartProcess("dotnet")
                        .WithArgument("test")
                        .WithArgument("--no-build")
                        .WithArgument("--no-restore")
                        .WithVersions(context)
                        .WithArgument(Config.FromDocument(doc => doc.Source.FullPath), true)
                        .WithParallelExecution(false)
                        .LogOutput()))
            };
        }

        public string PipelineName => $"{nameof(Test)}{_projectSet.Name}";
    }
}