
class Game
{
    public static void Main(string[] args)
    {
        Dungeon dungeon;
        Console.ForegroundColor = ConsoleColor.Magenta;
        
        Console.WriteLine("Dungeon Options:\n1) Small\n2) Medium\n3) Large\n");
        Console.WriteLine("Enter the Dungeon Size: ");
        Console.ForegroundColor = ConsoleColor.Cyan;

        int dungeonSize = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Type `help` for more info.");
        switch (dungeonSize)
        {
            case 2:
                dungeon = Dungeon.GenerateDungeon(6, 6);
                break;
            case 3:
                dungeon = Dungeon.GenerateDungeon(8, 8);
                break;
            case 1:
            default:
                dungeon = Dungeon.GenerateDungeon(4, 4);
                break;
        }
        Console.WriteLine("Welcome to the Dungeon!");
        do
        {
            dungeon.PrintCurrentRoomInfo();
            Console.Write("What do you want to do? ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string commandStr = Console.ReadLine();
            if (!dungeon.ExecuteCommand(commandStr))
            {
                Console.WriteLine("Could not perform action!");
            }

        } while (!dungeon.IsCompleted());
        dungeon.PrintCurrentRoomInfo();
        if (dungeon.CheckGameIsWon())
        {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("You Won!");

        } else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You Lost!");
        }
    }
}
class Dungeon(RoomInfo[,] roomInfo)
{
    private bool FountainEnabled { get; set; } = false;
    private RoomInfo[,] RoomList { get; set; } = roomInfo;
    private Position CurrentPosition { get; set; } = new Position( 0, 0 );

    private int ArrowCount { get; set; } = 5;
    public static RoomInfo[,] SetupAdjacents(RoomInfo[,] roomInfo, Position pos, int row, int col, string roomText, ConsoleColor color, RoomType roomType)
    {
        if(pos.X > 0)
        {
            roomInfo[pos.Y, pos.X - 1] = new RoomInfo(roomText, color, roomType);
            if (pos.Y > 0)
            {
                roomInfo[pos.Y - 1, pos.X - 1] = new RoomInfo(roomText, color, roomType);                
            }
            if (pos.Y < row - 1)
            {
                roomInfo[pos.Y + 1, pos.X - 1] = new RoomInfo(roomText, color, roomType);
            }
        }
        if (pos.X < col - 1)
        {
            roomInfo[pos.Y, pos.X + 1] = new RoomInfo(roomText, color, roomType);
            if (pos.Y > 0)
            {
                roomInfo[pos.Y - 1, pos.X + 1] = new RoomInfo(roomText, color, roomType);
            }
            if (pos.Y < row - 1)
            {
                roomInfo[pos.Y + 1, pos.X + 1] = new RoomInfo(roomText, color, roomType);
            }
        }
        
        if (pos.Y > 0)
        {
        roomInfo[pos.Y - 1, pos.X] = new RoomInfo(roomText, color, roomType);
        }
        if (pos.Y < row - 1) { 
        roomInfo[pos.Y + 1, pos.X] = new RoomInfo(roomText, color, roomType);
        }
        
        return roomInfo;
    }
    public static RoomInfo[,] SetupPits(RoomInfo[,] roomInfo, int row, int col)
    {
        int[,] pitArray;
        if(row == 4)
        {
            // 1 pit
            pitArray = new int[,] { { 2, 2 } };
        }
        else if (row == 6)
        {
            // 2 pit
            pitArray = new int[,] { { 2, 3 }, { 4, 5 } };
        }
        else 
        {
            // 4 pit
            pitArray = new int[,] { { 1, 2 }, { 3, 5 }, { 5, 7 }, { 7, 3 } };
        }
        for(int i = 0; i<pitArray.GetLength(0); i++)
        {
            (int xPos, int yPos) = (pitArray[i,0], pitArray[i,1]);
            roomInfo[yPos, xPos] = new RoomInfo("You fell into a pit!", ConsoleColor.Red, RoomType.Pit);
            roomInfo = SetupAdjacents(roomInfo, new Position(xPos, yPos), row, col, "You feel adraft. There is a pit in a nearby room.", ConsoleColor.DarkYellow, RoomType.PitAdjacent);
        }

        return roomInfo;
    }

    public static RoomInfo[,] SetupMaelstorms(RoomInfo[,] roomInfo, int row, int col)
    {
        int[,] maelstormArray;
        if (row == 4)
        {
            // 1 maelstorm
            maelstormArray = new int[,] { { 1, 3 } };
        }
        else if (row == 6)
        {
            // 2 maelstorm
            maelstormArray = new int[,] { { 3, 4} };
        }
        else
        {
            // 3 maelstorm
            maelstormArray = new int[,] { { 3, 4 }, { 5, 3 } };
        }
        for (int i = 0; i < maelstormArray.GetLength(0); i++)
        {
            (int xPos, int yPos) = (maelstormArray[i, 0], maelstormArray[i, 1]);
            roomInfo[yPos, xPos] = new RoomInfo("You encountered a Maelstorm! You were dispossesed!", ConsoleColor.DarkGray, RoomType.Maelstorm);
            roomInfo = SetupAdjacents(roomInfo, new Position(xPos, yPos), row, col, "You hear the growling and groaning of a maelstrom nearby", ConsoleColor.Gray, RoomType.MaelstormAdjacent);
        }
        return roomInfo;
    }

    public static RoomInfo[,] SetupAmaroks(RoomInfo[,] roomInfo, int row, int col)
    {
        int[,] amarokArray;
        if (row == 4)
        {
            // 1 amarok
            amarokArray = new int[,] { { 1, 3 }, };
        }
        else if (row == 6)
        {
            // 2 amarok
            amarokArray = new int[,] { { 5, 5 }, {1, 5} };

        }
        else
        {
            // 3 amarok
            amarokArray = new int[,] { { 3, 4 }, { 5, 3 }, {2, 6} };
        }
        for (int i = 0; i < amarokArray.GetLength(0); i++)
        {
            (int xPos, int yPos) = (amarokArray[i, 0], amarokArray[i, 1]);
            roomInfo[yPos, xPos] = new RoomInfo("An Amarok Appears! It violently destroys you!", ConsoleColor.Red, RoomType.Amarok);
            roomInfo = SetupAdjacents(roomInfo, new Position(xPos, yPos), row, col, "You can smell the rotten stench of an amarok in a nearby room", ConsoleColor.DarkRed, RoomType.AmarokAdjacent);
        }
        return roomInfo;
    }

    public static Dungeon GenerateDungeon(int row, int col)
    {
        RoomInfo[,] tempRoomList = new RoomInfo[row, col];
        for(int i = 0; i<row; ++i)
        {
            for(int j = 0; j<col; ++j)
            {
                tempRoomList[i, j] = new RoomInfo("", ConsoleColor.White, RoomType.Normal);
            }
        }
        
        tempRoomList = SetupPits(tempRoomList, row, col);
        tempRoomList = SetupMaelstorms(tempRoomList, row, col);
        tempRoomList = SetupAmaroks(tempRoomList, row, col);
        tempRoomList = SetupPrimaryPositions(tempRoomList);
        // Pit Positions
        // Set Pit positions

        return new Dungeon(tempRoomList);
    }
    static public RoomInfo[,] SetupPrimaryPositions(RoomInfo[,] roomList)
    {
        roomList[0, 0] = new RoomInfo("You see light coming from the cavern entrance.", ConsoleColor.Yellow, RoomType.Entrance);
        roomList[0, 2] = new RoomInfo("You hear water dripping in this room. The Fountain of Objects is here!", ConsoleColor.Blue, RoomType.Fountain);
        return roomList;
    }
    public void PrintCurrentRoomInfo()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"You are in the room at (Row={CurrentPosition.Y}, Column={CurrentPosition.X}).");
        Console.WriteLine($"You have {ArrowCount} arrows left.");
        RoomInfo roomInfo = RoomList[CurrentPosition.Y, CurrentPosition.X];
        if (roomInfo.Text != "")
        {
        Console.ForegroundColor = roomInfo.Color;
            Console.WriteLine(roomInfo.Text);
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static void PrintHelpInfo()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search\r\nof the Fountain of Objects.\r\nLight is visible only in the entrance, and no other light is seen anywhere in the caverns.\r\nYou must navigate the Caverns with your other senses.\r\nFind the Fountain of Objects, activate it, and return to the entrance.");
        Console.WriteLine("Look out for pits. You\r\nwill feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.");
        Console.WriteLine("Maelstroms are\r\nviolent forces of sentient wind. Entering a room with one could transport you to any other location\r\nin the caverns. You will be able to hear their growling and groaning in nearby rooms.");
        Console.WriteLine("Amaroks roam the\r\ncaverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.");
        Console.WriteLine("You carry with\r\nyou a bow and a quiver of arrows. You can use them to shoot monsters in the caverns but be warned:\r\nyou have a limited supply.");
        Console.WriteLine("There are 3 commands. Which you use alongside a directive");
        Console.WriteLine("Directions: east, west, north & south");
        Console.WriteLine("Ability: enable & disable");
        Console.WriteLine("Move Command: move north|south|east|west");
        Console.WriteLine("Move Command: shoot north|south|east|west");
        Console.WriteLine("");

        Console.ForegroundColor = ConsoleColor.White;

    }
    public bool ExecuteCommand(string commandStr)
    {
        string[] commandList = commandStr.Split(" ");
        if(commandList.Length == 1 && commandList[0] == "help")
        {
            PrintHelpInfo();
            return true;
        }
        else if(commandList.Length != 2)
        {
            return false;
        }
        switch (commandList[0])
        {
            case "move":
                return Move(commandList[1]);
            case "enable":
                return Action(commandList[1]);
            case "shoot":
                return Shoot(commandList[1]);
            default:
                return false;
        }
    }
    public bool Move(string positionString)
    {
        bool hasMoved = false;
        switch (positionString)
        {
            case "east":
                {
                    if (CurrentPosition.X >= RoomList.GetLength(1) - 1) break;
                    CurrentPosition = CurrentPosition with { X = CurrentPosition.X + 1 };
                    hasMoved = true;
                }
                break;
            case "west":
                {
                    if (CurrentPosition.X <= 0) break;
                    CurrentPosition = CurrentPosition with { X = CurrentPosition.X - 1 };
                    hasMoved = true;
                }
                break;
            case "south":
                {
                    if (CurrentPosition.Y >= RoomList.GetLength(0) - 1) break;
                    CurrentPosition = CurrentPosition with { Y = CurrentPosition.Y + 1 };
                    hasMoved = true;
                }
                break;
            case "north":
                {
                    if (CurrentPosition.Y <= 0) break;
                    CurrentPosition = CurrentPosition with { Y = CurrentPosition.Y - 1 };
                    hasMoved = true;
                }
                break;
            default:
                break;
        }
        if (hasMoved)
        {
            if (ProcessMoveEvent()) {
                Console.WriteLine("Something has changed because of the movement!");
            };
        }
        return hasMoved;
    }
    public bool Action(string actionString)
    {
        bool actionCommitted = false;
        switch (actionString)
        {
            case "fountain":
                {
                    FountainEnabled = actionCommitted = true;
                    RoomList[0, 0] = RoomList[0, 0] with { Text = "The Fountain of Objects has been reactivated, and you have escaped with your life!", Color = ConsoleColor.Green };
                    RoomList[0, 2] = RoomList[0, 2] with { Text = "You hear the rushing waters from the Fountain of Objects. It has been reactivated!"};
                }
                break;
            default:
                break;
        }

        return actionCommitted;
    }

    public bool Shoot(string direction)
    {
        --ArrowCount;
        bool enemyHit = false;
        switch (direction)
        {
            case "north":
                enemyHit = HitEnemy(CurrentPosition with { Y = CurrentPosition.Y - 1 });
                break;
            case "south":
                enemyHit = HitEnemy(CurrentPosition with { Y = CurrentPosition.Y + 1 });
                break;
            case "east":
                enemyHit = HitEnemy(CurrentPosition with { X = CurrentPosition.X + 1 });
                break;
            case "west":
                enemyHit = HitEnemy(CurrentPosition with { X = CurrentPosition.X - 1 });
                break;
            default:
                break;
        }
        if (enemyHit)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enemy has been killed by Arrow!");
            Console.ForegroundColor = ConsoleColor.White;
        }
            return true;
    }

    public bool HitEnemy(Position pos)
    {
        bool enemyHit = false;
        int r = RoomList.GetLength(0);
        int c = RoomList.GetLength(1);
        pos = WrappedPosition(pos, r, c);
        if (RoomList[pos.Y, pos.X].Type == RoomType.Amarok || RoomList[pos.Y, pos.X].Type == RoomType.Maelstorm)
        {
            // Remove the adjacent effects alongside it
            RemoveNodeAndAdjacent(pos, r, c);
            enemyHit = true;
            
        }
        return enemyHit;
    }
    public void RemoveNodeAndAdjacent(Position pos, int r, int c)
    {
        SetupAdjacents(RoomList, pos, r, c, "", ConsoleColor.White, RoomType.Normal);
        RoomList[pos.Y, pos.X] = new RoomInfo("", ConsoleColor.White, RoomType.Normal);
        RoomList = SetupPits(RoomList, r, c);
        RoomList = SetupPrimaryPositions(RoomList);
    }
    public Position WrappedPosition(Position pos, int r, int c)
    {
        int newX = pos.X < 0 ? pos.X + c - 1 : pos.X % ( c - 1 );
        int newY = pos.Y < 0 ? pos.Y + r - 1 : pos.Y % ( r - 1 );
        return new Position(newX, newY);
    }
    public Position ClampedPosition(Position pos, int r, int c)
    {
        int newX = pos.X < 0 ? 0 : (pos.X > (c - 1)) ? c - 1 : pos.X;
        int newY = pos.Y < 0 ? 0 : (pos.Y > (r - 1)) ? r - 1 : pos.Y;
        return new Position(newX, newY);
    }
    public bool ProcessMoveEvent()
    {
        return ProcessMaelstormEncounter();
    }
    public bool ProcessMaelstormEncounter()
    {
        if(RoomList[CurrentPosition.Y, CurrentPosition.X].Type == RoomType.Maelstorm)
        {
            int row = RoomList.GetLength(0);
            int col = RoomList.GetLength(1);
            PrintCurrentRoomInfo();
            SetupAdjacents(RoomList, CurrentPosition, row, col, "", ConsoleColor.White, RoomType.Normal);
            RoomList[CurrentPosition.Y, CurrentPosition.X] = new RoomInfo("", ConsoleColor.White, RoomType.Normal);
            (int xPos, int yPos) = WrappedPosition(CurrentPosition with { X = CurrentPosition.X - 2, Y = CurrentPosition.Y + 1 }, row, col);
            RoomList[yPos, xPos] = new RoomInfo("You encountered a Maelstorm! You were dispossesed!", ConsoleColor.DarkGray, RoomType.Maelstorm);
            RoomList = SetupAdjacents(roomInfo, new Position(xPos, yPos), row, col, "You hear the growling and groaning of a maelstrom nearby", ConsoleColor.Gray, RoomType.MaelstormAdjacent);
            RoomList = SetupPits(RoomList, row, col);
            RoomList = SetupPrimaryPositions(RoomList);
            CurrentPosition = WrappedPosition(CurrentPosition with { X= CurrentPosition.X + 2, Y = CurrentPosition.Y - 1 }, row, col);
            return true;
        }
        return false;
    }
    public bool CheckGameIsWon()
    {
        return (CurrentPosition == new Position(0, 0) && FountainEnabled);
    }
    public bool IsPositionPit()
    {
        return RoomList[CurrentPosition.Y, CurrentPosition.X].Type == RoomType.Pit; 
    }
    public bool IsPositionAmarok()
    {
        return RoomList[CurrentPosition.Y, CurrentPosition.X].Type == RoomType.Amarok;
    }
    public bool CheckGameIsLost()
    {
        return IsPositionPit() || IsPositionAmarok();
    }
    public bool IsCompleted()
    {
        return CheckGameIsWon() || CheckGameIsLost();  
    }
}

public record Position(int X, int Y);
public record RoomInfo(string Text, ConsoleColor Color, RoomType Type);
public enum RoomType { Normal, Pit, PitAdjacent, Fountain, Entrance, Maelstorm, MaelstormAdjacent, Amarok, AmarokAdjacent}
