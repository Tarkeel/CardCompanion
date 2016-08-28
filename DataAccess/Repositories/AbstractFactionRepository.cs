using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccess.Types;

namespace DataAccess.Repositories
{
    public abstract class AbstractFactionRepository
    {
        public abstract Faction GetFaction(int ID);
        public abstract Faction GetFaction(Game game, string title);
        public abstract Faction CreateOrGetFaction(Game game, string title, bool persist = true);
        public abstract void UpdateFaction(Faction updated, bool persist = true);
        public abstract void DeleteFaction(Faction deleted, bool cascade = false);
    }
}
