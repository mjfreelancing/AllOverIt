﻿using AllOverIt.Extensions;
using AllOverItDependencyDiagram.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverItDependencyDiagram.Generator
{
    internal static class DependencySummaryGenerator
    {
        /*
            55A9EE: A brighter blue that should provide good contrast against white and also complement the other colors you've chosen.
            6EBE50: A green color that is similar to 62B178 but brighter, and should provide good contrast against white.
            C56EE0: A purple color that is similar to B19CD9 but brighter, and should provide good contrast against white.
            FF8C67: An orange color that is similar to FF7F50 but brighter, and should provide good contrast against white.
            E3505C: A red color that is similar to CB484D but brighter, and should provide good contrast against white.
            FFC33C: A bright yellow color that can complement the other colors you've chosen and should provide good contrast against white.
        */

        private const string Blue = "55A9EE";
        private const string Green = "6EBE50";
        private const string Purple = "C56EE0";
        private const string Orange = "FF8C67";
        private const string Red = "E3505C";
        private const string Yellow = "FFC33C";

        public static readonly IDictionary<string, string> TargetFrameworkBadges = new Dictionary<string, string>
        {
            { "net7.0", $"![](https://img.shields.io/badge/.NET-7.0-{Blue}.svg)"},
            { "net7.0-windows", $"![](https://img.shields.io/badge/.NET-7.0--windows-{Blue}.svg)"},
            { "net6.0", $"![](https://img.shields.io/badge/.NET-6.0-{Orange}.svg)"},
            { "net6.0-windows", $"![](https://img.shields.io/badge/.NET-6.0--windows-{Orange}.svg)"},
            { "net5.0", $"![](https://img.shields.io/badge/.NET-5.0-{Yellow}.svg)"},
            { "net5.0-windows", $"![](https://img.shields.io/badge/.NET-5.0--windows-{Yellow}.svg)"},
            { "netcoreapp3.1", $"![](https://img.shields.io/badge/.NET-coreapp3.1-{Purple}.svg)"},
            { "netstandard2.1", $"![](https://img.shields.io/badge/.NET-standard2.1-{Green}.svg)"},
            { "netstandard2.0", $"![](https://img.shields.io/badge/.NET-standard2.0-{Red}.svg)"}
        };

        public static string CreateContent(IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Dependency Summary");
            sb.AppendLine();

            var maxLengths = new int[3];

            foreach (var solutionProject in solutionProjects)
            {
                var project = Path.GetFileNameWithoutExtension(solutionProject.Value.Path);
                sb.AppendLine($"## {project}");
                sb.AppendLine();


                var frameworkBadges = TargetFrameworkBadges.Keys
                    .Intersect(solutionProject.Value.TargetFrameworks)
                    .Select(key => TargetFrameworkBadges[key])
                    .ToList();

                var projectBadges = string.Join(" ", frameworkBadges);
                sb.AppendLine(projectBadges);

                sb.AppendLine();

                sb.AppendLine("### Dependencies");
                sb.AppendLine();

                var dependencies = AppendProjectDependencies(solutionProject.Value, solutionProjects);

                if (dependencies.Any())
                {
                    foreach (var dependency in dependencies)
                    {
                        sb.AppendLine($"* {dependency}");
                    }
                }
                else
                {
                    sb.AppendLine("* None");
                }

                sb.AppendLine();
                sb.AppendLine($"<br>");
                sb.AppendLine();
                sb.AppendLine($"---");
                sb.AppendLine();
                sb.AppendLine($"<br>");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GetProjectName(ProjectReference projectReference)
        {
            return Path.GetFileNameWithoutExtension(projectReference.Path);
        }

        private static IReadOnlyCollection<string> AppendProjectDependencies(SolutionProject solutionProject, IDictionary<string, SolutionProject> solutionProjects)
        {
            var dependencySet = new HashSet<string>();
            var transitiveSet = new HashSet<string>();

            AppendPackageDependencies(solutionProject, dependencySet, transitiveSet);

            foreach (var project in solutionProject.Dependencies.SelectMany(item => item.ProjectReferences))
            {
                AppendProjectDependenciesRecursively(project, solutionProjects, dependencySet, transitiveSet);
            }

            return dependencySet.Concat(transitiveSet)
                .Order()
                .AsReadOnlyCollection();
        }

        private static void AppendProjectDependenciesRecursively(ProjectReference projectReference, IDictionary<string, SolutionProject> solutionProjects,
            HashSet<string> dependencySet, HashSet<string> transitiveSet)
        {
            var projectName = GetProjectName(projectReference);

            dependencySet.Add(projectName);

            // Add all packages dependencies (recursively) for the current project
            foreach (var package in solutionProjects[projectName].Dependencies.SelectMany(item => item.PackageReferences))
            {
                AppendPackageDependenciesRecursively(package, dependencySet, transitiveSet);
            }

            // Add all project dependencies (recursively) for the current project
            foreach (var project in solutionProjects[projectName].Dependencies.SelectMany(item => item.ProjectReferences))
            {
                AppendProjectDependenciesRecursively(project, solutionProjects, dependencySet, transitiveSet);
            }
        }

        private static void AppendPackageDependencies(SolutionProject solutionProject, HashSet<string> dependencySet, HashSet<string> transitiveSet)
        {
            var projectName = solutionProject.Name;

            foreach (var package in solutionProject.Dependencies.SelectMany(item => item.PackageReferences))
            {
                AppendPackageDependenciesRecursively(package, dependencySet, transitiveSet);
            }
        }

        private static void AppendPackageDependenciesRecursively(PackageReference packageReference, HashSet<string> dependencySet, HashSet<string> transitiveSet)
        {
            var packageNameVersion = $"{packageReference.Name} v{packageReference.Version}";

            if (packageReference.Depth == 0)
            {
                dependencySet.Add(packageNameVersion);
            }
            else
            {
                transitiveSet.Add(packageNameVersion);
            }

            foreach (var package in packageReference.TransitiveReferences)
            {
                AppendPackageDependenciesRecursively(package, dependencySet, transitiveSet);
            }
        }
    }
}