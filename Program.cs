using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Services;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Day25();
        }

        static void Day1()
        {
            int frequency = 0;
            HashSet<int> seenFreqs = new HashSet<int>();
            while (true)
            {
                foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_1.txt"))
                {
                    if (seenFreqs.Contains(frequency))
                        break;
                    seenFreqs.Add(frequency);
                    frequency += int.Parse(line);
                }

                if (seenFreqs.Contains(frequency))
                    break;
            }

            Console.WriteLine(frequency);
            Console.ReadLine();
        }

        static void Day2()
        {

            int total2s = 0;
            int total3s = 0;
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_2.txt"))
            {
                Dictionary<char, int> letterCount = new Dictionary<char, int>();

                foreach (char c in line)
                {
                    if (letterCount.ContainsKey(c))
                    {
                        letterCount[c] += 1;
                    }
                    else
                    {
                        letterCount[c] = 1;
                    }
                }

                if (letterCount.Any(lc => lc.Value == 2))
                    total2s++;
                if (letterCount.Any(lc => lc.Value == 3))
                    total3s++;
            }

            Console.WriteLine(total2s * total3s);
            Console.ReadLine();
        }

        static void Day2B()
        {
            string[] inputLines = File.ReadAllLines("M:\\AoC\\Input_Day_2.txt");
            foreach (string line in inputLines)
            {
                foreach (string line2 in inputLines)
                {
                    int diffs = 0;
                    string answer = "";
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] != line2[j])
                            diffs++;
                        else
                            answer += line[j];
                        if (diffs > 1)
                            break;
                    }

                    if (diffs == 1)
                    {
                        Console.WriteLine(answer);
                        Console.ReadLine();
                        return;
                    }
                }
            }
        }

        static void Day3()
        {
            const int dimensions = 1000;
            int[,] cloth = new int[dimensions, dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    cloth[i, j] = 0;
                }
            }

            Regex lineRegex = new Regex("#(\\d+) @ (\\d+),(\\d+): (\\d+)x(\\d+)");
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_3.txt"))
            {
                Match m = lineRegex.Match(line);
                Rectangle r = new Rectangle(int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value), int.Parse(m.Groups[5].Value));
                CutCloth(cloth, r);
            }

            int count = cloth.Cast<int>().Count(i => i > 1);
            /*for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    if (cloth[i, j] > 1)
                        count++;
                }
            }*/
            string id = "";
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_3.txt"))
            {
                Match m = lineRegex.Match(line);
                Rectangle r = new Rectangle(int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value), int.Parse(m.Groups[5].Value));
                if (ClothContainsOnlyOnes(cloth, r))
                {
                    id = m.Groups[1].Value;
                    break;
                }
            }

            Console.WriteLine(id);
            Console.ReadLine();
        }

        private static void CutCloth(int[,] cloth, Rectangle r)
        {
            for (int i = r.X; i < r.X + r.Width; i++)
            {
                for (int j = r.Y; j < r.Y + r.Height; j++)
                {
                    cloth[i, j]++;
                }
            }
        }

        private static bool ClothContainsOnlyOnes(int[,] cloth, Rectangle r)
        {
            for (int i = r.X; i < r.X + r.Width; i++)
            {
                for (int j = r.Y; j < r.Y + r.Height; j++)
                {
                    if (cloth[i, j] > 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static void Day4()
        {
            int toDropGuardIdStart = "[1518-05-29 00:00] Guard #".Length;
            int toDropGuardIdEnd = " begins shift".Length;
            int toDropTimestampStart = "[1518-05-29 ".Length;
            //int toKeepTimestampEnd = "00:00".Length;
            List<string> lines = File.ReadAllLines("M:\\AoC\\Input_Day_4.txt").ToList();
            lines.Sort();

            string guardOnDuty = "";
            bool isAsleep = false;
            Dictionary<string, List<int>> schedules = new Dictionary<string, List<int>>();

            foreach (string line in lines)
            {
                if (line.Contains("Guard"))
                {
                    guardOnDuty = line.Substring(toDropGuardIdStart, line.Length - toDropGuardIdEnd - toDropGuardIdStart);
                    isAsleep = false;
                    if (!schedules.ContainsKey(guardOnDuty))
                    {
                        schedules.Add(guardOnDuty, new List<int>());
                        for (int i = 0; i < 60; i++)
                        {
                            schedules[guardOnDuty].Add(0);
                        }
                    }
                }
                else
                {
                    string hour = line.Substring(toDropTimestampStart, 2);
                    string minute = line.Substring(toDropTimestampStart + 3, 2);
                    if (hour != "00")
                    {
                        throw new Exception();
                    }

                    if (line.Contains("wakes up"))
                    {
                        if (!isAsleep)
                            throw new Exception("awake people can't wake up");
                        isAsleep = false;
                        List<int> schedule = schedules[guardOnDuty];
                        for (int i = int.Parse(minute); i < 60; i++)
                        {
                            schedule[i] -= 1;
                        }
                    }
                    else
                    {

                        if (isAsleep)
                            throw new Exception("sleeping people can't fall asleep");
                        isAsleep = true;
                        List<int> schedule = schedules[guardOnDuty];
                        for (int i = int.Parse(minute); i < 60; i++)
                        {
                            schedule[i] += 1;
                        }
                    }
                }

            }

            string sleepyGuard = "";
            int sleepTime = 0;
            int sleepiestTime = 0;
            foreach (KeyValuePair<string, List<int>> guardSchedules in schedules)
            {
                int mySleepTime = guardSchedules.Value.Max();
                if (mySleepTime > sleepTime)
                {
                    sleepyGuard = guardSchedules.Key;
                    sleepTime = mySleepTime;
                    sleepiestTime = guardSchedules.Value.ToList().IndexOf(guardSchedules.Value.Max());
                }
            }

            Console.WriteLine(int.Parse(sleepyGuard) * sleepiestTime);
            Console.ReadLine();
        }

        static void Day5()
        {
            string input = File.ReadAllText("M:\\AoC\\Input_Day_5.txt").Trim();
            //input = input.Substring(0, 15);
            //input = "dabAcCaCBAcCcaDA";
            input = ReducePolymer(input);
            string originalReduction = input;
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            int shortestLength = originalReduction.Length;
            char shortestLetter = '?';
            foreach (char c in alphabet)
            {
                //input = originalReduction.Where(ch => char.ToLower(ch) != c).ToList().to;
                input = originalReduction.Replace(c.ToString(), "");
                input = input.Replace(c.ToString().ToUpper(), "");
                string reduced = ReducePolymer(input);
                if (reduced.Length < shortestLength)
                {
                    shortestLength = reduced.Length;
                    shortestLetter = c;
                }

            }

            Console.WriteLine(shortestLetter + " " + shortestLength);
            Console.ReadLine();
        }

        private static string ReducePolymer(string input)
        {
            int lastLength = input.Length;
            while (true)
            {
                //for (int i = input.Length - 1; i > 0; i--)
                //{
                //    if (i < input.Length)
                //    {
                //        if (input[i] != input[i - 1] && input.ToLower()[i] == input.ToLower()[i - 1])
                //        {
                //            input = input.Substring(0, i - 1) + input.Substring(i + 1);
                //        }
                //    }
                //}

                for (int i = 0; i < input.Length; i++)
                {
                    if (i < input.Length - 1 && i >= 0)
                    {
                        if (input[i] != input[i + 1] && input.ToLower()[i] == input.ToLower()[i + 1])
                        {
                            input = input.Substring(0, i) + input.Substring(i + 2);
                            i -= 2;
                        }
                    }
                }

                if (input.Length == lastLength)
                    break;
                lastLength = input.Length;
            }

            return input;
        }

        static void Day6()
        {
            Regex r = new Regex("(\\d+), (\\d+)");
            List<Point> points = new List<Point>();
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_6.txt"))
            {
                Match m = r.Match(line);
                points.Add(new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)));
            }

            Dictionary<Point, List<int>> distances = new Dictionary<Point, List<int>>();
            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            int[,] winners = new int[maxX + 1, maxY + 1];
            int safeAreaSize = 0;
            //int[,] distanceTotals = new int[maxX + 1, maxY + 1];
            for (int i = 0; i <= maxX; i++)
            {
                for (int j = 0; j <= maxY; j++)
                {
                    List<int> localDistances = new List<int>();
                    Point localPoint = new Point(i, j);
                    foreach (Point p in points)
                    {
                        localDistances.Add(ManhattanDistance(p, localPoint));
                    }

                    int minDistance = localDistances.Min();
                    int minInstances = localDistances.Count(d => d == minDistance);
                    if (minInstances == 1)
                        winners[i, j] = localDistances.IndexOf(minDistance);
                    else
                        winners[i, j] = -1;
                    //distanceTotals[i, j] =
                    if (SuperManhattanDistance(localPoint, points) < 10000)
                        safeAreaSize++;
                }
            }

            //HashSet<int> infiniteAreas = new HashSet<int>();
            List<int> areas = new List<int>();
            for (int i = 0; i < points.Count; i++)
            {
                areas.Add(0);
            }

            //for (int i = 0; i <= maxX; i++)
            //{
            //    infiniteAreas.Add(winners[i, 0]);
            //    infiniteAreas.Add(winners[i, maxY]);
            //}
            //for (int i = 0; i <= maxY; i++)
            //{
            //    infiniteAreas.Add(winners[0, i]);
            //    infiniteAreas.Add(winners[maxX, i]);
            //}
            foreach (int winner in winners)
            {
                if (winner != -1)
                {
                    areas[winner]++;
                }
            }

            for (int i = 0; i <= maxX; i++)
            {
                if (winners[i, 0] != -1)
                {
                    areas[winners[i, 0]] = 0;
                }

                if (winners[i, maxY] != -1)
                {

                    areas[winners[i, maxY]] = 0;
                }
            }

            for (int i = 0; i <= maxY; i++)
            {
                if (winners[0, i] != -1)
                {
                    areas[winners[0, i]] = 0;
                }

                if (winners[maxX, i] != -1)
                {
                    areas[winners[maxX, i]] = 0;
                }
            }

            Console.WriteLine(safeAreaSize);
            Console.ReadLine();
        }

        private static int ManhattanDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        private static int SuperManhattanDistance(Point p, List<Point> coordinates)
        {
            return coordinates.Sum(p2 => ManhattanDistance(p2, p));
        }

        private static void Day7()
        {
            List<Tuple<string, string>> dependancies = new List<Tuple<string, string>>();
            string steps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string resolvedSteps = "";
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_7.txt"))
            {
                string dependancy = line.Substring("Step ".Length, 1);
                string dependant = line.Substring("Step E must be finished before step ".Length, 1);
                dependancies.Add(new Tuple<string, string>(dependancy, dependant));
            }

            int timer = 0;
            List<Tuple<string, int>> workers = new List<Tuple<string, int>>();
            workers.Add(null);
            workers.Add(null);
            workers.Add(null);
            workers.Add(null);
            workers.Add(null);
            //while ((options = AvailableSteps(dependancies, resolvedSteps, steps)).Length > 0)
            while (true)
            {
                //workers = workers.Select(x => (x != null && x.Item2 == 0) ? null : x).ToList();

                for (int i = 0; i < workers.Count; i++)
                {
                    if (workers[i] != null && workers[i].Item2 == 0)
                    {
                        resolvedSteps += workers[i].Item1;
                        workers[i] = null;
                    }

                }

                string options = AvailableSteps(dependancies, resolvedSteps, steps);
                string realOptions = "";
                for (int i = 0; i < options.Length; i++)
                {
                    if (workers.All(w => w == null || w.Item1 != options[i].ToString()))
                    {
                        realOptions += options[i];
                    }
                }

                if (realOptions.Length == 0 && workers.All(x => x == null))
                {
                    break;
                }

                for (int i = 0; i < workers.Count && realOptions.Length > 0; i++)
                {
                    if (workers[i] == null)
                    {
                        workers[i] = new Tuple<string, int>(realOptions[0].ToString(), steps.IndexOf(realOptions[0]) + 61);
                        realOptions = realOptions.Substring(1);
                    }
                }

                workers = workers.Select(w => w == null ? null : new Tuple<string, int>(w.Item1, w.Item2 - 1)).ToList();
                timer++;
            }

            Console.WriteLine(timer);
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }

        private static string AvailableSteps(List<Tuple<string, string>> dependancies, string resolveSteps, string steps)
        {
            string availableSteps = "";
            foreach (char step in steps)
            {
                List<string> realizedDependencies = new List<string>();
                foreach (Tuple<string, string> dependancy in dependancies)
                {
                    if (dependancy.Item2 == step.ToString())
                    {
                        realizedDependencies.Add(dependancy.Item1);
                    }
                }

                if (realizedDependencies.All(resolveSteps.Contains))
                    availableSteps += step;
            }

            return string.Join("", availableSteps.Where(s => !resolveSteps.Contains(s)));
        }

        static void Day8()
        {
            string input = File.ReadAllText("M:\\AoC\\Input_Day_8.txt").Trim();
            //string input = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2";
            int metadataSum = 0;
            Node root = DefineNodeRecursive(ref input, ref metadataSum);
            Console.WriteLine(root.Value());
            Console.ReadLine();
        }

        private static Node DefineNodeRecursive(ref string input, ref int metadataSum)
        {
            Regex r = new Regex("(\\d+) ?(.*)");
            int childrenCount = PopString(ref input, r);
            int metadataCount = PopString(ref input, r);

            Node result = new Node();
            for (int i = 0; i < childrenCount; i++)
            {
                result.children.Add(DefineNodeRecursive(ref input, ref metadataSum));
            }

            for (int i = 0; i < metadataCount; i++)
            {
                result.metadata.Add(PopString(ref input, r));
            }

            metadataSum += result.metadata.Sum();
            return result;
        }

        private static int PopString(ref string s, Regex r)
        {
            Match m = r.Match(s);
            s = m.Groups[2].Value;
            return int.Parse(m.Groups[1].Value);
        }

        private class Node
        {
            public Node()
            {
                children = new List<Node>();
                metadata = new List<int>();
            }

            public List<Node> children;
            public List<int> metadata;

            public int Value()
            {
                if (children.Count == 0)
                {
                    return metadata.Sum();
                }

                int result = 0;
                foreach (int i in metadata)
                {
                    if (i > 0 && i <= children.Count)
                    {
                        result += children[i - 1].Value();
                    }
                }

                return result;
            }
        }

        static void Day9()
        {
            int players = 30;
            int marbles = 5807;

            LinkedList<int> circle = new LinkedList<int>(new[]{0});
            List<long> scores = new List<long>();
            for (int i = 0; i < players; i++)
            {
                scores.Add(0);
            }

            int currentPlayer = 0;
            //int currentMarbleIndex = 0;
            LinkedListNode<int> currentMarbleNode = circle.First;

            for (int i = 1; i <= marbles; i++)
            {
                if (i % 23 == 0)
                {
                    scores[currentPlayer] += i;
                    currentMarbleNode = MoveCounterClockwise(circle, currentMarbleNode, 7);
                    scores[currentPlayer] += currentMarbleNode.Value;
                    LinkedListNode<int> nextMarbleNode = MoveClockwise(circle, currentMarbleNode, 1);
                    circle.Remove(currentMarbleNode);
                    currentMarbleNode = nextMarbleNode;
                }
                else
                {
                    currentMarbleNode = MoveClockwise(circle, currentMarbleNode, 2);
                    circle.AddBefore(currentMarbleNode, i);
                    currentMarbleNode = currentMarbleNode.Previous;
                }

                currentPlayer = (currentPlayer + 1) % players;
            }

            Console.WriteLine(scores.Max());
            Console.ReadLine();
        }

        private static LinkedListNode<int> MoveClockwise(LinkedList<int> circle, LinkedListNode<int> currentPosition, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                currentPosition = currentPosition.Next;
                if (currentPosition == null)
                    currentPosition = circle.First;
            }

            return currentPosition;
        }

        private static LinkedListNode<int> MoveCounterClockwise(LinkedList<int> circle, LinkedListNode<int> currentPosition, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                currentPosition = currentPosition.Previous;
                if (currentPosition == null)
                    currentPosition = circle.Last;
            }

            return currentPosition;
        }

        static void Day10()
        {
            List<Vector> points = new List<Vector>();
            foreach (string line in File.ReadAllLines("M:\\AoC\\Input_Day_10.txt"))
            {
                int xpos = int.Parse(line.Substring("position=<".Length, 6));
                int ypos = int.Parse(line.Substring("position=< 50321, ".Length, 6));
                int xvel = int.Parse(line.Substring("position=<-49933,  -9891> velocity=<".Length, 2));
                int yvel = int.Parse(line.Substring("position=< 50321,  30215> velocity=<-5, ".Length, 2));
                points.Add(new Vector(new Point(xpos, ypos), new Point(xvel, yvel)));
            }

            int ticks = 0;
            while (true)
            {
                ticks++;
                points.ForEach(p => p.Tick());
                int minX = points.Min(p => p.position.X);
                int maxX = points.Max(p => p.position.X);
                int minY = points.Min(p => p.position.Y);
                int maxY = points.Max(p => p.position.Y);
                long xSize = maxX - minX + 1;
                long ySize = maxY - minY + 1;
                long area = xSize * ySize;
                if (area < 4000)
                {
                    char[,] matrix = new char[xSize, ySize];
                    for (int i = 0; i < xSize; i++)
                    {
                        for (int j = 0; j < ySize; j++)
                        {
                            matrix[i, j] = ' ';
                        }
                    }

                    foreach (Vector vector in points)
                    {
                        Point p = vector.RelativePoint(new Point(minX, minY));
                        matrix[p.X, p.Y] = '|';
                    }

                    for (int i = 0; i < xSize; i++)
                    {
                        string s = "";
                        for (int j = 0; j < ySize; j++)
                        {
                            s += matrix[i, j];
                        }

                        Console.WriteLine(s);
                    }

                    Console.WriteLine(ticks);
                    Console.ReadLine();
                }
            }
        }

        private class Vector
        {
            public Point position;
            public Point velocity;

            public Vector(Point position, Point velocity)
            {
                this.position = position;
                this.velocity = velocity;
            }

            public Point Tick()
            {
                return position = new Point(position.X + velocity.X, position.Y + velocity.Y);
            }

            public Point RelativePoint(Point origin)
            {
                return new Point(position.X - origin.X, position.Y - origin.Y);
            }
        }

        static void Day11()
        {
            //PowerCell pc = new PowerCell(new Point(3,5));
            List<PowerCell> powerCells = new List<PowerCell>();
            int[,] powerLevels = new int[301, 301];
            for (int i = 1; i <= 300; i++)
            {
                for (int j = 1; j <= 300; j++)
                {
                    powerCells.Add(new PowerCell(new Point(i, j)));
                    powerLevels[i, j] = powerCells.Last().PowerLevel();
                }
            }

            Point bestPoint = new Point(-1, -1);
            int bestScore = int.MinValue;
            int bestSize = 0;
            for (int squareSize = 1; squareSize <= 300; squareSize++)
            {

                for (int i = 1; i <= 301 - squareSize; i++)
                {
                    for (int j = 1; j <= 301 - squareSize; j++)
                    {
                        int score = 0;

                        for (int k = 0; k < squareSize; k++)
                        {
                            for (int l = 0; l < squareSize; l++)
                            {
                                score += powerLevels[i + k, j + l];
                            }

                        }

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestPoint = new Point(i, j);
                            bestSize = squareSize;
                        }
                    }
                }
            }


            Console.WriteLine("Size: " + bestSize + " Point: " + bestPoint.ToString());
            Console.ReadLine();
        }

        private class PowerCell
        {
            public Point location;
            public int serialNumber = 7165;

            public PowerCell(Point p)
            {
                location = p;
            }

            public int PowerLevel()
            {
                return (RackId() * location.Y + serialNumber) * RackId() % 1000 / 100 - 5;
            }

            private int RackId()
            {
                return location.X + 10;
            }
        }

        static void Day12()
        {
            long generations = 50000000000;
            string[] patterns = File.ReadAllLines("M:\\AoC\\Input_Day_12.txt");
            string initialState = patterns[0].Substring("initial state: ".Length);
            patterns = patterns.Skip(2).ToArray();


            bool[] currentGeneration = NewGeneration(1, initialState);

            for (int i = 0; i < initialState.Length; i++)
            {
                currentGeneration[2 + i] = initialState[i] == '#';
            }

            PottedPlantRow row = new PottedPlantRow(currentGeneration);

            Console.WriteLine(row.ToString());
            List<Tuple<bool[], bool>> parsedPatterns = new List<Tuple<bool[], bool>>();
            foreach (string pattern in patterns)
            {
                bool[] myPattern = pattern.Substring(0, 5).Select(c => c == '#').ToArray();
                bool result = pattern.Substring("##.## => ".Length, 1) == "#";
                parsedPatterns.Add(new Tuple<bool[], bool>(myPattern, result));
            }

            Dictionary<string, Tuple<long, long>> snapshots = new Dictionary<string, Tuple<long, long>>();
            bool hasSkipped = false;
            for (long i = 0; i < generations; i++)
            {
                row.Tick(parsedPatterns);

                Console.WriteLine(row.ToString());
                if (hasSkipped)
                {
                    continue;
                }

                string snapshotString = StateToString(row.currentState);
                if (!snapshots.ContainsKey(snapshotString))
                {
                    snapshots.Add(snapshotString, new Tuple<long, long>(row.currenGeneration, row.currentStartIndex));
                }
                else
                {
                    Console.WriteLine("It's happened!");
                    long timeToLoop = row.currenGeneration - snapshots[snapshotString].Item1;
                    long moveEachLoop = row.currentStartIndex - snapshots[snapshotString].Item2;
                    long timesToLoop = (generations - row.currenGeneration) / timeToLoop;
                    row.currenGeneration += timesToLoop;
                    i += timesToLoop;
                    row.currentStartIndex += (moveEachLoop * timesToLoop);
                    hasSkipped = true;
                }
                ////Console.WriteLine(i.ToString("D2") + " " + string.Join("", currentGeneration.Select(b => b ? '#' : '.')));
                ////Console.WriteLine(currentGeneration.Select((b, index) => b ? 1 : 0).Sum());
                //bool[] nextGeneration = NewGeneration(generations, initialState);

                //for (int j = 2; j < nextGeneration.Length-2; j++)
                //{
                //    foreach (Tuple<bool[], bool> parsedPattern in parsedPatterns)
                //    {
                //        bool match = true;
                //        for (int k = 0; k < 5 && match; k++)
                //        {
                //            match = parsedPattern.Item1[k] == currentGeneration[j + k - 2];
                //        }

                //        if (match)
                //            nextGeneration[j] = parsedPattern.Item2;
                //    }
                //}

                //currentGeneration = nextGeneration;
            }

            Console.WriteLine(row.ToString());
            Console.WriteLine(currentGeneration.Select((b, index) => b ? 1 : 0).Sum());
            Console.WriteLine(row.Value());
            Console.WriteLine(row.currentStartIndex);
            Console.ReadLine();
        }

        private static bool[] NewGeneration(int generations, string initialState)
        {
            bool[] currentGeneration = new bool[4 * generations + initialState.Length];
            for (int i = 0; i < currentGeneration.Length; i++)
            {
                currentGeneration[i] = false;
            }

            return currentGeneration;
        }

        private static bool[] NewGeneration(int length)
        {
            bool[] currentGeneration = new bool[length];
            for (int i = 0; i < currentGeneration.Length; i++)
            {
                currentGeneration[i] = false;
            }

            return currentGeneration;
        }

        private class PottedPlantRow
        {
            public long currentStartIndex;
            public bool[] currentState;
            public long currenGeneration;
            private int padding = 4;

            public PottedPlantRow(bool[] initialState)
            {
                currenGeneration = 0;
                currentStartIndex = -padding;
                currentState = NewGeneration(padding * 2 + initialState.Length);
                for (int i = 0; i < initialState.Length; i++)
                {
                    currentState[i + padding] = initialState[i];
                }
            }

            public void Tick(List<Tuple<bool[], bool>> patterns)
            {
                bool[] nextGeneration = NewGeneration(currentState.Length);
                for (int j = 2; j < currentState.Length - 2; j++)
                {
                    foreach (Tuple<bool[], bool> parsedPattern in patterns)
                    {
                        bool match = true;
                        for (int k = 0; k < 5 && match; k++)
                        {
                            match = parsedPattern.Item1[k] == currentState[j + k - 2];
                        }

                        if (match)
                            nextGeneration[j] = parsedPattern.Item2;
                    }
                }

                //currentState = nextGeneration;

                int firstIndexOf = nextGeneration.ToList().IndexOf(true);
                int lastIndexOf = nextGeneration.ToList().LastIndexOf(true);
                int beginningChop = firstIndexOf - padding;
                int endChop = lastIndexOf + padding;

                bool[] choppedGeneration = NewGeneration(lastIndexOf - firstIndexOf + padding * 2 + 1);

                for (int i = 0; i < nextGeneration.Length; i++)
                {
                    if (nextGeneration[i])
                    {
                        choppedGeneration[i - beginningChop] = true;
                    }
                }

                currentStartIndex += beginningChop;
                currentState = choppedGeneration;
                currenGeneration++;
            }

            public long Value()
            {
                return currentState.Select((b, index) => b ? index + currentStartIndex - 2 : 0).Sum();
            }

            public override string ToString()
            {
                return currenGeneration.ToString("D5") + " " + currentStartIndex.ToString("D5") + " " + StateToString(currentState);
            }


        }

        private static string StateToString(bool[] state)
        {
            return string.Join("", state.Select(b => b ? '#' : '.'));
        }

        static void Day13()
        {
            string[] trackConfig = File.ReadAllLines("M:\\AoC\\Input_Day_13.txt");
            int maxWidth = trackConfig.Max(s => s.Length);
            char[,] track = new char[maxWidth, trackConfig.Length];


            for (int i = 0; i < maxWidth; i++)
            {
                for (int j = 0; j < trackConfig.Length; j++)
                {
                    track[i, j] = ' ';
                }
            }

            List<Cart> carts = new List<Cart>();
            for (int j = 0; j < trackConfig.Length; j++)
            {
                for (int i = 0; i < trackConfig[j].Length; i++)
                {
                    track[i, j] = trackConfig[j][i];
                    if (track[i, j] == '<' || track[i, j] == '>')
                    {
                        carts.Add(new Cart(new Point(i, j), track[i, j]));
                        track[i, j] = '-';
                    }
                    else if (track[i, j] == 'v' || track[i, j] == '^')
                    {
                        carts.Add(new Cart(new Point(i, j), track[i, j]));
                        track[i, j] = '|';
                    }
                }
            }

            PrintTrack(track, carts);
            carts.Sort();
            bool hadCrash = false;
            Point crashLocation = new Point(0, 0);
            while (carts.Count > 1)
            {
                foreach (Cart cart in carts.ToArray())
                {
                    cart.Tick(track);
                    Cart victim = carts.FirstOrDefault(c => c != cart && c.CompareTo(cart) == 0);
                    if (victim != null)
                    {
                        if (!carts.Contains(victim))
                            throw new Exception();
                        hadCrash = true;
                        carts.Remove(cart);
                        carts.Remove(victim);
                        crashLocation = cart.location;
                    }
                }

                carts.Sort();
            }

            PrintTrack(track, carts);
            Console.WriteLine(carts.Last().PreviousPoint());
            Console.ReadLine();


        }

        private class Cart : IComparable<Cart>
        {
            public Point location;
            private char nextTurnDirection;
            public char currentDirection;
            private string directions = "<^>v";
            private string turnDirections = "LSR";
            private List<Point> history;

            public Cart(Point initialLocation, char initialDirection)
            {
                location = initialLocation;
                currentDirection = initialDirection;
                nextTurnDirection = 'L';
                history = new List<Point>();
                history.Add(location);
            }

            public void Tick(char[,] track)
            {
                TurnCart(track[location.X, location.Y]);
                switch (currentDirection)
                {
                    case '<':
                        location = new Point(location.X - 1, location.Y);
                        break;
                    case '>':
                        location = new Point(location.X + 1, location.Y);
                        break;
                    case 'v':
                        location = new Point(location.X, location.Y + 1);
                        break;
                    case '^':
                        location = new Point(location.X, location.Y - 1);
                        break;
                    default:
                        throw new Exception();
                }

                history.Add(location);
            }

            public void TurnCart(char c)
            {
                char thisTurnDirection = 'S';
                if (c == '/')
                {
                    if (currentDirection == '<' || currentDirection == '>')
                        thisTurnDirection = 'L';
                    if (currentDirection == '^' || currentDirection == 'v')
                        thisTurnDirection = 'R';
                }

                if (c == '\\')
                {
                    if (currentDirection == '<' || currentDirection == '>')
                        thisTurnDirection = 'R';
                    if (currentDirection == '^' || currentDirection == 'v')
                        thisTurnDirection = 'L';
                }

                if (c == '+')
                {
                    thisTurnDirection = nextTurnDirection;
                    IncrementTurnDirection();
                }

                int currentIndex = directions.IndexOf(currentDirection);
                if (thisTurnDirection == 'L')
                    currentDirection = directions[(currentIndex + directions.Length - 1) % directions.Length];
                if (thisTurnDirection == 'R')
                    currentDirection = directions[(currentIndex + 1) % directions.Length];
            }

            public Point PreviousPoint()
            {
                return history.Last();
            }

            private void IncrementTurnDirection()
            {
                int currentIndex = turnDirections.IndexOf(nextTurnDirection);
                nextTurnDirection = turnDirections[(currentIndex + 1) % turnDirections.Length];
            }

            public int CompareTo(Cart y)
            {
                if (location.X != y.location.X)
                {
                    return location.X.CompareTo(y.location.X);
                }
                else
                {
                    return location.Y.CompareTo(y.location.Y);
                }
            }
        }

        private static void PrintTrack(char[,] track, List<Cart> carts)
        {
            for (int i = 0; i < track.GetLength(1); i++)
            {
                string output = "";
                for (int j = 0; j < track.GetLength(0); j++)
                {
                    Cart cart = carts.FirstOrDefault(c => c.location == new Point(j, i));
                    if (cart != null)
                    {
                        output += cart.currentDirection;
                    }
                    else
                    {
                        output += track[j, i];
                    }
                }

                Console.WriteLine(output);
            }
        }

        static void Day14()
        {
            List<int> recipes = new List<int>{3, 7};
            int elf1 = 0;
            int elf2 = 1;
            string input = "894501";
            int answer = 0;
            while (true)
            {
                int recipeSum = recipes[elf1] + recipes[elf2];
                if (recipeSum >= 10)
                {
                    recipes.Add(1);
                    recipes.Add(recipeSum - 10);
                }
                else
                {
                    recipes.Add(recipeSum);
                }

                elf1 = (elf1 + recipes[elf1] + 1) % recipes.Count;
                elf2 = (elf2 + recipes[elf2] + 1) % recipes.Count;
                string s = "";
                for (int i = recipes.Count - 7; i < recipes.Count && recipes.Count > 7; i++)
                {
                    s += recipes[i];
                }

                if (s.Contains(input))
                {
                    answer = recipes.Count - 7 + s.IndexOf(input);
                    break;
                }
            }

            Console.WriteLine(answer);
            Console.ReadLine();
        }

        static void Day15()
        {
            for (int i = 4; true; i++)
            {
                if (!Day15Main(i))
                {
                    Console.WriteLine(i);
                    break;
                }
            }

            Console.ReadLine();
        }

        static bool Day15Main(int attackPower)
        {
            string[] fieldConfig = File.ReadAllLines("M:\\AoC\\Input_Day_15.txt");
            int maxWidth = fieldConfig.Max(s => s.Length);
            bool[,] field = new bool[maxWidth, fieldConfig.Length];


            for (int i = 0; i < maxWidth; i++)
            {
                for (int j = 0; j < fieldConfig.Length; j++)
                {
                    field[i, j] = true;
                }
            }

            List<Sentient> sentients = new List<Sentient>();
            for (int j = 0; j < fieldConfig.Length; j++)
            {
                for (int i = 0; i < fieldConfig[j].Length; i++)
                {
                    field[i, j] = fieldConfig[j][i] == '#';

                    if (fieldConfig[j][i] == 'E')
                    {
                        sentients.Add(new Sentient(new Point(i, j), 'E', attackPower));
                    }

                    if (fieldConfig[j][i] == 'G')
                    {
                        sentients.Add(new Sentient(new Point(i, j), 'G', 3));
                    }
                }
            }

            PrintField(field, sentients);

            sentients.Sort();
            int rounds = 1;
            int startElves = GetSpecies(sentients, 'E').Count();
            while (true)
            {
                bool done = false;
                foreach (Sentient sentient in sentients.ToArray())
                {
                    if (!sentients.Contains(sentient))
                        continue;
                    if (sentient.health <= 0)
                        throw new Exception();
                    //IEnumerable<Sentient> enemies = sentients.Where(s => s.species != sentient.species);
                    sentient.Move(field, sentients);
                    sentient.Attack(field, sentients);

                    if (GetSpecies(sentients, 'E').Count() != startElves)
                    {
                        return true;
                    }

                    if (!(GetSpecies(sentients, 'E').Any() && GetSpecies(sentients, 'G').Any()))
                    {
                        if (sentient.Equals(sentients.Last()))
                            rounds++;
                        done = true;
                        break;
                    }
                }

                if (done)
                    break;
                sentients.Sort();
                //Console.WriteLine(rounds);
                //PrintField(field, sentients);
                //Console.ReadLine();
                rounds++;
            }

            PrintField(field, sentients);
            Console.WriteLine(sentients.Sum(s => s.health) * (rounds - 1));
            //Console.ReadLine();
            return false;
        }

        private static Point CalculateMoveTo(Sentient us, bool[,] field, List<Sentient> sentients)
        {
            Point bestPoint = us.location;
            int bestDistance = 999;
            foreach (Point p in AdjacentPassablePoints(us.location, field, sentients))
            {
                if (sentients.Any(s => s.location.Equals(p) && s.species != us.species))
                    return us.location;
                if (sentients.Any(s => s.location.Equals(p) && s.species == us.species))
                    continue;
                HashSet<Point> visitedPoints = new HashSet<Point>();
                visitedPoints.Add(us.location);
                visitedPoints.Add(p);
                Queue<Tuple<Point, int>> pointQueue = new Queue<Tuple<Point, int>>();
                pointQueue.Enqueue(new Tuple<Point, int>(p, 0));
                while (pointQueue.Count != 0)
                {
                    Tuple<Point, int> nextPoint = pointQueue.Dequeue();
                    if (bestDistance < nextPoint.Item2)
                        break;
                    Sentient occupant = sentients.FirstOrDefault(s => s.location.Equals(nextPoint.Item1));
                    if (occupant != null)
                    {
                        if (occupant.species == us.species)
                        {
                            continue;
                        }

                        if (nextPoint.Item2 + 1 < bestDistance)
                        {
                            bestDistance = nextPoint.Item2 + 1;
                            bestPoint = p;
                            continue;
                        }
                    }

                    foreach (Point nextNextPoint in AdjacentPassablePoints(nextPoint.Item1, field, sentients))
                    {
                        if (visitedPoints.Add(nextNextPoint))
                        {
                            pointQueue.Enqueue(new Tuple<Point, int>(nextNextPoint, nextPoint.Item2 + 1));
                        }
                    }
                }
                //int distance = RecursiveEnemyDistance(field, sentients, us.species, visitedPoints, p, 0);

            }

            if (bestDistance <= 0)
                return us.location;
            return bestPoint;
        }

        private static int RecursiveEnemyDistance(bool[,] field, List<Sentient> sentients, char species, Dictionary<Point, int> visitedPoints, Point p, int depth)
        {
            int result = 999;
            if (!visitedPoints.ContainsKey(p))
                visitedPoints.Add(p, depth);
            if (visitedPoints[p] > depth)
            {
                visitedPoints[p] = depth;
            }

            Sentient currentOccupant = sentients.FirstOrDefault(s => s.location.Equals(p));
            if (currentOccupant != null && currentOccupant.species != species)
                return 0;
            if (currentOccupant != null && currentOccupant.species == species)
                return 999;
            foreach (Point adjacentPassablePoint in AdjacentPassablePoints(p, field, sentients))
            {
                if (visitedPoints.ContainsKey(adjacentPassablePoint) && visitedPoints[adjacentPassablePoint] <= depth)
                    continue;
                Sentient occupant = sentients.FirstOrDefault(s => s.location.Equals(adjacentPassablePoint));
                if (occupant != null)
                {
                    if (occupant.species == species)
                        continue;
                    return 1;
                }

                result = Math.Min(result, 1 + RecursiveEnemyDistance(field, sentients, species, visitedPoints, adjacentPassablePoint, depth + 1));
            }

            return result;
        }

        private static IEnumerable<Point> AdjacentPassablePoints(Point p, bool[,] field, List<Sentient> sentients)
        {
            Point pUp = new Point(p.X, p.Y - 1);
            Point pDown = new Point(p.X, p.Y + 1);
            Point pLeft = new Point(p.X - 1, p.Y);
            Point pRight = new Point(p.X + 1, p.Y);
            if (IsPassable(pUp, field, sentients))
            {
                yield return pUp;
            }

            if (IsPassable(pLeft, field, sentients))
            {
                yield return pLeft;
            }

            if (IsPassable(pRight, field, sentients))
            {
                yield return pRight;
            }

            if (IsPassable(pDown, field, sentients))
            {
                yield return pDown;
            }
        }

        private static bool IsPassable(Point p, bool[,] field, List<Sentient> sentients)
        {
            if (field[p.X, p.Y])
            {
                return false;
            }

            return true;
            //return sentients.All(s => !s.location.Equals(p));

        }

        private static IEnumerable<Sentient> GetSpecies(IEnumerable<Sentient> sentients, char species)
        {
            return sentients.Where(s => s.species == species);
        }

        private static void PrintField(bool[,] field, List<Sentient> sentients)
        {
            string health = "";
            for (int i = 0; i < field.GetLength(1); i++)
            {
                string output = "";
                for (int j = 0; j < field.GetLength(0); j++)
                {
                    Sentient sentient = sentients.FirstOrDefault(c => c.location == new Point(j, i));
                    if (sentient != null)
                    {
                        output += sentient.species;
                        health += sentient.species + " " + sentient.health + " ";
                    }
                    else
                    {
                        output += field[j, i] ? '#' : ' ';
                    }
                }

                Console.WriteLine(output);
            }

            Console.WriteLine(health);
        }

        private class Sentient : IComparable<Sentient>
        {
            public int health = 200;
            public Point location;
            public int attackPower;
            public char species;

            public override string ToString()
            {
                return "Health: " + health + " Species: " + species + " Location: " + location;
            }

            public Sentient(Point initialLocation, char species, int attackPower)
            {
                location = initialLocation;
                this.species = species;
                this.attackPower = attackPower;
            }

            public void TakeHit(int attackPower)
            {
                health -= attackPower;
            }

            public int CompareTo(Sentient y)
            {
                if (location.Y != y.location.Y)
                {
                    return location.Y.CompareTo(y.location.Y);
                }

                return location.X.CompareTo(y.location.X);
            }

            public void Move(bool[,] field, List<Sentient> sentients)
            {
                location = CalculateMoveTo(this, field, sentients);
            }

            public void Attack(bool[,] field, List<Sentient> sentients)
            {
                int minHealth = 999;
                foreach (Point p in AdjacentPassablePoints(location, field, sentients))
                {
                    Sentient victim = sentients.FirstOrDefault(s => s.location.Equals(p) && s.species != species);
                    if (victim != null)
                        minHealth = Math.Min(minHealth, victim.health);
                }

                foreach (Point p in AdjacentPassablePoints(location, field, sentients))
                {
                    Sentient victim = sentients.FirstOrDefault(s => s.location.Equals(p) && s.species != species && s.health == minHealth);
                    if (victim != null)
                    {
                        victim.TakeHit(attackPower);
                        if (victim.health <= 0)
                            sentients.Remove(victim);
                        break;
                    }
                }
            }

        }

        static void Day16()
        {
            //string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_16.txt");
            //int answer = 0;
            //List<Tuple<int,List<CPU_OPS>>> possibleOps = new List<Tuple<int, List<CPU_OPS>>>();
            //List<int[]> commands = new List<int[]>();
            //for (int i = 0; i < input.Length; i++)
            //{
            //    if (input[i].StartsWith("Before", StringComparison.InvariantCulture))
            //    {
            //        int[] initialInts = input[i].Substring("Before: [".Length, 10).Split(',').Select(int.Parse).ToArray();
            //        CPU cpu = new CPU(initialInts);
            //        int[] instruction = input[i + 1].Split(' ').Select(int.Parse).ToArray();
            //        int[] finalInts = input[i+2].Substring("After:  [".Length, 10).Split(',').Select(int.Parse).ToArray();
            //        List<CPU_OPS> possibilities = cpu.TestInstruction(instruction, finalInts);
            //        possibleOps.Add(new Tuple<int, List<CPU_OPS>>(instruction[0], possibilities));
            //        if (possibilities.Count >= 3)
            //            answer++;
            //        i += 3;
            //    }
            //    else if (input[i] == "")
            //        continue;
            //    else
            //    {
            //        commands.Add(input[i].Split(' ').Select(int.Parse).ToArray());
            //    }
            //}
            //Dictionary<int, CPU_OPS> opCodeMapping = new Dictionary<int, CPU_OPS>();
            //while (true)
            //{
            //    foreach (Tuple<int, List<CPU_OPS>> possibilities in possibleOps)
            //    {
            //        if(opCodeMapping.ContainsKey(possibilities.Item1))
            //            continue;
            //        List<CPU_OPS> remainingPossibilities = possibilities.Item2.Where(c => !opCodeMapping.ContainsValue(c)).ToList();
            //        if (remainingPossibilities.Count == 1)
            //        {
            //            opCodeMapping.Add(possibilities.Item1, remainingPossibilities.First());
            //        }
            //    }

            //    if (opCodeMapping.Count == Enum.GetValues(typeof(CPU_OPS)).Length)
            //        break;
            //}
            //Console.WriteLine(answer);
            //foreach (KeyValuePair<int, CPU_OPS> keyValuePair in opCodeMapping)
            //{
            //    Console.WriteLine(keyValuePair.Key+": " + keyValuePair.Value);
            //}
            //CPU commandCpu = new CPU(new long[] {0,0,0,0});

            //Console.WriteLine(commandCpu.ToString());
            //for (int i = 0; i < commands.Count; i++)
            //{
            //    commandCpu.RunOperation(opCodeMapping[commands[i][0]], commands[i]);
            //}
            //Console.WriteLine(commandCpu.ToString());
            //Console.ReadLine();
        }

        public enum CPU_OPS
        {
            ADDR,
            ADDI,
            MULR,
            MULI,
            BANR,
            BANI,
            BORR,
            BORI,
            SETR,
            SETI,
            GTIR,
            GTRI,
            GTRR,
            EQIR,
            EQRI,
            EQRR
        }

        public class CPU
        {
            public long[] registers;
            public int instructionPointer;

            public CPU(long[] initialRegisters)
            {
                registers = initialRegisters;
            }

            public CPU(int instructionPointer, long registers)
            {
                this.instructionPointer = instructionPointer;
                this.registers = new long[registers];
            }

            public CPU(int instructionPointer, long[] initialRegisters)
            {
                this.instructionPointer = instructionPointer;
                registers = initialRegisters;
            }

            public long InstructionNumber => registers[instructionPointer];

            public long[] RunOperation(CPU_OPS op, long[] inputs, bool overwrite = true)
            {
                long[] output;
                switch (op)
                {
                    case CPU_OPS.ADDR:
                        output = Addr(inputs);
                        break;
                    case CPU_OPS.ADDI:
                        output = Addi(inputs);
                        break;
                    case CPU_OPS.MULR:
                        output = Mulr(inputs);
                        break;
                    case CPU_OPS.MULI:
                        output = Muli(inputs);
                        break;
                    case CPU_OPS.BANR:
                        output = Banr(inputs);
                        break;
                    case CPU_OPS.BANI:
                        output = Bani(inputs);
                        break;
                    case CPU_OPS.BORR:
                        output = Borr(inputs);
                        break;
                    case CPU_OPS.BORI:
                        output = Bori(inputs);
                        break;
                    case CPU_OPS.SETR:
                        output = Setr(inputs);
                        break;
                    case CPU_OPS.SETI:
                        output = Seti(inputs);
                        break;
                    case CPU_OPS.GTIR:
                        output = Gtir(inputs);
                        break;
                    case CPU_OPS.GTRI:
                        output = Gtri(inputs);
                        break;
                    case CPU_OPS.GTRR:
                        output = Gtrr(inputs);
                        break;
                    case CPU_OPS.EQIR:
                        output = Eqir(inputs);
                        break;
                    case CPU_OPS.EQRI:
                        output = Eqri(inputs);
                        break;
                    case CPU_OPS.EQRR:
                        output = Eqrr(inputs);
                        break;
                    default:
                        throw new ArgumentException();
                }

                if (overwrite)
                    registers = output;
                registers[instructionPointer]++;
                return output;
            }

            public List<CPU_OPS> TestInstruction(long[] inputs, long[] outputs)
            {
                List<CPU_OPS> successfulOps = new List<CPU_OPS>();
                foreach (CPU_OPS op in Enum.GetValues(typeof(CPU_OPS)))
                {
                    //if (SequenceEquals(outputs, RunOperation(op, inputs, false)))
                    //    successfulOps.Add(op);
                }

                return successfulOps;
                //if (SequenceEquals(outputs, Addr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Addi(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Mulr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Muli(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Banr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Bani(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Borr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Bori(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Setr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Seti(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Gtir(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Gtri(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Gtrr(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Eqir(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Eqri(inputs)))
                //    result++;
                //if (SequenceEquals(outputs, Eqrr(inputs)))
                //    result++;
            }

            public long[] Addr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] + result[inputs[2]];
                return result;
            }

            public long[] Addi(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] + inputs[2];
                return result;
            }

            public long[] Mulr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] * result[inputs[2]];
                return result;
            }

            public long[] Muli(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] * inputs[2];
                return result;
            }

            public long[] Banr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] & result[inputs[2]];
                return result;
            }

            public long[] Bani(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] & inputs[2];
                return result;
            }

            public long[] Borr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] | result[inputs[2]];
                return result;
            }

            public long[] Bori(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] | inputs[2];
                return result;
            }

            public long[] Setr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]];
                return result;
            }

            public long[] Seti(long[] inputs)
            {
                long[] result = registers.ToArray();


                result[inputs[3]] = inputs[1];
                return result;
            }

            public long[] Gtir(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = inputs[1] > result[inputs[2]] ? 1 : 0;
                return result;
            }

            public long[] Gtri(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] > inputs[2] ? 1 : 0;
                return result;
            }

            public long[] Gtrr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] > result[inputs[2]] ? 1 : 0;
                return result;
            }

            public long[] Eqir(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = inputs[1] == result[inputs[2]] ? 1 : 0;
                return result;
            }

            public long[] Eqri(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] == inputs[2] ? 1 : 0;
                return result;
            }

            public long[] Eqrr(long[] inputs)
            {
                long[] result = registers.ToArray();
                result[inputs[3]] = result[inputs[1]] == result[inputs[2]] ? 1 : 0;
                return result;
            }

            public override string ToString()
            {
                string output = "";
                foreach (long register in registers)
                {
                    output += register + " ";
                }

                return output.Trim();
            }
        }

        static void Day17()
        {
            Regex r = new Regex("([xy])=(\\d+), ([xy])=(\\d+)\\.\\.(\\d+)");
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_17.txt");

            int maxX = 500;
            int minX = 500;
            int maxY = 0;

            foreach (string line in input)
            {
                Match m = r.Match(line);
                if (m.Groups[1].Value == "x")
                {
                    maxX = Math.Max(maxX, int.Parse(m.Groups[2].Value));
                    minX = Math.Min(minX, int.Parse(m.Groups[2].Value));
                    maxY = Math.Max(maxY, int.Parse(m.Groups[5].Value));
                }
                else if (m.Groups[1].Value == "y")
                {
                    maxX = Math.Max(maxX, int.Parse(m.Groups[5].Value));
                    minX = Math.Min(minX, int.Parse(m.Groups[5].Value));
                    maxY = Math.Max(maxY, int.Parse(m.Groups[2].Value));
                }
                else
                {
                    throw new Exception();
                }
            }


            char[,] theEarth = new char[maxX + 2, maxY + 1];
            for (int j = 0; j < theEarth.GetLength(1); j++)
            {
                for (int i = 0; i < theEarth.GetLength(0); i++)
                {
                    theEarth[i, j] = '.';
                }
            }

            theEarth[500, 0] = '+';

            foreach (string line in input)
            {
                Match m = r.Match(line);

                if (m.Groups[1].Value == "x")
                {
                    for (int i = int.Parse(m.Groups[4].Value); i <= int.Parse(m.Groups[5].Value); i++)
                    {
                        theEarth[int.Parse(m.Groups[2].Value), i] = '#';
                    }
                }
                else if (m.Groups[1].Value == "y")
                {
                    for (int i = int.Parse(m.Groups[4].Value); i <= int.Parse(m.Groups[5].Value); i++)
                    {
                        theEarth[i, int.Parse(m.Groups[2].Value)] = '#';
                    }
                }
                else
                {
                    throw new Exception();
                }
            }

            PrintEarth(theEarth, minX);
            bool addedWater = true;
            while (addedWater)
            {
                Point currentPoint = new Point(500, 0);
                Queue<Point> nextPoints = new Queue<Point>();
                HashSet<Point> visitedPoints = new HashSet<Point>();
                nextPoints.Enqueue(currentPoint);
                addedWater = false;
                while (nextPoints.Any())
                {
                    Point nextPoint = nextPoints.Dequeue();
                    Point fallPoint = FallPoint(theEarth, nextPoint, maxY);
                    if (fallPoint.Y >= maxY)
                        continue;
                    Point[] flowFallPoints = FlowFallPoints(theEarth, fallPoint);
                    bool leftCanFall = CanFall(theEarth, flowFallPoints[0]);
                    bool rightCanFall = CanFall(theEarth, flowFallPoints[1]);
                    for (int i = flowFallPoints[0].X; i <= flowFallPoints[1].X; i++)
                    {
                        theEarth[i, flowFallPoints[0].Y] = !rightCanFall && !leftCanFall ? '~' : '|';
                    }

                    if (!rightCanFall && !leftCanFall)
                    {
                        addedWater = true;
                    }

                    if (leftCanFall && !visitedPoints.Contains(flowFallPoints[0]))
                    {
                        visitedPoints.Add(flowFallPoints[0]);
                        nextPoints.Enqueue(flowFallPoints[0]);
                    }

                    if (rightCanFall && !visitedPoints.Contains(flowFallPoints[1]))
                    {
                        visitedPoints.Add(flowFallPoints[1]);
                        nextPoints.Enqueue(flowFallPoints[1]);
                    }
                }
                //PrintEarth(theEarth, minX);
                //Console.ReadLine();

            }

            PrintEarth(theEarth, minX);
            Console.ReadLine();
        }

        private static bool CanFlowLeft(char[,] theEarth, Point p)
        {
            return theEarth[p.X - 1, p.Y] == '.' || theEarth[p.X - 1, p.Y] == '|';
        }

        private static bool CanFlowRight(char[,] theEarth, Point p)
        {
            return theEarth[p.X + 1, p.Y] == '.' || theEarth[p.X + 1, p.Y] == '|';
        }

        private static Point[] FlowFallPoints(char[,] theEarth, Point p)
        {
            Point leftPoint = p;
            Point rightPoint = p;
            while (CanFlowLeft(theEarth, leftPoint))
            {
                leftPoint = new Point(leftPoint.X - 1, leftPoint.Y);
                if (CanFall(theEarth, leftPoint))
                    break;
            }

            while (CanFlowRight(theEarth, rightPoint))
            {
                rightPoint = new Point(rightPoint.X + 1, rightPoint.Y);
                if (CanFall(theEarth, rightPoint))
                    break;
            }

            return new[]{leftPoint, rightPoint};
        }

        private static IEnumerable<Point> FlowablePoints(char[,] theEarth, Point p)
        {
            Point left = new Point(p.X - 1, p.Y);
            Point right = new Point(p.X, p.Y + 1);

            if (theEarth[left.X, left.Y] == '.' || theEarth[left.X, left.Y] == '|')
            {
                yield return left;
            }

            if (theEarth[right.X, right.Y] == '.' || theEarth[right.X, right.Y] == '|')
            {
                yield return right;
            }
        }

        private static bool CanFall(char[,] theEarth, Point p)
        {
            //if (p.Y + 1 > maxY)
            //{
            //    return false;
            //}

            //Point fallPoint = FallPoint(p);

            return theEarth[p.X, p.Y + 1] == '.' || theEarth[p.X, p.Y + 1] == '|';
        }

        private static Point FallPoint(char[,] theEarth, Point p, int maxY)
        {
            Point fallPoint = p;
            while (fallPoint.Y < maxY && CanFall(theEarth, fallPoint))
            {
                theEarth[fallPoint.X, fallPoint.Y] = theEarth[fallPoint.X, fallPoint.Y] == '+' ? '+' : '|';
                fallPoint = new Point(fallPoint.X, fallPoint.Y + 1);
            }

            theEarth[fallPoint.X, fallPoint.Y] = '|';
            return fallPoint;
        }

        private static void PrintEarth(char[,] theEarth, int minX)
        {
            int countWater = 0;
            int countFlows = 0;
            for (int j = 0; j < theEarth.GetLength(1); j++)
            {
                string output = "";
                for (int i = minX - 1; i < theEarth.GetLength(0); i++)
                {
                    output += theEarth[i, j];
                    if (theEarth[i, j] == '|')
                        countFlows++;
                    if (theEarth[i, j] == '~')
                        countWater++;
                }

                Console.WriteLine(output);
            }

            Console.WriteLine("Water: " + countWater + " Flows: " + countFlows + " Answer: " + (countWater + countFlows));
            Console.WriteLine("");
        }

        private static void Day18()
        {
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_18.txt");



            char[,] forest = new char[input.Length, input[0].Length];
            for (int j = 0; j < input.Length; j++)
            {
                for (int i = 0; i < input[j].Length; i++)
                {
                    forest[i, j] = input[j][i];
                }
            }

            PrintForest(forest);
            char[,] snapshot = null;
            int wavelength = -1;
            for (int c = 0; c < 2820; c++)
            {
                char[,] nextForest = new char[input.Length, input[0].Length];
                for (int j = 0; j < input.Length; j++)
                {
                    for (int i = 0; i < input[j].Length; i++)
                    {
                        char contents = forest[i, j];
                        Tuple<int, int> neighbors = CountSpaces(forest, new Point(i, j));
                        if (contents == '.')
                        {
                            if (neighbors.Item2 >= 3)
                                contents = '|';
                        }
                        else if (contents == '|')
                        {
                            if (neighbors.Item1 >= 3)
                                contents = '#';
                        }
                        else if (contents == '#')
                        {
                            if (neighbors.Item1 < 1 | neighbors.Item2 < 1)
                                contents = '.';
                        }
                        else
                        {
                            throw new Exception();
                        }

                        nextForest[i, j] = contents;
                    }
                }

                //if (c==2500)
                //    snapshot = forest;
                //if (c > 2500)
                //{
                //    if (MatrixEquals(snapshot, forest))
                //    {
                //        wavelength = c - 2500;
                //        Console.WriteLine(wavelength);
                //    }
                //}

                forest = nextForest;
                if (c % 2819 == 0)
                {
                    Console.WriteLine("After " + (c + 1) + " minutes...");
                    PrintForest(forest);
                    Console.ReadLine();
                }
            }

            Console.ReadLine();
        }

        private static bool MatrixEquals<T>(T[,] a, T[,] b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            if (a.GetLength(0) != b.GetLength(0))
                return false;
            if (a.GetLength(1) != b.GetLength(1))
                return false;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (!a[i, j].Equals(b[i, j]))
                        return false;
                }
            }

            return true;
        }

        private static Tuple<int, int> CountSpaces(char[,] forest, Point p)
        {
            int countLumberyards = 0;
            int countWoodlands = 0;
            for (int i = Math.Max(p.X - 1, 0); i < Math.Min(p.X + 2, forest.GetLength(0)); i++)
            {
                for (int j = Math.Max(p.Y - 1, 0); j < Math.Min(p.Y + 2, forest.GetLength(1)); j++)
                {
                    if (new Point(i, j) == p)
                        continue;
                    if (forest[i, j] == '|')
                        countWoodlands++;
                    if (forest[i, j] == '#')
                        countLumberyards++;
                }
            }

            return new Tuple<int, int>(countLumberyards, countWoodlands);
        }

        private static void PrintForest(char[,] forest)
        {
            int countLumberyards = 0;
            int countWoodlands = 0;
            for (int j = 0; j < forest.GetLength(1); j++)
            {
                string output = "";
                for (int i = 0; i < forest.GetLength(0); i++)
                {
                    output += forest[i, j];
                    if (forest[i, j] == '|')
                        countWoodlands++;
                    if (forest[i, j] == '#')
                        countLumberyards++;
                }

                Console.WriteLine(output);
            }

            Console.WriteLine("Lumberyards: " + countLumberyards + " Woodlands: " + countWoodlands + " Answer: " + countLumberyards * countWoodlands);
            Console.WriteLine("");
        }

        static void Day19()
        {
            Regex r = new Regex("(\\w+) (\\d+) (\\d+) (\\d+)");
            List<string> input = File.ReadAllLines("M:\\AoC\\Input_Day_19.txt").ToList();
            int instructionRegister = int.Parse(input[0].Substring("#ip ".Length));
            input.RemoveAt(0);
            List<Tuple<CPU_OPS, long[]>> parsedInstructions = new List<Tuple<CPU_OPS, long[]>>();
            foreach (string s in input)
            {
                Match m = r.Match(s);
                CPU_OPS op;
                Enum.TryParse(m.Groups[1].Value, true, out op);
                long[] instructions = {0, long.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value)};
                parsedInstructions.Add(new Tuple<CPU_OPS, long[]>(op, instructions));

            }

            CPU powerCPU = new CPU(instructionRegister, new long[]{0, 35, 0, 10550400, 7, 0}); //0, 9, 10551306, 0, 10551306, 10551305});//0 35 0 10550400 10551306 0
            Console.WriteLine(powerCPU.ToString());
            while (powerCPU.InstructionNumber < input.Count)
            {
                Tuple<CPU_OPS, long[]> parsedInstruction = parsedInstructions[(int)powerCPU.InstructionNumber];
                bool displayOutput = false;
                if (powerCPU.InstructionNumber == 7) //|| powerCPU.InstructionNumber = )
                {
                    Console.WriteLine("Input Registers: " + powerCPU + " Instructions:  " + parsedInstruction.Item1 + " " + string.Join(",", parsedInstruction.Item2));
                    displayOutput = true;
                }

                powerCPU.RunOperation(parsedInstruction.Item1, parsedInstruction.Item2);
                if (displayOutput)
                {
                    Console.WriteLine("Output Registers: " + powerCPU);
                    displayOutput = false;
                }

                //Console.ReadLine();
            }

            long answer = 0;
            for (int i = 1; i <= 10551306; i++)
            {
                if (10551306 % i == 0)
                    answer += i;
            }

            Console.WriteLine(answer);
            Console.WriteLine(powerCPU.ToString());
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static void Day20()
        {
            string input = File.ReadAllLines("M:\\AoC\\Input_Day_20.txt")[0];
            Queue<Tuple<int, Point>> builderQueue = new Queue<Tuple<int, Point>>();
            input = input.Substring(1, input.Length - 2);
            builderQueue.Enqueue(new Tuple<int, Point>(0, new Point(0, 0)));

            HashSet<Tuple<Point, Point>> doorLocations = new HashSet<Tuple<Point, Point>>();
            HashSet<Tuple<int, Point>> queuedItems = new HashSet<Tuple<int, Point>>();
            queuedItems.Add(new Tuple<int, Point>(0, new Point(0, 0)));

            while (builderQueue.Any())
            {
                Tuple<int, Point> builder = builderQueue.Dequeue();
                Point currentPoint = builder.Item2;
                int currentIndex = builder.Item1;
                while (currentIndex < input.Length)
                {
                    char currentChar = input[currentIndex];
                    Point nextPoint;
                    bool stopForNow = false;
                    switch (currentChar)
                    {
                        case 'E':
                            nextPoint = new Point(currentPoint.X + 1, currentPoint.Y);
                            doorLocations.Add(new Tuple<Point, Point>(currentPoint, nextPoint));
                            currentPoint = nextPoint;
                            break;
                        case 'W':
                            nextPoint = new Point(currentPoint.X - 1, currentPoint.Y);
                            doorLocations.Add(new Tuple<Point, Point>(currentPoint, nextPoint));
                            currentPoint = nextPoint;
                            break;
                        case 'N':
                            nextPoint = new Point(currentPoint.X, currentPoint.Y + 1);
                            doorLocations.Add(new Tuple<Point, Point>(currentPoint, nextPoint));
                            currentPoint = nextPoint;
                            break;
                        case 'S':
                            nextPoint = new Point(currentPoint.X, currentPoint.Y - 1);
                            doorLocations.Add(new Tuple<Point, Point>(currentPoint, nextPoint));
                            currentPoint = nextPoint;
                            break;
                        case '(':
                            foreach (int subIndex in AlternateIndices(input, currentIndex))
                            {
                                Tuple<int, Point> nextQueue = new Tuple<int, Point>(subIndex, currentPoint);
                                if (!queuedItems.Contains(nextQueue))
                                {
                                    builderQueue.Enqueue(nextQueue);
                                    queuedItems.Add(nextQueue);
                                }

                            }

                            stopForNow = true;
                            break;
                        case ')':
                            Tuple<int, Point> nextQueue2 = new Tuple<int, Point>(currentIndex + 1, currentPoint);
                            if (!queuedItems.Contains(nextQueue2))
                            {
                                builderQueue.Enqueue(nextQueue2);
                                queuedItems.Add(nextQueue2);
                            }

                            stopForNow = true;
                            break;
                        case '|':
                            Tuple<int, Point> nextQueue3 = new Tuple<int, Point>(GetNextCloseParenAtLevel(input, currentIndex), currentPoint);
                            if (!queuedItems.Contains(nextQueue3))
                            {
                                builderQueue.Enqueue(nextQueue3);
                                queuedItems.Add(nextQueue3);
                            }

                            stopForNow = true;
                            break;
                        default:
                            throw new Exception();
                    }

                    if (stopForNow)
                        break;
                    currentIndex++;
                }
            }

            //foreach (Tuple<Point, Point> doorLocation in doorLocations)
            //{
            //    Console.WriteLine(doorLocation.Item1 + " " + doorLocation.Item2);
            //}
            Console.WriteLine(doorLocations.Count);

            Dictionary<Point, HashSet<Point>> doorDict = new Dictionary<Point, HashSet<Point>>();
            HashSet<Point> visitedPoints = new HashSet<Point>();
            foreach (Tuple<Point, Point> doorLocation in doorLocations)
            {
                if (!doorDict.ContainsKey(doorLocation.Item1))
                {
                    doorDict.Add(doorLocation.Item1, new HashSet<Point>());
                }

                if (!doorDict.ContainsKey(doorLocation.Item2))
                {
                    doorDict.Add(doorLocation.Item2, new HashSet<Point>());
                }

                doorDict[doorLocation.Item1].Add(doorLocation.Item2);
                doorDict[doorLocation.Item2].Add(doorLocation.Item1);
            }

            Queue<Tuple<Point, int>> searchQueue = new Queue<Tuple<Point, int>>();
            searchQueue.Enqueue(new Tuple<Point, int>(new Point(0, 0), 0));
            int longestDistance = 0;
            int longDistanceDoors = 0;
            while (searchQueue.Any())
            {
                Tuple<Point, int> searchLocation = searchQueue.Dequeue();
                longestDistance = searchLocation.Item2;
                foreach (Point adjacentPoint in doorDict[searchLocation.Item1])
                {
                    if (!visitedPoints.Contains(adjacentPoint))
                    {
                        if (searchLocation.Item2 >= 999)
                        {
                            longDistanceDoors++;
                        }

                        searchQueue.Enqueue(new Tuple<Point, int>(adjacentPoint, searchLocation.Item2 + 1));
                        visitedPoints.Add(adjacentPoint);
                    }
                }
            }

            Console.WriteLine(longestDistance);
            Console.WriteLine(longDistanceDoors);
            Console.ReadLine();
        }

        private static int GetNextCloseParenAtLevel(string input, int index)
        {
            int depth = 0;
            int searchIndex = index;
            while (depth >= 0)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (input[searchIndex])
                {
                    case '(':
                        depth++;
                        break;
                    case ')':
                        depth--;
                        break;
                }

                searchIndex++;
            }

            return searchIndex;
        }

        private static List<int> AlternateIndices(string input, int index)
        {
            int depth = 1;
            int searchIndex = index + 1;
            List<int> resultList = new List<int>{searchIndex};
            while (depth > 0)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (input[searchIndex])
                {
                    case '(':
                        depth++;
                        break;
                    case ')':
                        depth--;
                        break;
                    case '|':
                        if (depth == 1)
                            resultList.Add(searchIndex + 1);
                        break;
                }

                searchIndex++;
            }

            return resultList;
        }

        static void Day21()
        {
            Regex r = new Regex("(\\w+) (\\d+) (\\d+) (\\d+)");
            List<string> input = File.ReadAllLines("M:\\AoC\\Input_Day_21.txt").ToList();
            int instructionRegister = int.Parse(input[0].Substring("#ip ".Length));
            input.RemoveAt(0);
            List<Tuple<CPU_OPS, long[]>> parsedInstructions = new List<Tuple<CPU_OPS, long[]>>();
            foreach (string s in input)
            {
                Match m = r.Match(s);
                CPU_OPS op;
                Enum.TryParse(m.Groups[1].Value, true, out op);
                long[] instructions = { 0, long.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value) };
                parsedInstructions.Add(new Tuple<CPU_OPS, long[]>(op, instructions));

            }

            int instructionCount = 0;
            CPU powerCPU = new CPU(instructionRegister, new long[] { 1, 0,0,0,0, 0 }); //0, 9, 10551306, 0, 10551306, 10551305});//0 35 0 10550400 10551306 0
            Console.WriteLine(powerCPU.ToString());
            HashSet<long> jValues = new HashSet<long>();
            while (powerCPU.InstructionNumber < input.Count)
            {
                Tuple<CPU_OPS, long[]> parsedInstruction = parsedInstructions[(int)powerCPU.InstructionNumber];
                bool displayOutput = false;
                if (powerCPU.InstructionNumber == 30)
                {
                    if(jValues.Contains(powerCPU.registers[3]))
                        break;
                    jValues.Add(powerCPU.registers[3]);
                    Console.WriteLine("Executions: " + instructionCount + " Input Registers: " + powerCPU + " Instructions:  " + parsedInstruction.Item1 + " " + string.Join(",", parsedInstruction.Item2));
                    displayOutput = true;
                }

                powerCPU.RunOperation(parsedInstruction.Item1, parsedInstruction.Item2);
                if (displayOutput)
                {
                    Console.WriteLine("Output Registers: " + powerCPU);
                    displayOutput = false;
                }

                instructionCount++;
                //Console.ReadLine();
            }
            Console.WriteLine(powerCPU.ToString());
            Console.WriteLine("Attempts: " + instructionCount);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
        static void Day22()
        {
            Point targetLocation = new Point(8,701);
            int maxDimension = targetLocation.X + targetLocation.Y * 2;
            int[,] geologicIndexes = new int[100, maxDimension];
            char[,] cave = new char[100, maxDimension];
            int riskLevel = 0;
            geologicIndexes[0, 0] = 0;
            geologicIndexes[targetLocation.X, targetLocation.Y] = 0;
            cave[0, 0] = '.';
            //cave[targetLocation.X, targetLocation.Y] = 'T';
            for (int i = 1; i < geologicIndexes.GetLength(0); i++)
            {
                geologicIndexes[i, 0] = 16807 * i;
                cave[i, 0] = ".=|"[ErosionLevel(geologicIndexes[i,0]) % 3];
                riskLevel += ErosionLevel(geologicIndexes[i, 0]) % 3;
            }
            for (int i = 1; i < geologicIndexes.GetLength(1); i++)
            {
                geologicIndexes[0,i] = 48271* i;
                cave[0,i] = ".=|"[ErosionLevel(geologicIndexes[0,i]) % 3];
                riskLevel += ErosionLevel(geologicIndexes[0,i]) % 3;
            }

            for (int i = 1; i < geologicIndexes.GetLength(0); i++)
            {
                for (int j = 1; j < geologicIndexes.GetLength(1); j++)
                {
                    if(i==targetLocation.X && j==targetLocation.Y)
                        geologicIndexes[i, j] = 0;
                    else
                    {
                        geologicIndexes[i, j] = ErosionLevel(geologicIndexes[i - 1, j]) * ErosionLevel(geologicIndexes[i, j - 1]);
                        cave[i, j] = ".=|"[ErosionLevel(geologicIndexes[i, j]) % 3];
                        riskLevel += ErosionLevel(geologicIndexes[i, j]) % 3;
                    }
                }
            }

            List<Tuple<Point, char, int>> searchQueue = new List<Tuple<Point, char, int>>();
            searchQueue.Add(new Tuple<Point, char, int>(new Point(0,0), 'T', 0));

            Dictionary<Tuple<Point, char>, int> fastestDictionary = new Dictionary<Tuple<Point, char>, int>();

            fastestDictionary.Add(new Tuple<Point, char>(new Point(0, 0), 'T'), 0);
            int targetCost = (targetLocation.X + targetLocation.Y) * 3;

            while (searchQueue.Any())
            {
                //Tuple<Point, char> currentLocation = searchQueue.Dequeue();
                int bestCost = searchQueue.Min(l2 => l2.Item3);
                Tuple<Point, char, int> currentLocation = searchQueue.First(l1 => l1.Item3 == bestCost);
                searchQueue.Remove(currentLocation);
                int cost = fastestDictionary[new Tuple<Point, char>(currentLocation.Item1, currentLocation.Item2)];
                foreach (Tuple<Point, int, char> travellablePoints in FindTravellablePointsAndCosts(cave, currentLocation.Item1, currentLocation.Item2))
                {
                    Tuple<Point, char> state = new Tuple<Point, char>(travellablePoints.Item1, travellablePoints.Item3);
                    int newCost = cost + travellablePoints.Item2;
                    if (newCost >= targetCost)
                        continue;
                    if (!fastestDictionary.ContainsKey(state))
                    {
                        fastestDictionary.Add(state, newCost);
                        searchQueue.Add(new Tuple<Point, char, int>(state.Item1, state.Item2, newCost));
                    }
                    else if (cost + travellablePoints.Item2 < fastestDictionary[state])
                    {
                        fastestDictionary[state] = newCost;
                        Tuple<Point, char, int> temp = searchQueue.FirstOrDefault(l => l.Item1 == state.Item1 && l.Item2 == state.Item2);
                        if (temp != null)
                        {
                            searchQueue.Remove(temp);
                            searchQueue.Add(new Tuple<Point, char, int>(state.Item1, state.Item2, newCost));
                        }
                    }

                    if (state.Item1.Equals(targetLocation))
                    {
                        targetCost = newCost + (travellablePoints.Item3 != 'T' ? 7 : 0);
                    }

                }
            }
            
            PrintForest(cave);
            Console.WriteLine(riskLevel);
            Console.WriteLine("TORCH COST: " + fastestDictionary[new Tuple<Point, char>(targetLocation, 'T')]);
            Console.WriteLine("CLIMBING GEAR COST: " + fastestDictionary[new Tuple<Point, char>(targetLocation, 'C')]);
            Console.ReadLine();
        }

        private static IEnumerable<Tuple<Point, int, char>> FindTravellablePointsAndCosts(char[,] cave, Point p, char tool)
        {
            Point pUp = new Point(p.X, p.Y - 1);
            Point pDown = new Point(p.X, p.Y + 1);
            Point pLeft = new Point(p.X - 1, p.Y);
            Point pRight = new Point(p.X + 1, p.Y);
            if (pUp.Y >= 0)
            {
                Tuple<int, char> pUpStats = CalculateMovementCostAndTool(cave, p, pUp, tool);
                yield return new Tuple<Point, int, char>(pUp, pUpStats.Item1, pUpStats.Item2);
            }
            if (pLeft.X >= 0)
            {
                Tuple<int, char> pLeftStats = CalculateMovementCostAndTool(cave, p, pLeft, tool);
                yield return new Tuple<Point, int, char>(pLeft, pLeftStats.Item1, pLeftStats.Item2);
            }

            if (pRight.X < cave.GetLength(0))
            {
                Tuple<int, char> pRightStats = CalculateMovementCostAndTool(cave, p, pRight, tool);
                yield return new Tuple<Point, int, char>(pRight, pRightStats.Item1, pRightStats.Item2);
            }

            if (pDown.Y < cave.GetLength(1))
            {
                Tuple<int, char> pDownStats = CalculateMovementCostAndTool(cave, p, pDown, tool);
                yield return new Tuple<Point, int, char>(pDown, pDownStats.Item1, pDownStats.Item2);
            }
        }

        private static Tuple<int, char> CalculateMovementCostAndTool(char[,] cave, Point p1, Point p2, char tool)
        {
            int cost = 1;
            char newTool = tool;
            char p2Type = cave[p2.X, p2.Y];
            //char[,,] tools = new char[3,3,3];
            switch (cave[p1.X, p1.Y])
            {
                case '.':
                    if (p2Type == '=' && tool == 'T')
                    {
                        newTool = 'C';
                    }
                    if (p2Type == '|' && tool == 'C')
                    {
                        newTool = 'T';
                    }
                    break;
                case '=':
                    if (p2Type == '.' && tool == 'N')
                    {
                        newTool = 'C';
                    }
                    if (p2Type == '|' && tool == 'C')
                    {
                        newTool = 'N';
                    }
                    break;
                case '|':
                    if (p2Type == '=' && tool == 'T')
                    {
                        newTool = 'N';
                    }
                    if (p2Type == '.' && tool == 'N')
                    {
                        newTool = 'T';
                    }
                    break;
            }

            if (newTool != tool)
                cost += 7;
            return new Tuple<int, char>(cost, newTool);
        }

        private static int ErosionLevel(int geologicIndex)
        {
            const int depth = 5913;
            return (geologicIndex + depth) % 20183;
        }

        static void Day23()
        {
            Regex r = new Regex("pos=<([-\\d]+),([-\\d]+),([-\\d]+)>, r=(\\d+)");
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_23.txt");
            int maxPower= int.MinValue;
            int[] maxPowerLocation = null;
            foreach (string s in input)
            {
                Match m = r.Match(s);
                int[] details = { int.Parse(m.Groups[1].Value) , int.Parse(m.Groups[2].Value) , int.Parse(m.Groups[3].Value) , int.Parse(m.Groups[4].Value) };
                if (details[3] > maxPower)
                {
                    maxPower = details[3];
                    maxPowerLocation = new[]{details[0], details[1], details[2]};
                }
            }
            int count = 0;
            foreach (string s in input)
            {
                Match m = r.Match(s);
                int[] details = { int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value) };
                int distance = Math.Abs(details[0] - maxPowerLocation[0]) + Math.Abs(details[1] - maxPowerLocation[1]) + Math.Abs(details[2] - maxPowerLocation[2]);
                if (distance <= maxPower)
                    count++;
            }
            Console.WriteLine(count);
            Console.ReadLine();

        }
        static void Day23B()
        {
            Regex r = new Regex("pos=<([-\\d]+),([-\\d]+),([-\\d]+)>, r=(\\d+)");
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_23.txt");
            int nanobotCount = input.Length;
            int[][] nanobots = new int[nanobotCount][];
            int index = 0;
            foreach (string s in input)
            {
                Match m = r.Match(s);
                nanobots[index]=new [] { int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value) };
                index++;
            }

            int maxRanges = int.MinValue;
            int[] maxRangeLocation = null;
            //for (int i = 0; i < nanobotCount; i++)
            //{
            //    foreach (int[] vertex in FindVertices(nanobots[i]))
            //    {
            //        //23933222, 26943113, 44664676), Manhattan sum 95541011, in range of 975 bots
            //        if (SequenceEquals(vertex, new[]{23933222, 26943113, 44664676}))
            //            index = 42;
                    
            //        int myRanges = CheckPoint(nanobots, vertex);
            //        if (myRanges > maxRanges)
            //        {
            //            maxRanges = myRanges;
            //            maxRangeLocation = vertex;
            //        }
            //        else if (myRanges == maxRanges)
            //        {
            //            if (Math.Abs(vertex[0]) + Math.Abs(vertex[1]) + Math.Abs(vertex[2]) < Math.Abs(maxRangeLocation[0]) + Math.Abs(maxRangeLocation[1]) + Math.Abs(maxRangeLocation[2]))
            //                maxRangeLocation = vertex;
            //        }
            //    }
            //}
            HashSet<int[]> pointsToCheck = new HashSet<int[]>();
            for (int i = 0; i < nanobots.Length; i++)
            {
                for (int j = 0; j < nanobots.Length; j++)
                {
                    if (i == j)
                        continue;

                    foreach (int[] checkableVertex in FindIntersectionVertices(nanobots[i], nanobots[j]))
                    {
                        pointsToCheck.Add(checkableVertex);
                    }
                }
            }

            foreach (int[] checkableVertex in pointsToCheck)
            {
                int myRanges = CheckPoint(nanobots, checkableVertex);
                if (myRanges > maxRanges)
                {
                    maxRanges = myRanges;
                    maxRangeLocation = checkableVertex;
                }
                else if (myRanges == maxRanges)
                {
                    if (Math.Abs(checkableVertex[0]) + Math.Abs(checkableVertex[1]) + Math.Abs(checkableVertex[2]) < Math.Abs(maxRangeLocation[0]) + Math.Abs(maxRangeLocation[1]) + Math.Abs(maxRangeLocation[2]))
                        maxRangeLocation = checkableVertex;
                }
            }


            Console.WriteLine(maxRanges);
            Console.WriteLine(string.Join(" ", maxRangeLocation.Select(x => x.ToString())));

            //while (true)
            //{
            //    int[] newLocation = maxRangeLocation;

            //    int xDif = Math.Max(1, Math.Min(-1, newLocation[0]));
            //    int[] trialLocation = new[]{newLocation[0] - xDif, newLocation[1], newLocation[2]};
            //    if (maxRanges <= CheckPoint(nanobots, trialLocation))
            //        newLocation = trialLocation;

            //    int yDif = Math.Max(1, Math.Min(-1, newLocation[1]));
            //    trialLocation = new[] { newLocation[0], newLocation[1]-yDif, newLocation[2] };
            //    if (maxRanges <= CheckPoint(nanobots, trialLocation))
            //        newLocation = trialLocation;

            //    int zDif = Math.Max(1, Math.Min(-1, newLocation[2]));
            //    trialLocation = new[] { newLocation[0], newLocation[1], newLocation[2]-zDif };
            //    if (maxRanges <= CheckPoint(nanobots, trialLocation))
            //        newLocation = trialLocation;

            //    if (newLocation == maxRangeLocation)
            //        break;
            //    maxRangeLocation = newLocation;
            //    Console.WriteLine(CheckPoint(nanobots, maxRangeLocation));
            //    Console.WriteLine(string.Join(" ", maxRangeLocation.Select(x => x.ToString())));
            //}
            Console.WriteLine(maxRangeLocation.Sum(Math.Abs));
            Console.ReadLine();

        }

        private static int CheckPoint(int[][] nanobots, int[] p)
        {
            int result = 0;
            for (int i = 0; i < nanobots.Length; i++)
            {
                int distance = Math.Abs(p[0] - nanobots[i][0]) + Math.Abs(p[2] - nanobots[i][2]) + Math.Abs(p[1] - nanobots[i][1]);
                if (distance <= nanobots[i][3])
                    result++;
            }

            return result;
        }

        private static IEnumerable<int[]> FindVertices(int[] nanobot)
        {
            yield return new []{nanobot[0], nanobot[1], nanobot[2] - nanobot[3]};
            yield return new []{nanobot[0], nanobot[1], nanobot[2] + nanobot[3]};
            yield return new []{nanobot[0], nanobot[1] - nanobot[3], nanobot[2]};
            yield return new []{nanobot[0], nanobot[1] + nanobot[3], nanobot[2]};
            yield return new []{nanobot[0] - nanobot[3], nanobot[1], nanobot[2]};
            yield return new []{nanobot[0] + nanobot[3], nanobot[1], nanobot[2]};
            yield return new[] { nanobot[0], nanobot[1], nanobot[2] };
        }

        private static HashSet<int[]> FindIntersectionVertices(int[] nanobotA, int[] nanobotB)
        {
            int[] nanobotBOffset = {nanobotB[0]-nanobotA[0], nanobotB[1] - nanobotA[1] , nanobotB[2] - nanobotA[2], nanobotB[3] };
            List<int[]> overlappingOffsetVertices = new List<int[]>();
            foreach (int[] vertex in FindVertices(nanobotBOffset))
            {
                if (Math.Abs(vertex[0]) + Math.Abs(vertex[1]) + Math.Abs(vertex[2]) > nanobotA[3])
                    overlappingOffsetVertices.Add(vertex);
                // new[]{vertex[0] + nanobotA[0], vertex[1] + nanobotA[1], vertex[2] + nanobotA[2]};
            }
            HashSet<int[]> finalOutputVertices = new HashSet<int[]>();
            foreach (int[] overlappingOffsetVertex in overlappingOffsetVertices)
            {
                
                int vertexSum = overlappingOffsetVertex.Sum(Math.Abs);
                int manhattanDiff = (nanobotA[3] - vertexSum) / 2;
                //int evenSplit = (nanobotA[3] - vertexSum) % 2;

                HashSet<int[]> subset = new HashSet<int[]>();
                finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2]});
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1] + manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] - manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1] + manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1] - manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] - manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1] - manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] + manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] + manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] - manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] + manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] + manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] - manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] - manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2] - manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] + manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] - manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] + manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] - manhattanDiff });
                subset.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] - manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] - manhattanDiff });
                //finalOutputVertices.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] });
                //finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0] - manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2]});
                //finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] + manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2]});
                //finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1] - manhattanDiff, overlappingOffsetVertex[2] + nanobotA[2]});
                //finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] + manhattanDiff});
                //finalOutputVertices.Add(new[]{overlappingOffsetVertex[0] + nanobotA[0], overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] - manhattanDiff});
                //finalOutputVertices.Add(new[] { overlappingOffsetVertex[0] + nanobotA[0] + manhattanDiff, overlappingOffsetVertex[1] + nanobotA[1], overlappingOffsetVertex[2] + nanobotA[2] });
                foreach (int[] intse in subset)
                {
                    int distance = Math.Abs(intse[0] - nanobotA[0]) + Math.Abs(intse[2] - nanobotA[2]) + Math.Abs(intse[1] - nanobotA[1]);
                    if (distance >= vertexSum)
                        finalOutputVertices.Add(intse);
                }
            }

            return finalOutputVertices;
        }

        private static IEnumerable<int[]> FindIntersectionLines(int[] vertices1, int[] vertices2)
        {
            int[] intersectionVector = {vertices1[1] * vertices2[2]};
            return null;
        }

        private static bool SequenceEquals(int[] a, int[] b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            if (a.GetLength(0) != b.GetLength(0))
                return false;
            for (int i = 0; i < a.GetLength(0); i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        static void Day24()
        {
            Regex r = new Regex("(\\d+) units each with (\\d+) hit points ?(\\(.*\\))? with an attack that does (\\d+) ([a-z]+) damage at initiative (\\d+)");
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_24.txt");
            List<Army> immuneSystem = new List<Army>();
            List<Army> infection = new List<Army>();
            ReloadFile(input, immuneSystem, infection, r);
            int powerBoost = 37;
            while (infection.Any())
            {
                immuneSystem.Clear();
                infection.Clear();
                ReloadFile(input, immuneSystem, infection, r);
                powerBoost++;
                List<Army> everyone = RunRoundWithBoosts(immuneSystem, infection, powerBoost);
                //foreach (Army a in everyone)
                //{
                //    Console.WriteLine(a.ToString());
                //}
            }
            Console.WriteLine(powerBoost);
            Console.ReadLine();
        }

        private static void ReloadFile(string[] input, List<Army> immuneSystem, List<Army> infection, Regex r)
        {
            List<Army> listToAdd = null;
            int idCount = 1;
            int attackBoost = 0;
            foreach (string line in input)
            {
                if (line == "Immune System:")
                {
                    listToAdd = immuneSystem;
                    idCount = 101;
                    continue;
                }

                if (line == "Infection:")
                {
                    listToAdd = infection;
                    idCount = 201;
                    continue;
                }

                if (line == "")
                    continue;

                Match m = r.Match(line);
                string[] weaknesses = new string[0];
                string[] immunities = new string[0];
                if (m.Groups[3].Value != "")
                {
                    string elementString = m.Groups[3].Value;
                    string[] halfParsed = elementString.Substring(1, elementString.Length - 2).Split(';').Select(s => s.Trim()).ToArray();
                    foreach (string s in halfParsed)
                    {
                        if (s.StartsWith("weak"))
                        {
                            weaknesses = s.Substring("weak to ".Length).Split(',').Select(s2 => s2.Trim()).ToArray();
                        }
                        else if (s.StartsWith("immune"))
                        {
                            immunities = s.Substring("immune to ".Length).Split(',').Select(s2 => s2.Trim()).ToArray();
                        }
                        else
                            throw new Exception();
                    }
                }

                listToAdd.Add(new Army(idCount, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), weaknesses, immunities, int.Parse(m.Groups[4].Value), m.Groups[5].Value, int.Parse(m.Groups[6].Value)));
                idCount++;
            }
        }

        private static List<Army> RunRoundWithBoosts(List<Army> immuneSystem, List<Army> infection, int powerBoost)
        {
            List<Army> everyone = new List<Army>();
            foreach (Army army in immuneSystem)
            {
                army.PowerBoost = powerBoost;
            }
            everyone.AddRange(immuneSystem);
            everyone.AddRange(infection);
            infection.Sort();
            immuneSystem.Sort();
            everyone.Sort();
            while (infection.Any() && immuneSystem.Any())
            {
                Dictionary<Army, Army> targetSelections = new Dictionary<Army, Army>();
                List<Army> infectionTargets = new List<Army>(immuneSystem);
                List<Army> immuneTargets = new List<Army>(infection);
                foreach (Army army in SelectionOrder(everyone))
                {
                    if (infection.Contains(army))
                    {
                        Army target = army.SelectTarget(infectionTargets);
                        if (target == null || army.CalculateDamageTo(target) == 0)
                        {
                            continue;
                        }

                        targetSelections.Add(army, target);
                        infectionTargets.Remove(target);
                    }

                    if (immuneSystem.Contains(army))
                    {
                        Army target = army.SelectTarget(immuneTargets);
                        if (target == null)
                        {
                            continue;
                        }

                        targetSelections.Add(army, target);
                        immuneTargets.Remove(target);
                    }
                }

                foreach (Army army in everyone)
                {
                    if (targetSelections.ContainsKey(army) && army.Units > 0)
                    {
                        targetSelections[army].TakeDamage(army);
                        if (targetSelections[army].Units <= 0)
                        {
                            immuneSystem.Remove(targetSelections[army]);
                            infection.Remove(targetSelections[army]);
                        }
                    }
                }

                everyone.Clear();
                everyone.AddRange(infection);
                everyone.AddRange(immuneSystem);
                everyone.Sort();
                
            }
            Console.WriteLine("Remaining Units: " + everyone.Sum(a => a.Units));
            return everyone;
        }

        private static IEnumerable<Army> SelectionOrder(List<Army> armies)
        {
            List<Army> armyCopy = armies.ToArray().ToList();
            while (armyCopy.Any())
            {
                Army targetArmy = null;
                foreach (Army army in armyCopy)
                {
                    if (targetArmy == null)
                    {
                        targetArmy = army;
                        continue;
                    }

                    if (targetArmy.EffectivePower < army.EffectivePower)
                    {
                        targetArmy = army;
                    }
                    else if (targetArmy.EffectivePower == army.EffectivePower)
                    {
                        if (targetArmy.Initiative < army.Initiative)
                        {
                            targetArmy = army;
                        }
                    }
                }
                armyCopy.Remove(targetArmy);
                yield return targetArmy;
            }

            //return targetArmy;
        }

        private class Army : IComparable<Army>
        {
            private int id;
            public int Units;
            private int HealthPerUnit;
            private string[] Weaknesses;
            private string[] Immunities;
            public int Damage;
            private string DamageType;
            public int Initiative;
            public int PowerBoost;

            public Army(int id, int units, int healthPerUnit, string[] weaknesses, string[] immunities, int damage, string damageType, int initiative)
            {
                Units = units;
                HealthPerUnit = healthPerUnit;
                Weaknesses = weaknesses;
                Immunities = immunities;
                Damage = damage;
                DamageType = damageType;
                Initiative = initiative;
                this.id = id;
                PowerBoost = 0;
            }

            public int EffectivePower => Units * (Damage+ PowerBoost);

            public int CalculateDamageTo(Army army)
            {
                return EffectivePower * (army.Weaknesses.Contains(DamageType) ? 2 : 1) * (army.Immunities.Contains(DamageType) ? 0 : 1);
            }
            public int CalculateDamageFrom(Army army)
            {
                return army.EffectivePower * (Weaknesses.Contains(army.DamageType) ? 2 : 1) * (Immunities.Contains(army.DamageType) ? 0 : 1);
            }

            public Army SelectTarget(List<Army> availableTargets)
            {
                Army targetArmy = null;
                foreach (Army army in availableTargets)
                {
                    if (army == this)
                        continue;
                    if (targetArmy == null)
                    {
                        targetArmy = army;
                        continue;
                    }

                    if (CalculateDamageTo(targetArmy) < CalculateDamageTo(army))
                    {
                        targetArmy = army;
                    }
                    else if (CalculateDamageTo(targetArmy) == CalculateDamageTo(army))
                    {
                        if (targetArmy.EffectivePower < army.EffectivePower)
                        {
                            targetArmy = army;
                        }
                        else if (targetArmy.EffectivePower == army.EffectivePower)
                        {
                            if (targetArmy.Initiative < army.Initiative)
                            {
                                targetArmy = army;
                            }
                        }
                    }
                }

                //if(targetArmy != null)
                    //Console.WriteLine("Army " + id + " will attack Army " + targetArmy.id);
                //else
                    //Console.WriteLine("Army " + id + " has no targets");
                return targetArmy;
            }

            public void TakeDamage(Army army)
            {
                int damage = CalculateDamageFrom(army);
                //Console.WriteLine("Army " + army.id + " attacks Army " + id + " dealing " + damage + " damage and killing " + damage/HealthPerUnit + " units.");
                while (damage >= HealthPerUnit)
                {
                    damage -= HealthPerUnit;
                    Units--;
                    if (Units <= 0)
                        break;
                }
            }

            public int CompareTo(Army other)
            {
                return -Initiative.CompareTo(other.Initiative);
            }

            public override string ToString()
            {
                return id.ToString()+": " + Units;
            }
        }

        static void Day25()
        {
            List<int[]> points = new List<int[]>();
            string[] input = File.ReadAllLines("M:\\AoC\\Input_Day_25.txt");
            Dictionary<int, int> constellationPoints = new Dictionary<int, int>();
            foreach (string s in input)
            {
                string[] splitString = s.Split(',');
                points.Add(new[] { int.Parse(splitString[0]) , int.Parse(splitString[1]) , int.Parse(splitString[2]) , int.Parse(splitString[3]) });
                constellationPoints.Add(points.Count - 1, points.Count - 1);
            }

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (constellationPoints[i] == constellationPoints[j])
                        continue;
                    int manhattanDistance = Math.Abs(points[i][0] - points[j][0]) + Math.Abs(points[i][1] - points[j][1]) + Math.Abs(points[i][2] - points[j][2]) + Math.Abs(points[i][3] - points[j][3]);
                    if (manhattanDistance <= 3)
                    {
                        int iConst = constellationPoints[i];
                        int jConst = constellationPoints[j];
                        for (int k = 0; k < points.Count; k++)
                        {
                            if (constellationPoints[k] == iConst)
                            {
                                constellationPoints[k] = jConst;
                            }
                        }
                    }
                }
            }
            Console.WriteLine(constellationPoints.Select(c => c.Value).Distinct().Count());
            Console.ReadLine();
        }
    }
}
