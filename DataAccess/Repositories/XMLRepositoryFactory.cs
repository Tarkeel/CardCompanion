using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using DataAccess.Types;

namespace DataAccess.Repositories
{
    public sealed class XMLRepositoryFactory : AbstractRepositoryFactory
    {
        #region Internal attributes
        internal XDocument Document { get; private set; }
        private string filename;
        #endregion
        #region Singleton
        //We apply the Singleton pattern to ensure that only one repository is in use.
        private static readonly Lazy<XMLRepositoryFactory> instance = new Lazy<XMLRepositoryFactory>(() =>
            new XMLRepositoryFactory(@"C:\Projects\CardRepository.xml"));
        public static AbstractRepositoryFactory Instance
        {
            get { return instance.Value; }
        }
        private XMLRepositoryFactory(string _filename)
        {
            filename = _filename;
            try
            {
                //Read in the file
                Document = XDocument.Load(filename);
                //Check the version stored, update as needed
                Int32 xmlVersion = Convert.ToInt32(Document.Root.Attribute("version").Value);
                if (xmlVersion != currentVersion)
                {
                    if (! updateVersion())
                    {
                        //TODO: Throw new invalid file exception
                    }
                }
            }
            //System.IO.DirectoryNotFoundException
            catch (System.IO.FileNotFoundException ex)
            {
                //File not found, so create it.
                Document = new XDocument(
                    new XElement("Repository",
                        new XAttribute("version", currentVersion), //Store which version of the XML datafile we're creating
                        new XElement("Config"),
                        new XElement("Games")
                    //TODO: Add new elements for other top nodes
                    )
                );
                Save();
            }
            //Initialize Configuration
            configurationRepository = new XMLConfigurationRepository(this);
            //Initialize Game repository, with next ID from config
            string nextGameID = ConfigurationRepository.GetValue("NextGameID");
            if (nextGameID == null || nextGameID.Equals(""))
            {
                ConfigurationRepository.SetValue("NextGameID", "1");
                gameRepository = new XMLGameRepository(this, 1);
            }
            else { gameRepository = new XMLGameRepository(this, Convert.ToInt32(nextGameID)); }
            //Initialize Faction repository, with next ID from config
            string nextFactionID = ConfigurationRepository.GetValue("NextFactionID");
            if (nextFactionID == null || nextFactionID.Equals(""))
            {
                ConfigurationRepository.SetValue("NextFactionID", "1");
                factionRepository = new XMLFactionRepository(this, 1);
            }
            else { factionRepository = new XMLFactionRepository(this, Convert.ToInt32(nextFactionID)); }
            //Initialize Cardtype repository, with next ID from config
            string nextCardtypeID = ConfigurationRepository.GetValue("NextCardtypeID");
            if (nextCardtypeID == null || nextCardtypeID.Equals(""))
            {
                ConfigurationRepository.SetValue("NextCardtypeID", "1");
                cardtypeRepository = new XMLCardtypeRepository(this, 1);
            }
            else { cardtypeRepository = new XMLCardtypeRepository(this, Convert.ToInt32(nextCardtypeID)); }
            //Initialize Card repository, with next ID from config
            string nextCardID = ConfigurationRepository.GetValue("NextCardID");
            if (nextCardID == null || nextCardID.Equals(""))
            {
                ConfigurationRepository.SetValue("NextCardID", "1");
                cardRepository = new XMLCardRepository(this, 1);
            }
            else { cardRepository = new XMLCardRepository(this, Convert.ToInt32(nextCardID)); }
        }
        #endregion
        #region Versioning
        private long currentVersion = 1;
        /* Version History:
         * 0: Configuration branch
         * 1: Games branch
         */
        /// <summary>
        /// Update the XML Document to the latest version.
        /// </summary>
        /// <returns></returns>
        private bool updateVersion()
        {
            Int32 oldVersion = Convert.ToInt32(Document.Root.Attribute("version").Value);
            switch (oldVersion)
            {
                case 0:
                    Document.Root.Add(new XElement("Games"));
                    Document.Root.Attribute("version").Value = Convert.ToString(currentVersion);
                    Save();
                    return true;
                default:
                    return false;
            }
        }
        #endregion
        #region Repository Delegates
        private XMLConfigurationRepository configurationRepository;
        public override AbstractConfigurationRepository ConfigurationRepository
        {
            get { return configurationRepository; }
        }
        private XMLGameRepository gameRepository;
        public override AbstractGameRepository GameRepository
        {
            get { return gameRepository; }
        }
        private XMLFactionRepository factionRepository;
        public override AbstractFactionRepository FactionRepository
        {
            get { return factionRepository; }
        }
        private XMLCardtypeRepository cardtypeRepository;
        public override AbstractCardtypeRepository CardtypeRepository
        {
            get { return cardtypeRepository; }
        }
        private XMLCardRepository cardRepository;
        public override AbstractCardRepository CardRepository
        {
            get { return cardRepository; }
        }
        #endregion
        internal void Save()
        {
            Document.Save(filename);
        }
        public override void Persist()
        {
            Save();
        }
    }
}