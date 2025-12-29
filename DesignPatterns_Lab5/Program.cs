using System;
using System.Collections.Generic;
using System.Threading;

namespace BuilderPrototypeConsole
{
    public interface IPrototype<T>
    {
        T Clone();
    }

    public class GameMap : IPrototype<GameMap>
    {
        public string Name { get; set; }
        public char[] Grid { get; set; }
        public ConsoleColor ThemeColor { get; set; }
        public string Skybox { get; set; }

        public GameMap(string name, ConsoleColor color)
        {
            Name = name;
            ThemeColor = color;
            Grid = new char[10];
            Skybox = "Empty";
        }

        public GameMap Clone()
        {
            GameMap clone = new GameMap(this.Name + " (Clone)", this.ThemeColor);
            clone.Skybox = this.Skybox;
            clone.Grid = (char[])this.Grid.Clone();
            return clone;
        }

        public void Show()
        {
            Console.ForegroundColor = ThemeColor;
            Console.WriteLine($"Map: {Name} | Sky: {Skybox}");
            Console.Write("[");
            foreach (var block in Grid)
            {
                Console.Write(block);
            }
            Console.WriteLine("]");
            Console.ResetColor();
        }
    }

    public interface IMapBuilder
    {
        void Reset();
        void BuildTerrain();
        void BuildWalls();
        void BuildEnemies();
        void BuildLoot();
        GameMap GetMap();
    }

    public class ForestBuilder : IMapBuilder
    {
        private GameMap _map;
        public ForestBuilder() { Reset(); }

        public void Reset() { _map = new GameMap("Forest", ConsoleColor.Green); }

        public void BuildTerrain()
        {
            for (int i = 0; i < 10; i++) _map.Grid[i] = 'w';
        }
        public void BuildWalls() { _map.Grid[0] = 'T'; _map.Grid[9] = 'T'; }
        public void BuildEnemies() { _map.Grid[5] = 'W'; }
        public void BuildLoot() { _map.Grid[4] = '*'; }

        public GameMap GetMap() { return _map; }
    }

    public class DungeonBuilder : IMapBuilder
    {
        private GameMap _map;
        public DungeonBuilder() { Reset(); }

        public void Reset() { _map = new GameMap("Dungeon", ConsoleColor.DarkGray); }

        public void BuildTerrain()
        {
            for (int i = 0; i < 10; i++) _map.Grid[i] = '.';
        }
        public void BuildWalls() { _map.Grid[0] = '#'; _map.Grid[9] = '#'; }
        public void BuildEnemies() { _map.Grid[3] = 'S'; }
        public void BuildLoot() { _map.Grid[6] = '$'; }

        public GameMap GetMap() { return _map; }
    }

    public class Director
    {
        private IMapBuilder _builder;
        public void SetBuilder(IMapBuilder builder) { _builder = builder; }

        public void ConstructFullMap()
        {
            _builder.Reset();
            _builder.BuildTerrain();
            _builder.BuildWalls();
            _builder.BuildEnemies();
            _builder.BuildLoot();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Director director = new Director();
            ForestBuilder forestBuilder = new ForestBuilder();
            DungeonBuilder dungeonBuilder = new DungeonBuilder();

            GameMap originalMap = null;
            List<GameMap> clones = new List<GameMap>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== PATTERN LAB: BUILDER + PROTOTYPE ===");

                if (originalMap != null)
                {
                    Console.WriteLine("\n[Current Original]:");
                    originalMap.Show();
                }
                else
                {
                    Console.WriteLine("\n[No Map Created yet]");
                }

                if (clones.Count > 0)
                {
                    Console.WriteLine($"\n[Clones Created: {clones.Count}]");
                    int start = Math.Max(0, clones.Count - 3);
                    for (int i = start; i < clones.Count; i++)
                    {
                        Console.Write($"#{i + 1} ");
                        clones[i].Show();
                    }
                }

                Console.WriteLine("\n---------------- MENU ----------------");
                Console.WriteLine("1. BUILD: Forest Map");
                Console.WriteLine("2. BUILD: Dungeon Map");
                Console.WriteLine("3. CLONE: Make a copy of current map");
                Console.WriteLine("4. MODIFY: Change original");
                Console.WriteLine("0. Exit");
                Console.Write("Select: ");

                string choice = Console.ReadLine();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nBuilding Forest...");
                        SimulateWork();
                        director.SetBuilder(forestBuilder);
                        director.ConstructFullMap();
                        originalMap = forestBuilder.GetMap();
                        clones.Clear();
                        break;

                    case "2":
                        Console.WriteLine("\nBuilding Dungeon...");
                        SimulateWork();
                        director.SetBuilder(dungeonBuilder);
                        director.ConstructFullMap();
                        originalMap = dungeonBuilder.GetMap();
                        clones.Clear();
                        break;

                    case "3":
                        if (originalMap == null)
                        {
                            Console.WriteLine("Error: Nothing to clone!");
                            Console.ReadKey();
                            break;
                        }

                        GameMap newClone = originalMap.Clone();
                        clones.Add(newClone);
                        Console.WriteLine("Clone created instantly!");
                        break;

                    case "4":
                        if (originalMap != null)
                        {
                            originalMap.Grid[5] = 'X';
                            originalMap.Name = "DESTROYED MAP";
                            Console.WriteLine("Original map modified!");
                        }
                        break;
                }
            }
        }

        static void SimulateWork()
        {
            Console.Write("[");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                Console.Write("=");
            }
            Console.WriteLine("] Done!");
        }
    }
}