using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public abstract class AbstractRepositoryFactory
    {
        /// <summary>
        /// Persist the repository. Exact details depend on which implementation is in use.
        /// </summary>
        public abstract void Persist();
        /// <summary>
        /// Find the repository for handling configuration.
        /// </summary>
        public abstract AbstractConfigurationRepository ConfigurationRepository { get; }
        public abstract AbstractGameRepository GameRepository { get; }
        public abstract AbstractFactionRepository FactionRepository { get; }
        public abstract AbstractCardtypeRepository CardtypeRepository { get; }
    }
}
