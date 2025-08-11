using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Constants
{
    public static class Settings
    {
        public static readonly object Environments = new { Development = "Development", Staging = "Staging", Production = "Production" };

        public static readonly object Sections = new { Mongo = "Mongo", Redis = "Redis" };

    }
}
