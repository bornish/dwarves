using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGame.Common.Types;

namespace WebGame.Common.Display
{
    interface Map
    {
        void Update(dynamic tiles, int v1, int v2);
    }
}
