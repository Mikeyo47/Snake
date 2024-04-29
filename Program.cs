using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static readonly Random _random = new();
    static bool shouldExit = false;
    static readonly int _height = Console.WindowHeight - 1;
    static readonly int _width = Console.WindowWidth - 1;

    static string exitMessage = "Thanks for playing! Press enter to exit.";

    static int snakeY;
    static int snakeX;

    static readonly List<int[]> snakeBody = new();
    static readonly char _snake = 'o';
    static char snakeHead = 'o';
    static int snakeLength;
    static char snakeDirection;
    static int speed;

    static readonly char _food = '*';
    static int foodX;
    static int foodY;

    static async Task Main(string[] args)
    {
        Console.CursorVisible = false;
        InitializeGame();

        while (!shouldExit)
        {
            if (TerminalResized())
            {
                Console.Clear();
                exitMessage = "Console was resized. Press enter to exit.";
                shouldExit = true;
            }

            if (Console.KeyAvailable)
            {
                ProcessKey(Console.ReadKey(true).Key);
            }

            MoveSnake();
            await Task.Delay(1000 / speed);
        }

        await ExitLoop();
        Console.WriteLine(exitMessage);
        Console.ReadKey();
    }

    static void ProcessKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                snakeDirection = 'U';
                break;
            case ConsoleKey.DownArrow:
                snakeDirection = 'D';
                break;
            case ConsoleKey.LeftArrow:
                snakeDirection = 'L';
                break;
            case ConsoleKey.RightArrow:
                snakeDirection = 'R';
                break;
            case ConsoleKey.Escape:
                shouldExit = true;
                break;
        }
    }

    static void MoveSnake()
    {
        snakeX = snakeBody[snakeLength - 1][0];
        snakeY = snakeBody[snakeLength - 1][1];
        snakeHead = 'o';

        switch (snakeDirection)
        {
            case 'U':
                snakeY--;
                break;
            case 'D':
                snakeY++;
                break;
            case 'L':
                snakeX--;
                break;
            case 'R':
                snakeX++;
                break;
        }

        if (OutOfBounds())
        {
            Console.Clear();
            exitMessage = "You hit the wall. Press enter to exit.";
            shouldExit = true;
        }
        else if (HitItself())
        {
            Console.Clear();
            exitMessage = "You hit yourself. Press enter to exit.";
            shouldExit = true;
        }
        else
        {
            NearFood();
            if (GotFood())
            {
                snakeLength++;
                speed++;
                GenerateFood();
                snakeHead = 'O';
            }

            snakeBody.Add([snakeX, snakeY]);

            if (snakeBody.Count > snakeLength)
            {
                snakeBody.RemoveAt(0);
            }

            Console.Clear();
            ShowFood();
            DisplaySnake();
        }
    }

    static bool TerminalResized()
    {
        return _height != Console.WindowHeight - 1 || _width != Console.WindowWidth - 1;
    }

    static void GenerateFood()
    {
        foodX = _random.Next(0, _width);
        foodY = _random.Next(0, _height);
    }

    static void ShowFood()
    {
        Console.SetCursorPosition(foodX, foodY);
        Console.Write(_food);
    }

    static bool GotFood()
    {
        return snakeY == foodY && snakeX == foodX;
    }

    static void NearFood()
    {
        if (snakeY == foodY && snakeX == foodX - 1)
            snakeHead = '<';
        else if (snakeY == foodY && snakeX == foodX + 1)
            snakeHead = '>';
        else if (snakeY == foodY - 1 && snakeX == foodX)
            snakeHead = 'Λ';
        else if (snakeY == foodY + 1 && snakeX == foodX)
            snakeHead = 'V';
    }

    static bool OutOfBounds()
    {
        if (snakeX < 0) return true;
        else if (snakeX > _width) return true;
        else if (snakeY < 0) return true;
        else if (snakeY > _height) return true;
        else return false;
    }

    static bool HitItself()
    {
        return snakeBody.Any(x => x[0] == snakeX && x[1] == snakeY);
    }
    static void DisplaySnake()
    {
        Console.SetCursorPosition(snakeBody[snakeLength - 1][0], snakeBody[snakeLength - 1][1]);
        Console.Write(snakeHead);
        for (int i = 0; i < snakeLength - 1; i++)
        {
            Console.SetCursorPosition(snakeBody[i][0], snakeBody[i][1]);
            Console.Write(_snake);
        }
    }

    static async Task ExitLoop()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.Clear();
            DisplaySnake();
            await Task.Delay(150);
            Console.Clear();
            await Task.Delay(150);
        }
    }

    static void InitializeGame()
    {
        snakeLength = 3;

        int startSnakeX = snakeLength - 1;
        int startSnakeY = Console.WindowHeight / 2;

        speed = 4;
        snakeDirection = 'R';

        for (int i = snakeLength - 1; i >= 0; i--)
        {
            snakeBody.Add([startSnakeX - i, startSnakeY]);
        }

        Console.Clear();
        GenerateFood();
        ShowFood();
        DisplaySnake();
    }
}