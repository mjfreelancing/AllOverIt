﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using SolutionInspector.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionInspector
{
    internal sealed class D2DependencyGenerator
    {
        public ProjectAliases ProjectAliases { get; private set; }

        public D2DependencyGenerator(string solutionPath, string projectsRootPath)
        {
            _ = solutionPath.WhenNotNullOrEmpty();
            _ = projectsRootPath.WhenNotNullOrEmpty();

            var allAliases = new Dictionary<string, string>();
            var allDependencies = new HashSet<ProjectDependency>();

            ProjectAliases = new ProjectAliases(allAliases, allDependencies);
                
            InitProjectAliases(solutionPath, projectsRootPath);
        }

        public async Task CreateAllFilesAsync(string docsPath)
        {
            var scopes = new[] { string.Empty }     // everything
                .Concat(ProjectAliases.AllAliases.Where(item => item.Key.StartsWith("alloverit"))
                .SelectAsReadOnlyCollection(item => item.Key));

            var extensions = new[] { "svg", "png" /*, "pdf"*/ };

            foreach (var scope in scopes)
            {
                // Get the D2 diagram content
                var d2Content = GenerateD2Content(scope);

                // Create the file and return the fully-qualified file path
                var filePath = await CreateD2FileAsync(d2Content, docsPath, scope);

                foreach (var extension in extensions)
                {
                    await ExportD2ImageFileAsync(filePath, extension);
                }
            }
        }

        private void InitProjectAliases(string solutionPath, string projectsRootPath)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths
            var solutionParser = new SolutionParser(solutionPath, projectsRootPath);

            foreach (var project in solutionParser.Projects)
            {
                var projectAlias = project.Name.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectAlias, project.Name, ProjectAliases.AllAliases);
                AddProjectDependencies(projectAlias, project.Dependencies);
                AddPackageDependencies(projectAlias, project.Dependencies);
            }
        }

        private void AddProjectDependencies(string projectAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies)
        {
            var allProjectDependencies = projectDependencies.SelectMany(dependency => dependency.ProjectReferences);

            foreach (var projectDependency in allProjectDependencies)
            {
                var projectDependencyName = Path.GetFileNameWithoutExtension(projectDependency.Path);

                var projectDependencyAlias = projectDependencyName.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectDependencyAlias, projectDependencyName, ProjectAliases.AllAliases);

                var dependency = new ProjectDependency(projectAlias, projectDependencyAlias);
                ProjectAliases.AllDependencies.Add(dependency);
            }
        }

        private void AddPackageDependencies(string parentAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies)
        {
            var allPackageDependencies = projectDependencies.SelectMany(dependency => dependency.PackageReferences);

            foreach (var packageDependency in allPackageDependencies)
            {
                ProcessPackageReference(parentAlias, packageDependency);
            }
        }



        private void ProcessPackageReference(string packageAlias, PackageReference packageDependency)
        {
            var packageDependencyName = packageDependency.Name;

            var packageDependencyAlias = packageDependencyName.Replace(".", "-").ToLowerInvariant();

            UpdateAliasCache(packageDependencyAlias, packageDependencyName, ProjectAliases.AllAliases);

            var dependency = new ProjectDependency(packageAlias, packageDependencyAlias);
            
            if (ProjectAliases.AllDependencies.Add(dependency)) // dependency is a record type
            {
                Console.WriteLine($"{dependency.Alias} - {dependency.DependencyAlias}");

                foreach (var transitive in packageDependency.TransitiveReferences)
                {
                    ProcessPackageReference(packageDependencyAlias, transitive);
                }
            }
        }






        private string GenerateD2Content(string scope = default)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            var allAliases = ProjectAliases.AllAliases;
            var allDependencies = ProjectAliases.AllDependencies.AsEnumerable();

            var aliasesUsed = allAliases;

            if (scope.IsNotNullOrEmpty())
            {
                IEnumerable<ProjectDependency> GetProjectDependencies(string alias)
                {
                    var dependencies = ProjectAliases.AllDependencies.Where(item => item.Alias == alias);

                    foreach (var dependency in dependencies)
                    {
                        yield return dependency;

                        foreach (var dependencyAlias in GetProjectDependencies(dependency.DependencyAlias))
                        {
                            yield return dependencyAlias;
                        }
                    }
                }

                // Get all dependencies recursively
                allDependencies = GetProjectDependencies(scope).AsReadOnlyCollection();

                aliasesUsed = new Dictionary<string, string>();

                if (allDependencies.Any())
                {
                    foreach (var (alias, dependencyAlias) in allDependencies)
                    {
                        UpdateAliasCache(alias, allAliases[alias], aliasesUsed);
                        UpdateAliasCache(dependencyAlias, allAliases[dependencyAlias], aliasesUsed);
                    }
                }
                else
                {
                    // If there's no dependencies then add the scope itself - otherwise there'll be nothing to export
                    UpdateAliasCache(scope, allAliases[scope], aliasesUsed);
                }
            }

            foreach (var alias in aliasesUsed)
            {
                // Alias: Display Name
                sb.AppendLine($"{GetDiagramAliasId(alias.Key)}: {alias.Value}");

                // Style non-AllOverIt dependencies
                if (!alias.Key.StartsWith("alloverit"))
                {
                    sb.AppendLine($"{alias.Key}.style.fill: lightblue");
                    sb.AppendLine($"{alias.Key}.style.opacity: 0.8");
                }
            }

            sb.AppendLine();

            foreach (var (alias, dependencyAlias) in allDependencies)
            {
                sb.AppendLine($"{GetDiagramAliasId(dependencyAlias)} <- {GetDiagramAliasId(alias)}");
            }

            return sb.ToString();
        }

        private static void UpdateAliasCache(string alias, string name, IDictionary<string, string> aliases)
        {
            if (!aliases.ContainsKey(alias))
            {
                aliases.Add(alias, name);
            }
        }

        private static async Task<string> CreateD2FileAsync(string content, string docsPath, string scope)
        {
            var fileName = scope.IsNullOrEmpty()
                ? "alloverit-all.d2"
                : $"{scope}.d2";

            var d2FilePath = Path.Combine(docsPath, fileName);

            Console.WriteLine($"Creating D2 diagram and images for '{Path.GetFileNameWithoutExtension(fileName)}'...");

            File.WriteAllText(d2FilePath, content);

            await ProcessBuilder
                .For("d2.exe")
                .WithArguments("fmt", d2FilePath)
                .BuildProcessExecutor()
                .ExecuteAsync();

            Console.WriteLine($"Created diagram {d2FilePath}");

            return d2FilePath;
        }

        private static async Task ExportD2ImageFileAsync(string d2FileName, string extension)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, extension);

            var export = ProcessBuilder
               .For("d2.exe")
               .WithArguments("-l", "elk", d2FileName, imageFileName)
               .BuildProcessExecutor();

            await export.ExecuteAsync();

            Console.WriteLine($"Exported image {imageFileName}");
        }

        private static string GetDiagramAliasId(string alias)
        {
            // Group all 'AllOverIt' packages together
            return alias.StartsWith("alloverit")
                ? $"aoi.{alias}"
                : alias;
        }
    }
}