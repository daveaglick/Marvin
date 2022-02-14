using Marvin.Pipelines;
using Statiq.Common;
using Statiq.Core;

namespace Marvin;

public static class StartProcessExtensions
{
    public static StartProcess WithVersions(this StartProcess startProcess, IExecutionContext context)
    {
        startProcess.ThrowIfNull(nameof(startProcess));
        context.ThrowIfNull(nameof(context));

        // Add the versions of all project sets prefixed with their name
        // This way projects can reference other projects with the correct versioning
        // The project file should have something like this:
        // <Version Condition="'$(ProjectSetNameVersion)' == ''">1.0.0</Version>
        // <Version Condition="'$(ProjectSetNameVersion)' != ''">$(ProjectSetNameVersion)</Version>
        foreach (ProjectSet project in context.Settings.GetProjectSets())
        {
            startProcess = startProcess.WithArgument(Config.FromContext(ctx =>
                $"-p:{project.Name}Version=\"{ctx.Outputs.FromPipeline(nameof(GetVersions))[0].GetString(project.Name)}\""));
        }

        return startProcess;
    }
}