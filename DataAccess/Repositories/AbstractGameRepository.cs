using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccess.Types;

namespace DataAccess.Repositories
{
    public abstract class AbstractGameRepository
    {
        public abstract ObservableGameCollection All { get; }
        public abstract Game GetGame(int ID);
        public abstract Game GetGame(string title);
        public abstract Game CreateOrGetGame(string title, bool persist = true);
        public abstract void UpdateGame(Game updated, bool persist = true);
        public abstract void DeleteGame(Game deleted, bool cascade = false);
    }
}
