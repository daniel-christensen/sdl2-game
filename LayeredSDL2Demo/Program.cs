using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using Vanara.PInvoke;
using System.Runtime.InteropServices;
using LayeredSDL2Demo.Entities;
using LayeredSDL2Demo.Interfaces;

namespace LayeredSDL2Demo;

// Tech demo demonstrating the ability to create entirely transparent windows whilst still allowing
// textures to be drawn to the screen, overlapping general windows functionality.
// Pokemon Sprites: https://www.pokencyclopedia.info/en/index.php?id=sprites/gen4/spr_platinum
// Pokemon Sounds: https://play.pokemonshowdown.com/audio/cries/

// Current limitations:
// 1. Access to WIN32 API transforms the project into windows only (unsure what version)
//    Most likely win8 due to inability to create layered child windows prior to win8.

internal class Program
{
    internal static void Main(string[] args)
    {
        // --------------- INITIALISE COMPONENTS ---------------
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0) throw new Exception(SDL_GetError());
        if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0) throw new Exception(SDL_GetError());
        if (IMG_Init(IMG_InitFlags.IMG_INIT_PNG) != 2) throw new Exception(SDL_GetError());

        SDL_Rect desktopAreaWithoutTaskBar = GetWorkArea();
        IntPtr window = SDL_CreateWindow("Transparent Window", 0, 0, desktopAreaWithoutTaskBar.w, desktopAreaWithoutTaskBar.h, SDL_WindowFlags.SDL_WINDOW_BORDERLESS | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP);

        // During the window initiailisation phase, the user will see a split-second flash.
        // This "flicker" is caused by the window starting with a background colour, and then being turned transparent.
        // So, we hide the window as soon as it is created. We'll show it once initialisation is done.
        // Creating window with SDL_WindowFlags.SDL_WINDOW_HIDDEN doesn't seem to do anything.
        SDL_HideWindow(window);

        // Toggling flags can give creater controls rather than setting via initialisation
        // HOWEVER, for some reason calling these here instead of passing the flags into the initial window creation
        // returns the "flickering" effect.
        // Toggles the SDL_WindowFlags.SDL_WINDOW_BORDERLESS flag for the window
        //SDL_SetWindowBordered(window, SDL_bool.SDL_FALSE);

        // Toggles the SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP flag for the window
        //SDL_SetWindowAlwaysOnTop(window, SDL_bool.SDL_TRUE);

        // Accesses the WIN32 API to set our SDL2 window to a Layered Extended Windows Style
        SetWindowExStyleLayered(window);

        IntPtr renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // Set Hints
        // Firstly, as mentioned on https://github.com/flibitijibibo/SDL2-CS
        SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

        // Allows window focus click to also act as a mouse event
        SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");

        // Show window now complete
        SDL_ShowWindow(window);

        // ------------------ PREPARE ENTITES ------------------
        //EntityManager entityManager = new EntityManager();

        IEntity myCharizard = new Charizard(0, 0, 200, 200).LoadTextures(renderer).LoadSounds();
        IEntity myCharmander = new Charmander(300, 300, 200, 200).LoadTextures(renderer).LoadSounds();

        //entityManager.Add(myCharizard);

        // --------------------- GAME LOOP ---------------------
        try
        {
            bool run = true;
            SDL_Event sdlEvent;
            while (run)
            {
                SDL_PollEvent(out sdlEvent);
                if (sdlEvent.type == SDL_EventType.SDL_QUIT)
                    run = false;

                // This colour code matches the COLORREF assigned in the Layered Window Attributes
                // Beacuse of this, the pixels of this colour are turned transparent
                SDL_SetRenderDrawColor(renderer, 255, 0, 255, 255);
                SDL_RenderClear(renderer);

                myCharizard.PollEvent(sdlEvent);
                myCharmander.PollEvent(sdlEvent);
                myCharizard.Draw(renderer);
                myCharmander.Draw(renderer);

                SDL_RenderPresent(renderer);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            myCharizard.CleanUp();
            myCharmander.CleanUp();
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            Mix_Quit();
            IMG_Quit();
            SDL_Quit();
        }
    }

    internal static SDL_Rect GetWorkArea()
    {
        // Create a new instance of the desired struct and Marshal a pointer for it
        var workArea = new RECT();
        IntPtr workAreaPtr = Marshal.AllocHGlobal(Marshal.SizeOf(workArea));
        Marshal.StructureToPtr(workArea, workAreaPtr, true);

        // Call WIN32 API to acquire desired information
        User32.SystemParametersInfo(User32.SPI.SPI_GETWORKAREA, 0, workAreaPtr, User32.SPIF.None);

        // Marshal the pointer back into a useable structure and free the allocated pointer memory
        workArea = Marshal.PtrToStructure<RECT>(workAreaPtr);
        Marshal.FreeHGlobal(workAreaPtr);

        // Convert struct information into a clean SDL structure
        SDL_Rect rect = new SDL_Rect
        {
            x = workArea.Left,
            y = workArea.Top,
            w = workArea.Width,
            h = workArea.Height
        };

        return rect;
    }

    internal static void SetWindowExStyleLayered(IntPtr window)
    {
        // Retrieve system information on window, including native WIN32 window handler
        SDL_SysWMinfo wmInfo = new SDL_SysWMinfo();
        SDL_VERSION(out wmInfo.version);
        SDL_GetWindowWMInfo(window, ref wmInfo);
        IntPtr hWnd = wmInfo.info.win.window;

        // Acquire the system long which describes characteristics of our window
        IntPtr dwNewLong = User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE);

        // Bit shift the long to enabled our Extended Layered Style
        // From what I understand, this is the only style that lets us create transparent backgrounds
        dwNewLong = dwNewLong | (IntPtr)User32.WindowStylesEx.WS_EX_LAYERED;

        // Now set that long back onto the same window handler with reference to our Extended Style
        User32.SetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE, dwNewLong);

        // Now that our window is set to the Extended Style of "Layered" we can now use Layered-specific methods
        // I beieve SetLayeredWindowAttributes() is one of the only methods that come with this style
        // What we are saying here is that any pixel drawn to the window matching the given COLORREF will turn transparent
        COLORREF transparentColorKey = new COLORREF(255, 0, 255);
        User32.SetLayeredWindowAttributes(hWnd, transparentColorKey, 0, User32.LayeredWindowAttributes.LWA_COLORKEY);
    }
}