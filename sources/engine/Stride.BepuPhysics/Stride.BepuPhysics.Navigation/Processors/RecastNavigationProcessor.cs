using Stride.BepuPhysics.Definitions;
using Stride.BepuPhysics.Navigation.Components;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Engine;
using Stride.Games;
using System.Collections.Concurrent;

namespace Stride.BepuPhysics.Navigation.Processors;

public sealed class RecastNavigationProcessor : EntityProcessor<RecastNavigationComponent>
{
    private RecastMeshProcessor _recastMeshProcessor;
    private readonly List<RecastNavigationComponent> _components = new();
    private readonly ConcurrentQueue<RecastNavigationComponent> _tryGetPathQueue = new();

    public RecastNavigationProcessor()
    {
        //run after the RecastMeshProcessor
        Order = 20001;
        _recastMeshProcessor = null!; // Initialized below
    }

    protected override void OnSystemAdd()
    {
        ServicesHelper.LoadBepuServices(Services, out _, out _, out _);
        if (Services.GetService<RecastMeshProcessor>() is { } recastMeshProcessor)
        {
            _recastMeshProcessor = recastMeshProcessor;
        }
        else
        {
            // add the RecastMeshProcessor if it doesn't exist
            _recastMeshProcessor = new RecastMeshProcessor(Services);
            // add to the Scenes processors
            var sceneSystem = Services.GetSafeServiceAs<SceneSystem>();
            sceneSystem.Game!.GameSystems.Add(_recastMeshProcessor);
        }

        Services.AddService(this);
    }

    protected override void OnEntityComponentAdding(Entity entity, RecastNavigationComponent component, RecastNavigationComponent data)
    {
        _components.Add(component);
    }

    protected override void OnEntityComponentRemoved(Entity entity, RecastNavigationComponent component, RecastNavigationComponent data)
    {
        _components.Remove(component);
    }

    public override void Update(GameTime time)
    {
        var deltaTime = (float)time.Elapsed.TotalSeconds;

        for (int i = 0; i < 10; i++)
        {
            if (_tryGetPathQueue.IsEmpty) break;

            if (_tryGetPathQueue.TryDequeue(out var pathfinding))
            {
                // cannot use dispatcher here because of the TryFindPath method.
                SetNewPath(pathfinding);
            }
        }

        Dispatcher.For(0, _components.Count, i =>
        {
            var component = _components[i];

            if(component.State == NavigationState.QueuePathPlanning)
            {
                _tryGetPathQueue.Enqueue(component);
                component.State = NavigationState.PlanningPath;
            }

            // This allows the agent to move towards the target even if a path is being planned.
            // This allows  the user to determine if the agent should stop moving if the path is no longer valid.
            if (component.IsMoving)
            {
                Move(component, deltaTime);
                Rotate(component);
            }
        });
    }

    public bool SetNewPath(RecastNavigationComponent pathfinder)
    {
        if (_recastMeshProcessor.TryFindPath(pathfinder.Entity.Transform.WorldMatrix.TranslationVector, pathfinder.Target, ref pathfinder.Polys, ref pathfinder.Path))
        {
            pathfinder.State = NavigationState.PathIsReady;
            return true;
        }
        return false;
    }

    private void Move(RecastNavigationComponent pathfinder, float deltaTime)
    {
        if (pathfinder.Path.Count == 0)
        {
            pathfinder.State = NavigationState.PathIsInvalid;
            return;
        }

        var position = pathfinder.Entity.Transform.WorldMatrix.TranslationVector;

        var nextWaypointPosition = pathfinder.Path[0];
        var distanceToWaypoint = Vector3.Distance(position, nextWaypointPosition);

        // When the distance between the character and the next waypoint is large enough, move closer to the waypoint
        if (distanceToWaypoint > 0.1)
        {
            var direction = nextWaypointPosition - position;
            direction.Normalize();
            direction *= pathfinder.Speed * deltaTime;

            position += direction;
        }
        else
        {
            if (pathfinder.Path.Count > 0)
            {
                // need to test if storing the index in Pathfinder would be faster than this.
                pathfinder.Path.RemoveAt(0);
            }
        }

        pathfinder.Entity.Transform.Position = position;
    }

    private void Rotate(RecastNavigationComponent pathfinder)
    {
        if (pathfinder.Path.Count == 0)
        {
            return;
        }
        var position = pathfinder.Entity.Transform.WorldMatrix.TranslationVector;

        float angle = (float)Math.Atan2(pathfinder.Path[0].Z - position.Z,
            pathfinder.Path[0].X - position.X);

        pathfinder.Entity.Transform.Rotation = Quaternion.RotationY(-angle);
    }
}
