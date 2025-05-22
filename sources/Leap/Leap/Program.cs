using Leap.Extensions;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
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
    .UseDefaultDb()
    .UseDefaultContentManager()
    .SetGameContext(GameContextFactory.NewGameContextSDL())
    .AddLogListener(new ConsoleLogListener());

var game = gameBuilder.Build();



//var content = game.Services.GetService<IContentManager>();
//var settings = content.Load<GameSettings>("GameSettings");
//
//var sceneSystem = game.Services.GetService<SceneSystem>();
//sceneSystem.InitialSceneUrl = settings.DefaultSceneUrl;
//sceneSystem.InitialGraphicsCompositorUrl = settings.DefaultGraphicsCompositorUrl;

var fileProvider = game.Services.GetService<IDatabaseFileProviderService>().FileProvider;
game.Services.GetService<EffectSystem>()
    .CreateDefaultEffectCompiler(fileProvider);

game.SetupBase3DScene();
game.AddSkybox();

var entity = game.Create3DPrimitive(PrimitiveModelType.Capsule);

entity.Transform.Position = new Vector3(0, 8, 0);

entity.Scene = rootScene;

game.Run();
