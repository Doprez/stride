using Doprez.Stride.Input;
using GameBuilderTest;
using Stride.Core.Diagnostics;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Engine.Builder;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Rendering;

var gameBuilder = GameBuilder.Create();

gameBuilder.UseDefaultGameSystemsDI()
    .UseStrideInput()
    .UseStrideFontSystem()
    .UseDefaultDb()
    .UseDefaultContentManager()
    .SetGameContext(GameContextFactory.NewGameContextSDL())
    //.AddStrideInputSource(new SharpHookInputSource())
    //.AddStrideInputSource(new HIDDeviceInputSource())
    .AddLogListener(new ConsoleLogListener());

var game = gameBuilder.Build();

var content = game.Services.GetService<IContentManager>();
var settings = content.Load<GameSettings>("GameSettings");

var sceneSystem = game.Services.GetService<SceneSystem>();
sceneSystem.InitialSceneUrl = settings.DefaultSceneUrl;
sceneSystem.InitialGraphicsCompositorUrl = settings.DefaultGraphicsCompositorUrl;

var fileProvider = game.Services.GetService<IDatabaseFileProviderService>().FileProvider;
game.Services.GetService<EffectSystem>()
    .CreateDefaultEffectCompiler(fileProvider);

game.Run();
