// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/nuke-build/sample/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Core;
using Nuke.Core.Execution;
using Nuke.Sample;

[assembly: IconClass(typeof(SampleTasks), "power-cord2")]

namespace Nuke.Sample
{
    [PublicAPI]
    public class SampleTasks
    {
        public static void LogConfiguration()
        {
            var configuration = Build.Instance?.Configuration;
            Logger.Info($"Build is running in {configuration}.");
        }
    }
}
