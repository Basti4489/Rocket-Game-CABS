using System;
using System.Text;
using System.Threading;
using System.Media;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using static System.Net.WebRequestMethods;
using System.Net.WebSockets;
using System.Dynamic;
using System.Runtime.Serialization;

namespace Rocket_Game_CABS
{
    internal class Program
    {
        static int width = 75;
        static int height = 30;
        static int windowWidth;
        static int windowHeight;
        static Random random = new();
        static string[,] scene;
        static int score = 0;
        static int rocketPosition;
        static int rocketVelocity;
        static bool gameRunning;
        static bool keepPlaying = true;
        static bool consoleSizeError = false;
        static int previousRoadUpdate = 0;
        static string unicode = "\uD83D\uDE80";
        static string obstacle = "\u2730";
        static int delay;
        static string filePath;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.CursorVisible = false;

            PrintGame();
            Getdifficulty();
            GetMusicLevel();
            PlayMusic(filePath);
            LaunchScreen();
            Console.Clear();

            while (keepPlaying)
            {
                InitializeScene();
                while (gameRunning)
                {
                    if (Console.WindowHeight < height || Console.WindowWidth < width)
                    {
                        consoleSizeError = true;
                        keepPlaying = false;
                        break;
                    }
                    HandleInput();
                    Update();
                    Render();
                    if (gameRunning)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(delay));
                    }
                }
                if (keepPlaying)
                {
                    GameOverScreen();
                }
            }
            Console.Clear();
            if (consoleSizeError)
            {
                Console.WriteLine("Console/Terminal window is too small.");
                Console.WriteLine($"Minimum size is {width} width x {height} height.");
                Console.WriteLine("Increase the size of the console window.");
            }
            Console.WriteLine("Drive was closed.");
        }
        static void PlayMusic(string filpath)
        {
            SoundPlayer soundPlayer = new SoundPlayer(filpath);
            soundPlayer.PlayLooping();
        }
        static void PrintGame()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("\n\n\n                 □□□□□□□□□     □□□□□□□□     □□□□□□□    □□     □□   □□□□□□□□□□   □□□□□□□□□□□□");
            Console.WriteLine("                 □□      □□   □        □   □       □   □□    □□    □□                □□");
            Console.WriteLine("                 □□      □□   □        □   □           □□  □□      □□                □□");
            Console.WriteLine("                 □□□□□□□□□    □        □   □           □□□□□□      □□□□□□□□□□        □□");
            Console.WriteLine("                 □□    □□     □        □   □           □□□□□□      □□□□□□□□□□        □□");
            Console.WriteLine("                 □□     □□    □        □   □           □□   □□     □□                □□");
            Console.WriteLine("                 □□      □□   □        □   □       □   □□    □□    □□                □□");
            Console.WriteLine("                 □□       □□   □□□□□□□□     □□□□□□□    □□     □□   □□□□□□□□□□        □□");
            Console.WriteLine();
            Console.WriteLine("                            □□□□□□□□□          □□        □□□       □□□   □□□□□□□□□□");
            Console.WriteLine("                           □□       □□        □□□□       □□□□     □□□□   □□ ");
            Console.WriteLine("                           □□                □□  □□      □□ □□   □□ □□   □□");
            Console.WriteLine("                           □□    □□□□       □□□□□□□□     □□  □□ □□  □□   □□□□□□□□□□");
            Console.WriteLine("                           □□       □□     □□      □□    □□   □□□   □□   □□");
            Console.WriteLine("                           □□       □□    □□        □□   □□         □□   □□");
            Console.WriteLine("                            □□□□□□□□□    □□          □□  □□         □□   □□□□□□□□□□");
            Console.WriteLine();
            Console.WriteLine("                                          by Haider Sebastian");
            Console.WriteLine("");
            Console.WriteLine("                                          Weiter = Leertaste");

            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
        enum music
        {
            lever1,
            lever2,
            lever3
        }

        static void GetMusicLevel()
        {
            music choosenMusic = music.lever1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n                                               Musiklevel auswählen:");
                Console.WriteLine();

                foreach (music music in Enum.GetValues(typeof(music)))
                {
                    if (music == choosenMusic)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"                                                      {music}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"                                                      {music}");
                    }

                    if (choosenMusic == music.lever1)
                    {
                        filePath = "musikDatei.wav";
                    }
                    else if (choosenMusic == music.lever2)
                    {
                        filePath = "Geometry Dash Level 2.wav";
                    }
                    else if (choosenMusic == music.lever3)
                    {
                        filePath = "DRY Out.wav";
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        choosenMusic = DekrementiereMusicLevel(choosenMusic);
                        break;
                    case ConsoleKey.DownArrow:
                        choosenMusic = InkrementiereMusicLevel(choosenMusic);
                        break;
                    case ConsoleKey.Enter:
                        Console.ReadKey();
                        return;
                }
            }
        }
        static music InkrementiereMusicLevel(music music)
        {
            int musicIndex = (int)music;
            musicIndex = (musicIndex + 1) % Enum.GetNames(typeof(music)).Length;
            return (music)musicIndex;
        }

        static music DekrementiereMusicLevel(music music)
        {
            int musicIndex = (int)music;
            musicIndex = (musicIndex - 1 + Enum.GetNames(typeof(music)).Length) % Enum.GetNames(typeof(music)).Length;
            return (music)musicIndex;
        }
        static void Getdifficulty()
        {
            difficultyLevel choosenMod = difficultyLevel.Einfach;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\n\n\n\n\n\n\n                                           Schwierigkeitsmodus auswählen:");
                Console.WriteLine();

                foreach (difficultyLevel modus in Enum.GetValues(typeof(difficultyLevel)))
                {
                    if (modus == choosenMod)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"                                                      {modus}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"                                                      {modus}");
                    }

                    if (choosenMod == difficultyLevel.Schwer)
                    {
                        delay = 33;
                    }
                    else if (choosenMod == difficultyLevel.Einfach)
                    {
                        delay = 100;
                    }
                    else if (choosenMod == difficultyLevel.Mittel)
                    {
                        delay = 66;
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        choosenMod = DekrementiereSchwierigkeitsmodus(choosenMod);
                        break;
                    case ConsoleKey.DownArrow:
                        choosenMod = InkrementiereSchwierigkeitsmodus(choosenMod);
                        break;
                    case ConsoleKey.Enter:
                        Console.ReadKey();
                        return;
                }
            }
        }
        static void LaunchScreen()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Clear();
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");                    
            Console.WriteLine();
            Console.WriteLine("                                  Mit A und D oder Pfeiltaste Rakete bewegen");
            Console.WriteLine();
            Console.Write("                                              Press [enter] to start...");
            PressEnterToContinue();

            Console.ForegroundColor = ConsoleColor.White;
        }
        static void InitializeScene()
        {
            const int roadWidth = 10;
            gameRunning = true;
            rocketPosition = width / 2;
            rocketVelocity = 0;
            int leftEdge = (width - roadWidth) / 2;
            int rightEdge = leftEdge + roadWidth + 1;
            scene = new string[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j < leftEdge || j > rightEdge)
                    {
                        scene[i, j] = obstacle;
                    }
                    else
                    {
                        scene[i, j] = " ";
                    }
                }
            }
        }
        static difficultyLevel InkrementiereSchwierigkeitsmodus(difficultyLevel modus)
        {
            int modusIndex = (int)modus;
            modusIndex = (modusIndex + 1) % Enum.GetNames(typeof(difficultyLevel)).Length;
            return (difficultyLevel)modusIndex;
        }

        static difficultyLevel DekrementiereSchwierigkeitsmodus(difficultyLevel modus)
        {
            int modusIndex = (int)modus;
            modusIndex = (modusIndex - 1 + Enum.GetNames(typeof(difficultyLevel)).Length) % Enum.GetNames(typeof(difficultyLevel)).Length;
            return (difficultyLevel)modusIndex;
        }
    
        enum difficultyLevel
        {
            Einfach,
            Mittel,
            Schwer
        }
        static void Initialize()
        {
            windowWidth = Console.WindowWidth;
            windowHeight = Console.WindowHeight;
            if (OperatingSystem.IsWindows())
            {
                if (windowWidth < width && OperatingSystem.IsWindows())
                {
                    windowWidth = Console.WindowWidth = width + 1;
                }
                if (windowHeight < height && OperatingSystem.IsWindows())
                {
                    windowHeight = Console.WindowHeight = height + 1;
                }
                Console.BufferWidth = windowWidth;
                Console.BufferHeight = windowHeight;
            }
        }
        static void Render()
        {
            StringBuilder stringBuilder = new(width * height);
            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 1 && j == rocketPosition)
                    {
                        stringBuilder.Append(
                            !gameRunning ? unicode :
                            rocketVelocity < 0 ? unicode :
                            rocketVelocity > 0 ? unicode :
                            unicode);
                    }
                    else
                    {
                        stringBuilder.Append(scene[i, j]);
                    }
                }
                if (i > 0)
                {
                    stringBuilder.AppendLine();
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(stringBuilder);
        }
        static void GameOverScreen()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n\n\n\n                           ██████╗  █████╗ ███╗   ███╗███████╗     ██████╗ ██╗   ██╗███████╗██████╗ ");
            Console.WriteLine("                          ██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ██╔═══██╗██║   ██║██╔════╝██╔══██╗");
            Console.WriteLine("                          ██║  ███╗███████║██╔████╔██║█████╗      ██║   ██║██║   ██║█████╗  ██████╔╝");
            Console.WriteLine("                          ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝      ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗");
            Console.WriteLine("                          ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗    ╚██████╔╝ ╚████╔╝ ███████╗██║  ██║");
            Console.WriteLine("                           ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"                                   Du hast verloren. Dein Score liegt bei {score} Punkten");
            Console.WriteLine($"                                                    Play Again (Y/N)?");
        GetInput:
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                
                case ConsoleKey.Y:
                    Console.Clear();
                    keepPlaying = true;
                    break;
                case ConsoleKey.N or ConsoleKey.Escape:
                    keepPlaying = false;
                    Console.Clear();
                    break;
                default:
                    goto GetInput;
            }
        }
        static void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A or ConsoleKey.LeftArrow:
                        rocketVelocity = -1;
                        break;
                    case ConsoleKey.D or ConsoleKey.RightArrow:
                        rocketVelocity = +1;
                        break;
                    case ConsoleKey.Escape:
                        gameRunning = false;
                        keepPlaying = false;
                        break;
                    case ConsoleKey.Enter:
                        Console.ReadLine();
                        break;
                }
            }
        }
        static void Update()
        {
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    scene[i, j] = scene[i + 1, j];
                }
            }
            int roadUpdate =
                random.Next(5) < 4 ? previousRoadUpdate :
                random.Next(3) - 1;
            if (roadUpdate is -1 && scene[height - 1, 0] is " ") roadUpdate = 1;
            if (roadUpdate is 1 && scene[height - 1, width - 1] is " ") roadUpdate = -1;
            switch (roadUpdate)
            {
                case -1: // left
                    for (int i = 0; i < width - 1; i++)
                    {
                        scene[height - 1, i] = scene[height - 1, i + 1];
                    }
                    scene[height - 1, width - 1] = obstacle;
                    break;
                case 1: // right
                    for (int i = width - 1; i > 0; i--)
                    {
                        scene[height - 1, i] = scene[height - 1, i - 1];
                    }
                    scene[height - 1, 0] = obstacle;
                    break;
            }
            previousRoadUpdate = roadUpdate;
            rocketPosition += rocketVelocity;
            if (rocketPosition < 0 || rocketPosition >= width || scene[1, rocketPosition] is not " ")
            {
                gameRunning = false;
            }
            score++;
        }
        static void PressEnterToContinue()
        {
        GetInput:
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Escape:
                    keepPlaying = false;
                    break;
                default: goto GetInput;
            }
        }
    }
}