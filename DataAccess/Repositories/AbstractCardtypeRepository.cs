using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    public abstract class AbstractCardtypeRepository
    {
        public abstract Cardtype GetCardtype(int ID);
        public abstract Cardtype GetCardtype(Game game, string title);
        public abstract Cardtype CreateOrGetCardtype(Game game, string title, bool persist = true);
        public abstract void UpdateCardtype(Cardtype updated, bool persist = true);
        public abstract void DeleteCardtype(Cardtype deleted, bool cascade = false);
    }
}
