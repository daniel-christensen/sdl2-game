using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using LayeredSDL2Demo.Entities;
using LayeredSDL2Demo.Interfaces;
using LayeredSDL2Demo.Helpers;

namespace LayeredSDL2Demo;

// Tech demo demonstrating the ability to create entirely transparent windows whilst still allowing
// textures to be drawn to the screen, overlapping general windows functionality.
// Pokemon Sprites: https://www.pokencyclopedia.info/en/index.php?id=sprites/gen4/spr_platinum
// Pokemon Sounds: https://play.pokemonshowdown.com/audio/cries/

// Current limitations:
// 1. Access to WIN32 API transforms the project into windows only (unsure what version)
//    Most likely win8 due to inability to create layered child windows prior to win8.
// 2. Window must be borderless when having a layered style. I don't know the specifics, but the surfaces
//    used to draw the sprites become bugged, as if their Y coordinate was shifted down per surface. Despite seeing
//    a pixel, the window doesn't treat it as an actual window pixel so you end up clicking through sprites.
//    The amount of Y pixels shifted seems to match the height of the standard windows mini/max/close border.

internal class Program
{
    internal static void Main(string[] args)
    {
        // --------------- INITIALISE COMPONENTS ---------------
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0) throw new Exception(SDL_GetError());
        if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0) throw new Exception(SDL_GetError());
        if (IMG_Init(IMG_InitFlags.IMG_INIT_PNG) != 2) throw new Exception(SDL_GetError());

        SDL_Rect desktopAreaWithoutTaskBar = LayeredWindowHelper.GetWorkArea();
        IntPtr window = SDL_CreateWindow("Transparent Window", 0, 0, desktopAreaWithoutTaskBar.w, desktopAreaWithoutTaskBar.h, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);// | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP);

        // During the window initiailisation phase, the user will see a split-second flash.
        // This "flicker" is caused by the window starting with a background colour, and then being turned transparent.
        // So, we hide the window as soon as it is created. We'll show it once initialisation is done.
        // Creating window with SDL_WindowFlags.SDL_WINDOW_HIDDEN doesn't seem to do anything.
        SDL_HideWindow(window);

        // Accesses the WIN32 API to set our SDL2 window to a Layered Extended Windows Style
        LayeredWindowHelper.SetWindowExStyleLayered(window);

        IntPtr renderer = SDL_CreateRenderer(window, -1, 0);

        // As mentioned on https://github.com/flibitijibibo/SDL2-CS
        SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

        // Allows window focus click to also act as a mouse event
        SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");

        // Show window now complete
        SDL_ShowWindow(window);

        // ------------------ PREPARE ENTITES ------------------
        EntityManager entityManager = new EntityManager();

        Player player = new Player();

        //IEntity myCharizard = new Charizard(player, 0, 0, 200, 200).LoadTextures(renderer).LoadSounds().CreateContentRect();
        IEntity myCharmeleon = new Charmeleon(player, 500, 0, 160, 160).LoadTextures(renderer).LoadSounds().CreateContentRect();
        //IEntity myCharmander = new Charmander(player, 0, 0, 80, 80).LoadTextures(renderer).LoadSounds().CreateContentRect();

        //entityManager.Add(myCharizard);
        entityManager.Add(myCharmeleon);
        //entityManager.Add(myCharmander);

        // --------------------- GAME LOOP ---------------------
        try
        {
            bool run = true;
            SDL_Event sdlEvent;
            while (run)
            {
                // Read current SDL2 event
                SDL_PollEvent(out sdlEvent);

                // Quit if sdl2 event is to quit
                if (sdlEvent.type == SDL_EventType.SDL_QUIT)
                    run = false;

                // This colour code matches the COLORREF assigned in the Layered Window Attributes
                // Beacuse of this, the pixels of this colour are turned transparent
                SDL_SetRenderDrawColor(renderer, 255, 0, 255, 0);
                SDL_RenderClear(renderer);

                // Poll and Draw all entites
                player.PollEvent(sdlEvent);
                entityManager.PollEvents(sdlEvent);
                entityManager.UpdateLogic();
                entityManager.Draw(renderer);

                // Present all data in renderer buffer
                SDL_RenderPresent(renderer);
            }
        }
        finally
        {
            entityManager.CleanUp();
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            Mix_Quit();
            IMG_Quit();
            SDL_Quit();
        }
    }
}