using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MONOPOLY
{
    interface ICell
    {
        string Name { get; }
        void Play(Player player);
    }
}
