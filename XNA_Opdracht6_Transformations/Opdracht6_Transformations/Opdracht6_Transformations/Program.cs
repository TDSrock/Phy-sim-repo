﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opdracht6_Transformations
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new SimPhyGameWorld())
                game.Run();
        }
    }
}
