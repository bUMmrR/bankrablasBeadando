using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Bandita : Characther
    {
        private bool _seen;

        public override bool seen
        {
            get { return _seen; }
            set { _seen = value; }
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


        static int count = 0;

        public int id;

        public Bandita()
        {
            health = 100;
            id = count;
            count++;
            seen = false;
            goldCount = 0;
        }

        private static readonly int[,] Directions = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1},
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1}
        };

        public void move(int x, int y, ref List<List<VarosElem>> t)
        {
            Random random = new Random();
            while (true)
            {
                if(OnStep(x, y, ref t))
                {
                    break;
                };
                //kell nezni van e arany
                for (int i = 0; i < Directions.Length/2; i++)
                {
                    int neighborXtemp = x + Directions[i, 0];
                    int neighborYtemp = y + Directions[i, 1];
                    if (neighborXtemp >= 0 && neighborXtemp < t.Count && neighborYtemp >= 0 && neighborYtemp < t[neighborXtemp].Count && t[neighborXtemp][neighborYtemp].GetType() == typeof(Gold))
                    {
                        var currentBandit = t[x][y];
                        var targetGround = t[neighborXtemp][neighborYtemp];

                        var newGround = new Ground();

                        newGround.seen = currentBandit.seen;
                        currentBandit.seen = targetGround.seen;

                        OnStep(neighborXtemp, neighborYtemp, ref t);

                        t[neighborXtemp][neighborYtemp] = currentBandit;
                        t[x][y] = newGround;
                        goldCount++;
                    }
                }

                int rnd = random.Next(Directions.Length/2);
                int neighborX = x + Directions[rnd, 0];
                int neighborY = y + Directions[rnd, 1];
                if (neighborX >= 0 && neighborX < t.Count && neighborY >= 0 && neighborY < t[neighborX].Count && t[neighborX][neighborY] is Ground)
                {
                    
                    var currentBandit = t[x][y]; 
                    var targetGround = t[neighborX][neighborY]; 
    
                    var newGround = new Ground();
    
                    newGround.seen = currentBandit.seen;
                    currentBandit.seen = targetGround.seen;

                    OnStep(neighborX, neighborY, ref t);

                    t[neighborX][neighborY] = currentBandit;
                    t[x][y] = newGround;

                    //ha látja a sheriffet akkor üssön


                    break;
                }

            }
        }


        public bool OnStep(int x,int y, ref List<List<VarosElem>> t)
        {
            for (int i = 0; i < Directions.Length/2; i++)
            {
                int neighborX = x + Directions[i, 0];
                int neighborY = y + Directions[i, 1];
                if (neighborX >= 0 && neighborX < t.Count && neighborY >= 0 && neighborY < t[neighborX].Count && t[neighborX][neighborY].GetType() == typeof(Sherrif))
                {
                    var sherrif = t[neighborX][neighborY];
                    if (sherrif is Sherrif sh)
                    {
                        sh.health -= new Random().Next(5, 15);
                        return true;
                    }
                }
            }
            return false;
        }


        public override string ToString()
        {
            return ("B");
        }
    }
}
