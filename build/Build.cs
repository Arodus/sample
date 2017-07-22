// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/powershell/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Core;
using Nuke.Core.Utilities.Collections;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Core.IO.FileSystemTasks;
using static Nuke.Core.IO.PathConstruction;

class Build : NukeBuild
{
    [Parameter] readonly string MyGetApiKey;
    [GitVersion] readonly GitVersion GitVersion;

    public static int Main () => Execute<Build>(x => x.Compile);

    Target Clean => _ => _
            .Executes(() => DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj")))
            .Executes(() => EnsureCleanDirectory(OutputDirectory));

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildRestore));

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildCompileWithVersion));

    Target Pack => _ => _
            .DependsOn(Compile)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildPack));

    Target Push => _ => _
            .DependsOn(Pack)
            .Executes(() => GlobFiles(OutputDirectory, "*.nupkg")
                    .Where(x => !x.EndsWith("symbols.nupkg"))
                    .ForEach(x => NuGetPush(s => s
                            .SetTargetPath(x)
                            .SetSource("SOURCE_URL")
                            .SetApiKey(MyGetApiKey))));
}
