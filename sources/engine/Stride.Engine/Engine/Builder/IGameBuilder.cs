using System.Collections.Generic;
using Stride.Core.Diagnostics;
using Stride.Games;
using System;
using Microsoft.Extensions.DependencyInjection;
using Stride.Input;
using Stride.Core.IO;

namespace Stride.Engine.Builder;
public interface IGameBuilder
{
    Dictionary<Type, object> Services { get; }

    IServiceCollection DiServices { get; }

    GameSystemCollection GameSystems { get; }

    List<LogListener> LogListeners { get; }

    List<IInputSource> InputSources { get; }

    DatabaseFileProvider DatabaseFileProvider { get; set; }

    GameBase Game { get; }

    GameContext Context { get; }
}
