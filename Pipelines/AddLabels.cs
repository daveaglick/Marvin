using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Octokit;
using Statiq.Common;
using Statiq.Core;

namespace Marvin.Pipelines
{
    public class AddLabels : Pipeline
    {
        private static readonly Label[] Labels = new[]
        {
            new Label("⚠ Bug", "Something isn't working as expected", "b60205"),
            new Label("🚨 Breaking Change", "Potentially breaks existing users", "fbca04"),
            new Label("General Enhancement", "New feature or request", "0052cc"),
            new Label("Tooling", "New feature or request related to tooling", "0052cc"),
            new Label("Performance", "New feature or request related to improving performance", "0052cc"),
            new Label("On Hold", "Waiting for other conditions to be met", "fbca04"),
            new Label("Feedback Needed", "Further information is requested", "fbca04"),
            new Label("Duplicate", "This issue or pull request already exists", "ffffff"),
            new Label("Out Of Scope", "This will not be worked on", "ffffff"),
            new Label("Documentation", "Improvements to documentation", "5319e7"),
            new Label("Discussion/Question", "Discussions or questions about the code", "006b75")
        };

        private class Label
        {
            public Label(string name, string description, string color)
            {
                Name = name;
                Description = description;
                Color = color;
            }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Color { get; set; }
        }

        public AddLabels()
        {
            ExecutionPolicy = ExecutionPolicy.Manual;

            ProcessModules = new ModuleList
            {
                // ReSharper disable once HeapView.ObjectAllocation
                new ExecuteConfig(Config.FromContext(async ctx =>
                {
                    GitHubClient github = new GitHubClient(new ProductHeaderValue("Marvin"))
                    {
                        Credentials = new Credentials(ctx.GetString(Settings.GitHubToken))
                    };

                    string owner = ctx.GetString(Settings.GitHubOwner);
                    if (owner.IsNullOrWhiteSpace())
                    {
                        throw new Exception("GitHubOwner is not set");
                    }

                    string name = ctx.GetString(Settings.GitHubName);
                    if (name.IsNullOrWhiteSpace())
                    {
                        throw new Exception("GitHubName is not set");
                    }

                    // Get existing labels
                    IReadOnlyList<Octokit.Label> existingLabels =
                        await github.Issue.Labels.GetAllForRepository(owner, name);

                    // Add or update labels
                    foreach (Label label in Labels)
                    {
                        // Get existing label
                        ctx.LogInformation($"Checking label {label.Name}...");
                        Octokit.Label existingLabel = existingLabels
                            .FirstOrDefault(x => x.Name.Equals(label.Name, StringComparison.OrdinalIgnoreCase));

                        // Does the color match?
                        if (existingLabel is object
                            && (!existingLabel.Color.Equals(label.Color, StringComparison.OrdinalIgnoreCase)
                            || existingLabel.Description != label.Description))
                        {
                            try
                            {
                                // Update the label
                                await github.Issue.Labels.Update(
                                    owner,
                                    name,
                                    existingLabel.Name,
                                    new LabelUpdate(label.Name, label.Color)
                                    {
                                        Description = label.Description
                                    });
                            }
                            catch (Exception e)
                            {
                                ctx.LogWarning($"Could not update label {label.Name}: {e.Message}");
                            }
                        }
                        else
                        {
                            try
                            {
                                await github.Issue.Labels.Create(
                                    ctx.GetString(Settings.GitHubOwner),
                                    ctx.GetString(Settings.GitHubName),
                                    new NewLabel(label.Name, label.Color)
                                    {
                                        Description = label.Description
                                    });
                                ctx.LogInformation($"Added label {label} to GitHub");
                            }
                            catch (Exception e)
                            {
                                ctx.LogWarning($"Could not add label {label.Name}: {e.Message}");
                            }
                        }
                    }
                }))
            };
        }
    }
}