using TestApp.Helper;
using TestApp.Messages;
using Serilog;
using TestApp.Components;
using TestApp.Registry;
using TestApp.Interfaces;
using TestApp.Collections;
using TestApp.Entities;

namespace TestApp
{
    internal class Game
    {
        internal Configuration Configuration { get; }
            
        internal bool IsRunning { get; set; }

        private ComponentDictionary Components { get; }

        internal IEntity Player { get; }

        private Game()
        {
            if (!ConsoleManager.IsConsoleAllocated())
                Log.Information(GameLoggingMessage.GameInstanceWithoutConsole);

            // REGISTRATIONS
            RegistryManager.Register<IComponent>(RegistryKey.Component, ComponentKey.EventPoller, typeof(EventPoller));
            RegistryManager.Register<IComponent>(RegistryKey.Component, ComponentKey.Logic, typeof(Logic));
            RegistryManager.Register<IComponent>(RegistryKey.Component, ComponentKey.Graphics, typeof(Graphics));
            RegistryManager.Register<IEntity>(RegistryKey.Entity, EntityKey.Player, typeof(Player));

            // ESSENTIAL MANAGER & MISC INITIALISATIONS
            IsRunning = true;
            Configuration = new Configuration();
            Components = new ComponentDictionary
            {
                { ComponentKey.EventPoller, RegistryManager.CreateInstance<IComponent>(RegistryKey.Component, ComponentKey.EventPoller, this) },
                { ComponentKey.Logic, RegistryManager.CreateInstance<IComponent>(RegistryKey.Component, ComponentKey.Logic, this) },
                { ComponentKey.Graphics, RegistryManager.CreateInstance<IComponent>(RegistryKey.Component, ComponentKey.Graphics, this) }
            };

            Player = RegistryManager.CreateInstance<IEntity>(RegistryKey.Entity, EntityKey.Player, this, 0, 0, true, true);
        }

        private void Run()
        {
            try
            {
                Initialise();
                while (IsRunning) Update();
            }
            catch (Exception e)
            {
                Log.Fatal(GameExceptionMessage.FatalUncaught);
                Log.Fatal(e.Message);
            }
            finally
            {
                CleanUp();
            }
        }

        private void Initialise()
        {
            Components.Initialise();
        }

        private void Update()
        {
            Components.Update();
        }

        private void CleanUp()
        {
            Components.CleanUp();
        }

        internal static void NewGame()
        {
            new Game().Run();
        }
    }
}
