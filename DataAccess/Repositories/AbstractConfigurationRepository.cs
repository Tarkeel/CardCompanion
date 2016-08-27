using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repositories
{
    public abstract class AbstractConfigurationRepository
    {
        public abstract string GetValue(string Setting);
        public abstract void SetValue(string Setting, string Value, bool persist = true);
        public abstract void ClearSetting(string Setting, bool persist = true);
   }
}