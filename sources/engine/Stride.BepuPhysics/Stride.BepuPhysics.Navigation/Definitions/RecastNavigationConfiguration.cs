﻿using Stride.Core;
using Stride.Core.Threading;
using Stride.Data;

namespace Stride.BepuPhysics.Navigation.Definitions;
[DataContract("RecastNavigationConfiguration")]
[Display("Recast Navigation")]
public class RecastNavigationConfiguration : Configuration
{
    [Display("Build Settings", Expand = ExpandRule.Never)]
    public BuildSettings BuildSettings { get; set; } = new();
    /// <summary>
    /// Total thread count to use for pathfinding. By default uses half of the available threads due to noticable stutter if all threads are used.
    /// </summary>
    public int UsableThreadCount { get; set; } = Dispatcher.MaxDegreeOfParallelism / 2;
}
