// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/sample/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.NuGet;
using Nuke.Core;
using Nuke.Core.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Core.EnvironmentInfo;

class SampleBuild : GitHubBuild
{
    public static void Main () => Execute<SampleBuild>(x => x.Compile);

    Target Clean => _ => _
            .Executes(() => DeleteDirectories(GlobDirectories(SolutionDirectory, "**/bin", "**/obj")))
            .Executes(() => PrepareCleanDirectory(OutputDirectory));

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildRestore));

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildCompile));

    Target Pack => _ => _
            .DependsOn(Compile)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildPack));

    Target Push => _ => _
            .DependsOn(Pack)
            .Executes(() => GlobFiles(OutputDirectory, "*.nupkg")
                    .Where(x => !x.EndsWith("symbols.nupkg"))
                    .ForEach(x => NuGetPush(s => s
                            .SetTargetPath(x)
                            .SetApiKey(EnsureVariable("API_KEY"))
                            .SetSource(EnsureVariable("SOURCE")))));
}
