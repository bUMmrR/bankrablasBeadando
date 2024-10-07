using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Sherrif: Characther
    {
        private bool _seen;

        public override bool seen
        {
            get { return _seen; }
            set { _seen = true; }
        }

        private int _health;

        public override int health
        {
            get { return _health; }
            set { _health = value; }
        }

        private int _goldCount;

        public override int goldCount
        {
            get { return _goldCount; }
            set { _goldCount = value; }
        }

        public override bool stepable => true;

        public Sherrif()
        {
            health = 100;
            goldCount = 0;
        }

        public string currentMoveString = "";

        public int dmg = new Random().Next(20, 35);

        private static readonly int[,] Directions = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1},
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1}
        };

        public void move(ref List<List<VarosElem>> t, ref (int, int) sherrifCurrentPos, ref List<(int, int)> sheriffCurrentMoves)
        {
            (int, int) currentMove = sheriffCurrentMoves.First();
            sheriffCurrentMoves.Remove(currentMove);


            VarosElem ahovaMozog = t[currentMove.Item1][currentMove.Item2];



            if (ahovaMozog is Ground)
            {

                t[currentMove.Item1][currentMove.Item2] = t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2];
                t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2] = new Ground();
                sherrifCurrentPos = currentMove;
                makeSeen(currentMove.Item1, currentMove.Item2, ref t);
            }
            else if (ahovaMozog is Kobambi)
            {
                t[currentMove.Item1][currentMove.Item2] = t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2];
                t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2] = new Ground();
                sherrifCurrentPos = currentMove;
                makeSeen(currentMove.Item1, currentMove.Item2, ref t);
                health += 50;
                currentMoveString = " ";
                Random r = new Random();
                while (true)
                {
                    int x = r.Next(25);
                    int y = r.Next(25);
                    if (t[x][y] is Ground)
                    {
                        if (t[x][y].seen == true)
                        {
                            t[x][y] = new Kobambi();
                            t[x][y].seen = true;
                        }
                        else
                        {
                            t[x][y] = new Kobambi();
                            t[x][y].seen = false;
                        }

                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if (ahovaMozog is Gold)
            {
                t[currentMove.Item1][currentMove.Item2] = t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2];
                t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2] = new Ground();
                sherrifCurrentPos = currentMove;
                goldCount++;
                makeSeen(currentMove.Item1, currentMove.Item2, ref t);
            }
            else if (ahovaMozog is Varoshaza)
            {
                var varos = new Varos();
                varos.gameWon();
            }
        }

        public List<(int, int)> think(ref List<List<VarosElem>> t, ref (int, int) sherrifCurrentPos)
        {
            Random r = new Random();

            int x = r.Next(25);
            int y = r.Next(25);

            while (true)
            {
                var cucc = GetPath(t, sherrifCurrentPos, (x, y));
                if (cucc != null){
                    return cucc;
                }
            }
        }

        public void makeSeen(int x, int y, ref List<List<VarosElem>> t)
        {
            for (int i = 0; i < Directions.GetLength(0); i++)
            {
                int neighborX = x + Directions[i, 0];
                int neighborY = y + Directions[i, 1];

                if (neighborX >= 0 && neighborX < t.Count && neighborY >= 0 && neighborY < t[neighborX].Count)
                {
                    t[neighborX][neighborY].seen = true;
                }
            }
        }


        public List<(int, int)> GetPath(List<List<VarosElem>> t, (int x, int y) start, (int x, int y) target)
        {

            Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
            queue.Enqueue(start);

            Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();

            HashSet<(int, int)> visited = new HashSet<(int, int)> { start };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var (currentX, currentY) = current;


                //if (currentX == target.x && currentY == target.y)
                //    return ReconstructPath(cameFrom, (target.x, target.y));

                for (int i = 0; i < Directions.GetLength(0); i++)
                {
                    int neighborX = currentX + Directions[i, 0];
                    int neighborY = currentY + Directions[i, 1];

                    if (neighborX == target.x && neighborY == target.y)
                    {

                        var neighbor = (neighborX, neighborY);
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = (currentX, currentY);
                        return ReconstructPath(cameFrom, (target.x, target.y));
                    }

                    if (IsValidPosition(t, neighborX, neighborY, visited))
                    {
                        var neighbor = (neighborX, neighborY);
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = (currentX, currentY);
                    }
                }
            }

            return null;
        }

        private static bool IsValidPosition(List<List<VarosElem>> t, int x, int y, HashSet<(int, int)> visited)
        {

            return x >= 0 && y >= 0 && x < 25 && y < 25 &&
                   t[x][y].stepable == true && !visited.Contains((x, y));
        }

        private static List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int x, int y) current)
        {
            var totalPath = new List<(int, int)> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            totalPath.RemoveAt(0);
            return totalPath;
        }

        public (int, int) findClosest(List<List<VarosElem>> t, int currentX, int currentY, VarosElem gadzi)
        {
            int closestDistance = int.MaxValue;
            (int, int) closest = (-1,-1);

            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[i].Count; j++)
                {
                    if (t[i][j].GetType() == gadzi.GetType() && t[i][j].seen == true)
                    {
                        int distance = Math.Abs(currentX - i) + Math.Abs(currentY - j);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closest = (i, j);
                        }
                    }
                }
            }

            return closest ;
        }




        public bool OnStep(int x, int y, ref List<List<VarosElem>> t,bool fight)
        {
            bool ret = false;
            for (int i = 0; i < Directions.Length / 2; i++)
            {
                int neighborX = x + Directions[i, 0];
                int neighborY = y + Directions[i, 1];
                if (neighborX >= 0 && neighborX < t.Count && neighborY >= 0 && neighborY < t[neighborX].Count && t[neighborX][neighborY].GetType() == typeof(Bandita))
                {
                    var bandita = t[neighborX][neighborY];
                    if (bandita is Bandita ba)
                    {
                        if (fight)
                        {
                            ba.health -= dmg;
                            if (ba.health <= 0)
                            {
                                t[neighborX][neighborY] = new Ground();
                                t[neighborX][neighborY].seen = true;
                                goldCount += ba.goldCount;
                            }
                        }
                        Console.WriteLine("Bandita hp: " + ba.health);
                        ret = true;
                    }
                }

            }
            return ret;
        }


        public override string ToString()
        {
            return ("S");
        }

        public (int,int) findClosestUnseen(List<List<VarosElem>> t, int currentX, int currentY)
        {
            int closestDistance = int.MaxValue;
            (int, int) closest = (-1, -1);


            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[i].Count; j++)
                {
                    if (t[i][j].seen == false && t[i][j].stepable == true && i != currentX && j != currentY)
                    {
                        int distance = Math.Abs(currentX - i) + Math.Abs(currentY - j);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closest = (i, j);
                        }
                    }
                }
            }

            return closest;
        }
    }
}
