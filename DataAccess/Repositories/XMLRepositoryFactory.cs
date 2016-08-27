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
                        new XElement("Config")
                        //TODO: Add new elements for other top nodes
                    )
                );
                Save();
            }
            //Initialize Configuration
            configurationRepository = new XMLConfigurationRepository(this);
        }
        #endregion
        #region Versioning
        private Int32 currentVersion = 0;
        /* Version History:
         *
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