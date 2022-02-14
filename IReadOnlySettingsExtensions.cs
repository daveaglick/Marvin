using System.Linq;
using Statiq.Common;

namespace Marvin;

public static class IReadOnlySettingsExtensions
{
    public static ProjectSet[] GetProjectSets(this IReadOnlySettings settings) =>
        settings
            .ThrowIfNull(nameof(settings))
            .GetDocumentList(Settings.ProjectSets)
            .Select(x => new ProjectSet(x))
            .ToArray();
}