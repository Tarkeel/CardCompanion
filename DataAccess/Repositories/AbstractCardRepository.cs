using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccess.Types;

namespace DataAccess.Repositories
{
    public abstract class AbstractCardRepository
    {
        public abstract Card GetCard(int ID);
        public abstract Card GetCard(Game game, string code);
        public abstract Card CreateOrGetCard(Game game, string code, bool persist = true);
        public abstract void UpdateCard(Card updated, bool persist = true);
        public abstract void DeleteCard(Card deleted, bool cascade = false);
    }
}
