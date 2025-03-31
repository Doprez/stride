// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using DotRecast.Detour.Dynamic;
using DotRecast.Recast.Toolset;
using Stride.DotRecast.Definitions;

namespace Stride.DotRecast.Extensions;
internal static class BuildSettingsExtensions
{
    /// <summary>
    /// Creates a <see cref="RcNavMeshBuildSettings"/> object from the Stride config <see cref="BuildSettings"/>.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    internal static RcNavMeshBuildSettings CreateRecastSettings(this BuildSettings settings)
    {
        return new RcNavMeshBuildSettings
        {
            cellSize = settings.CellSize,
            cellHeight = settings.CellHeight,
            agentHeight = settings.AgentHeight,
            agentRadius = settings.AgentRadius,
            agentMaxClimb = settings.AgentMaxClimb,
            agentMaxSlope = settings.AgentMaxSlope,
            agentMaxAcceleration = settings.AgentMaxAcceleration,
            minRegionSize = settings.MinRegionSize,
            mergedRegionSize = settings.MergedRegionSize,
            partitioning = settings.Partitioning,
            filterLowHangingObstacles = settings.FilterLowHangingObstacles,
            filterLedgeSpans = settings.FilterLedgeSpans,
            filterWalkableLowHeightSpans = settings.FilterWalkableLowHeightSpans,
            edgeMaxLen = settings.EdgeMaxLen,
            edgeMaxError = settings.EdgeMaxError,
            vertsPerPoly = settings.VertsPerPoly,
            detailSampleDist = settings.DetailSampleDist,
            detailSampleMaxError = settings.DetailSampleMaxError,
            tiled = settings.Tiled,
            tileSize = settings.TileSize,

        };
    }

    internal static DtDynamicNavMeshConfig CreateDynamicRecastSettings(this BuildSettings settings)
    {
        return new DtDynamicNavMeshConfig(settings.Tiled, settings.TileSize, settings.TileSize, settings.CellSize)
        {
            walkableHeight = settings.AgentHeight,
            walkableRadius = settings.AgentRadius,
            walkableClimb = settings.AgentMaxClimb,
            walkableSlopeAngle = settings.AgentMaxSlope,
            maxSimplificationError = settings.EdgeMaxError,
            maxEdgeLen = settings.EdgeMaxLen,
            minRegionArea = settings.MinRegionSize,
            regionMergeArea = settings.MergedRegionSize,
            buildDetailMesh = true,
            detailSampleDistance = settings.DetailSampleDist,
            detailSampleMaxError = settings.DetailSampleMaxError,
            filterLowHangingObstacles = settings.FilterLowHangingObstacles,
            filterLedgeSpans = settings.FilterLedgeSpans,
            filterWalkableLowHeightSpans = settings.FilterWalkableLowHeightSpans,
            vertsPerPoly = settings.VertsPerPoly,
        };
    }
}
