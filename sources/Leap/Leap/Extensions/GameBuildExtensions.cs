using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Audio;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Builder;
using Stride.Engine.Processors;
using Stride.Graphics.Font;
using Stride.Profiling;
using Stride.Rendering;
using Stride.Rendering.Fonts;
using Stride.Rendering.Sprites;
using Stride.Streaming;

namespace Leap.Extensions;
public static class GameBuildExtensions
{
    public static IGameBuilder UseDefaultGameSystems(this IGameBuilder gameBuilder)
    {
        var services = gameBuilder.Services[typeof(IServiceRegistry)] as IServiceRegistry;

        var scriptSystem = new ScriptSystem(services);
        var sceneSystem = new SceneSystem(services);
        var audioSystem = new AudioSystem(services);
        var gameFontSystem = new GameFontSystem(services);
        var spriteAnimationSystem = new SpriteAnimationSystem(services);
        var debugTextSystem = new DebugTextSystem(services);
        var gameProfilingSystem = new GameProfilingSystem(services);
        var inputSystem = new InputSystem(services);
        var effectSystem = new EffectSystem(services);
        // registers itself as a service
        var streamingManager = new StreamingManager(services);

        gameBuilder
            .AddGameSystem(scriptSystem)
            .AddGameSystem(sceneSystem)
            .AddGameSystem(audioSystem)
            .AddGameSystem(gameFontSystem)
            .AddGameSystem(spriteAnimationSystem)
            .AddGameSystem(debugTextSystem)
            .AddGameSystem(gameProfilingSystem)
            .AddGameSystem(inputSystem)
            .AddGameSystem(effectSystem)
            .AddGameSystem(streamingManager);

        // add services
        gameBuilder
            .AddService(scriptSystem)
            .AddService(sceneSystem)
            .AddService(spriteAnimationSystem)
            .AddService(debugTextSystem)
            .AddService(gameProfilingSystem)
            .AddService(inputSystem)
            .AddService(effectSystem)
            .AddService(inputSystem.Manager)
            .AddService(audioSystem)
            .AddService<IAudioEngineProvider>(audioSystem)
            .AddService(gameFontSystem)
            .AddService(gameFontSystem.FontSystem)
            .AddService<IFontFactory>(gameFontSystem.FontSystem);

        return gameBuilder;
    }

    public static IGameBuilder UseDefaultGameSystemsDI(this IGameBuilder gameBuilder)
    {
        gameBuilder
            .AddService<ScriptSystem>()
            .AddService<SceneSystem>()
            .AddService<SpriteAnimationSystem>()
            .AddService<DebugTextSystem>()
            .AddService<GameProfilingSystem>()
            .AddService<EffectSystem>()
            .AddService<StreamingManager>()
            .AddService<IAudioEngineProvider, AudioSystem>();

        return gameBuilder;
    }


}
