using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Varos
    {
        public static List<List<VarosElem>> t = new List<List<VarosElem>>();

        static List<(int, int)> sheriffCurrentMoves = new List<(int, int)>();

        static (int, int) sherrifCurrentPos;

        //1 sheriff 5 bandita 5 arany 3 kobi 1 varoshaza


        Random r = new Random();

        public void gameLost()
        {
            Console.Clear();
            Console.WriteLine("Játék vége, Vesztettel");
            Console.ReadLine();
            Environment.Exit(0);

        }

        public void gameWon()
        {
            Console.Clear();
            Console.WriteLine("Játék vége, Nyertel");
            Console.ReadLine();
            Environment.Exit(0);
        }

        public void init()
        {
            baseGen();
            baricadeGen();
            remainingGen();
            kiiras();
        }

        public void kiiras()
        {
            Console.Clear();
            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[0].Count; j++)
                {
                    VarosElem temp = t[i][j];
                    if (temp.seen == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (temp is Sherrif)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    if(temp.stepable == false)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    }
                    Console.Write(temp.ToString());
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                    Console.ForegroundColor = ConsoleColor.White;

                    //return;
                }
                Console.WriteLine();
            }
            if(t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2] is Sherrif sh)
            {
                if (sh.health <= 0)
                {
                    gameLost();
                }
                Console.WriteLine("Sherrif hp: " + sh.health);
                Console.WriteLine("Sherrif arany: " + sh.goldCount);

            }
        }

        private void remainingGen()
        {
            //1 sheriff 5 bandita 5 arany 3 kobi 1 varoshaza
            placeStuff(new Sherrif());
            for (int i = 0; i < 5; i++)
            {
                placeStuff(new Bandita());
            }
            for (int i = 0; i < 5; i++)
            {
                placeStuff(new Gold());
            }
            for (int i = 0; i < 3; i++)
            {
                placeStuff(new Kobambi());
            }
            placeStuff(new Varoshaza());


        }

        private void placeStuff(VarosElem gadzi)
        {
            while (true)
            {
                int x = r.Next(25);
                int y = r.Next(25);
                if (t[x][y] is Ground)
                {
                    t[x][y] = gadzi;
                    if (gadzi is Bandita bandita)
                    {
                        Console.WriteLine(bandita.id);
                    }
                    if (gadzi is Sherrif sh)
                    {
                        sherrifCurrentPos = (x, y);
                        sh.makeSeen(x,y,ref t);
                    }

                    break;
                }
                else
                {
                    continue;
                }
            }
        }


        private void baricadeGen()
        {

            int barricadesPlaced = 0;

            //a szam a barikadok szama
            while (barricadesPlaced < 100)
            {
                int x = r.Next(25);
                int y = r.Next(25);

                if (!(t[x][y] is Barikad))
                {
                    bool[,] visited = new bool[25, 25];

                    int reachableCount = countReachable(x, y, visited);


                    if (reachableCount == 625-barricadesPlaced)
                    {
                        Barikad barikad = new Barikad();
                        t[x][y] = barikad;
                        barricadesPlaced++;
                    }

                }
            }

        }

        static int countReachable(int x, int y, bool[,] visited)
        {
            int[,] directions = new int[,]
            {
                    { -1, -1 }, { -1, 0 }, { -1, 1 },
                    { 0, -1 },           { 0, 1 },
                    { 1, -1 }, { 1, 0 }, { 1, 1 }
            };

            if (x < 0 || y < 0 || x >= 25 || y >= 25 || t[x][y] is Barikad || visited[x, y])
            {
                return 0;
            }

            visited[x, y] = true;

            int count = 1;

            for (int d = 0; d < 8; d++)
            {
                int newX = x + directions[d, 0];
                int newY = y + directions[d, 1];
                count += countReachable(newX, newY, visited);
            }

            return count;
        }

        private void baseGen()
        {
            for (int i = 0; i < 25; i++)
            {
                List<VarosElem> tempSor = new List<VarosElem>();
                for (int j = 0; j < 25; j++)
                {
                    Ground g = new Ground();
                    tempSor.Add(g);
                }
                t.Add(tempSor);
            }
        }


        public void sim()
        {
            VarosElem tempSheriff = t[sherrifCurrentPos.Item1][sherrifCurrentPos.Item2];

            while (true)
            {
                //csak hogy lehessen sherrif
                if (tempSheriff is Sherrif sh)
                {
                    //ha epp kuzdene,ne menjen el, csak ha keves hp
                    if(sh.goldCount == 5)
                    {
                        sheriffCurrentMoves = null;
                        var cucc = sh.findClosest(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, new Varoshaza());
                        // ha talalt varoshaza
                        if (cucc.Item1 != -1)
                        {
                            sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, cucc);
                            sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                        }
                        //menjen valami uresre
                        else
                        {
                            var legkozelebb = sh.findClosestUnseen(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2);
                            sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, legkozelebb);
                            sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                        }
                    }
                    else if (sh.health < 50)
                    {
                        if (sheriffCurrentMoves.Count != 0)
                        {
                            sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                        }
                        else
                        {
                            var kobi = sh.findClosest(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, new Kobambi());

                            // ha talalt varoshazat
                            if (kobi.Item1 != -1)
                            {
                                sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, kobi);
                                sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                            }
                            //menjen valami uresre
                            else
                            {
                                var igen = sh.findClosestUnseen(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2);
                                sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, igen);
                            }
                        }

                        //menjen, vagy talaljon kobambi
                    }
                    //küzd egy banditaval
                    else if (sh.OnStep(sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, ref t, false))
                    {
                        sh.OnStep(sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, ref t, true);
                    }
                    //nincs most lepese, de nincs is bajban
                    else if (sheriffCurrentMoves.Count == 0)
                    {
                        var arany = sh.findClosest(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, new Gold());
                        if (arany.Item1 != -1)
                        {
                            sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, arany);
                            sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                        }
                        else
                        {
                            var legkozelebb = sh.findClosestUnseen(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2);
                            if (legkozelebb.Item1 != -1)
                            {
                                sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, legkozelebb);
                                sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                            }
                            var legkozelebbbBandita = sh.findClosest(t, sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, new Bandita());
                            if (legkozelebbbBandita.Item1 != -1)
                            {
                                sheriffCurrentMoves = sh.GetPath(t, sherrifCurrentPos, legkozelebbbBandita);
                                sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                            }
                        }
                    }
                    else
                    {
                        sh.move(ref t, ref sherrifCurrentPos, ref sheriffCurrentMoves);
                    }
                    
                    sh.OnStep(sherrifCurrentPos.Item1, sherrifCurrentPos.Item2, ref t, true);
                }

                //ha minden beszarik legalabb menjen valahova

                banditaLepesek();

                kiiras();
                Thread.Sleep(500);


            }

            // ha seriff csinal valamit folytassa ha minden jo
            // lep a seriff

            // ha valami kulonlegesre lep akkor annak a cucca tortenjen meg
            // bandita lep, ut ha neki van kore



        }

        private void banditaLepesek()
        {
            List<int> seenIds = new List<int>();
            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[0].Count; j++)
                {
                    VarosElem temp = t[i][j];
                    if (temp is Bandita ba && !seenIds.Contains(ba.id))
                    {
                        ba.move(i,j,ref t);
                        seenIds.Add(ba.id);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}


            //foreach ((int, int) tempSor in sheriffCurrentMoves)
            //{
            //    Console.WriteLine($"{tempSor.Item1}x, {tempSor.Item2}y");
            //}
