using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.OleDb;

using DataAccess.Repositories;
using DataAccess.Types;

namespace DataAccess.BulkLoaders
{
    public class XLSLoader
    {

        string filename;
        AbstractRepositoryFactory repositories;
        //The backgroundworker running the process
        System.ComponentModel.BackgroundWorker worker;
        System.ComponentModel.DoWorkEventArgs eventArgs;
        public XLSLoader(string _filename,
            AbstractRepositoryFactory _repositories,
            System.ComponentModel.BackgroundWorker _worker,
            System.ComponentModel.DoWorkEventArgs _eventArgs)
        {
            repositories = _repositories;
            filename = _filename;
            worker = _worker;
            eventArgs = _eventArgs;


        }
        public void Load()
        {
            DataSet data = parseFile();
            importGames(data.Tables["Games"]);
            repositories.Persist();
        }
        private DataSet parseFile()
        {
            DataSet result = new DataSet();
            //Open the file
            string provider = "Microsoft.Jet.OLEDB.4.0";
            string properties = "'Excel 8.0;HDR=Yes;'";
            string connectionString = string.Format(@"Provider={0};Data Source={1};Extended Properties={2}",
                provider,
                filename,
                properties);
            OleDbConnection connection = new OleDbConnection(connectionString);
            try
            {
                connection.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = connection;
                //Parse file and build resultset

                //Games:
                cmd.CommandText = string.Format("SELECT * FROM [{0}]", "Game$");
                DataTable table = new DataTable("Games");
                new OleDbDataAdapter(cmd).Fill(table);
                result.Tables.Add(table);

                //Close out
                cmd = null;
                connection.Close();
                return result;
            }
            catch (OleDbException e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
        private void importGames(DataTable source)
        {

            int colTitle = -1;
            int colYear = -1;
            int colUrl = -1;
            int colPublisher = -1;
            int colBasePath = -1;
            int colLogoPath = -1;
            int colBannerPath = -1;
            #region Map column headers
            //Determine what the column maps to
            int counter = 0;
            foreach (DataColumn col in source.Columns)
            {
                String column = col.ColumnName.ToLower();
                switch (column)
                {
                    case "game_name":
                    case "game":
                    case "name":
                    case "title":
                        colTitle = counter;
                        break;
                    case "year":
                        colYear = counter;
                        break;
                    case "url":
                        colUrl = counter;
                        break;
                    case "publisher":
                        colPublisher = counter;
                        break;
                    case "base":
                    case "path":
                    case "basepath":
                        colBasePath = counter;
                        break;
                    case "logo":
                    case "logopath":
                        colLogoPath = counter;
                        break;
                    case "banner":
                    case "bannerpath":
                        colBannerPath = counter;
                        break;
                    default:
                        //TODO: Move this to logging
                        Console.WriteLine("Unhandled column: {0}", col.ColumnName);
                        break;
                }
                counter++;
            }
            #endregion
            #region Process and import data
            int totalRows = source.Rows.Count;
            int rowCount = 0;
            if (colTitle != -1)
            {
                foreach (DataRow row in source.Rows)
                {
                    if (worker.CancellationPending)
                    {
                        eventArgs.Cancel = true;
                        break;
                    }
                    Game game = null;
                    bool updated = false;
                    //Show
                    if (colTitle != -1
                        && !DBNull.Value.Equals(row[colTitle]))
                    {
                        game = repositories.GameRepository.CreateOrGetGame(Convert.ToString(row[colTitle]), false);
                    }
                    //Year
                    if (game != null && colYear != -1 && !DBNull.Value.Equals(row[colYear]))
                    {
                        long _year = Convert.ToInt32(row[colYear]);
                        if (game.Year <= 0)
                        {
                            //Only overwrite blank values
                            game.Year = _year;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.Year.Equals(_year)) { Console.WriteLine("{0}: Year {1} blocked by existing value {2};", game.Title, _year, game.Year); }
                        }
                    }
                    //Url
                    if (game != null && colUrl != -1 && !DBNull.Value.Equals(row[colUrl]))
                    {
                        string _url = Convert.ToString(row[colUrl]);
                        if (game.Url == null || game.Url.Equals(""))
                        {
                            //Only overwrite blank values
                            game.Url = _url;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.Url.Equals(_url)) { Console.WriteLine("{0}: URL {1} blocked by existing value {2};", game.Title, _url, game.Url); }
                        }
                    }
                    //Publisher
                    if (game != null && colPublisher != -1 && !DBNull.Value.Equals(row[colPublisher]))
                    {
                        string _publisher = Convert.ToString(row[colPublisher]);
                        if (game.Publisher == null || game.Publisher.Equals(""))
                        {
                            //Only overwrite blank values
                            game.Publisher = _publisher;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.Publisher.Equals(_publisher)) { Console.WriteLine("{0}: Publisher {1} blocked by existing value {2};", game.Title, _publisher, game.Publisher); }
                        }
                    }
                    //BasePath
                    if (game != null && colBasePath != -1 && !DBNull.Value.Equals(row[colBasePath]))
                    {
                        string _path = Convert.ToString(row[colBasePath]);
                        if (game.BasePath == null || game.BasePath.Equals(""))
                        {
                            //Only overwrite blank values
                            game.BasePath = _path;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.BasePath.Equals(_path)) { Console.WriteLine("{0}: BasePath {1} blocked by existing value {2};", game.Title, _path, game.BasePath); }
                        }
                    }
                    //LogoPath
                    if (game != null && colLogoPath != -1 && !DBNull.Value.Equals(row[colLogoPath]))
                    {
                        string _logo = Convert.ToString(row[colLogoPath]);
                        if (game.LogoPath == null || game.LogoPath.Equals(""))
                        {
                            //Only overwrite blank values
                            game.LogoPath = _logo;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.LogoPath.Equals(_logo)) { Console.WriteLine("{0}: LogoPath {1} blocked by existing value {2};", game.Title, _logo, game.LogoPath); }
                        }
                    }
                    //BannerPath
                    if (game != null && colBannerPath != -1 && !DBNull.Value.Equals(row[colBannerPath]))
                    {
                        string _banner = Convert.ToString(row[colBannerPath]);
                        if (game.BannerPath == null || game.BannerPath.Equals(""))
                        {
                            //Only overwrite blank values
                            game.BannerPath = _banner;
                            updated = true;
                        }
                        else
                        {
                            //TODO: Update this for user confirmation
                            if (!game.BannerPath.Equals(_banner)) { Console.WriteLine("{0}: BannerPath {1} blocked by existing value {2};", game.Title, _banner, game.BannerPath); }
                        }
                    }
                    //Save if needed, delayed persist
                    if (updated)
                    {
                        repositories.GameRepository.UpdateGame(game, false);
                    }
                    //Report progress
                    rowCount++;
                    if (worker.WorkerReportsProgress) { worker.ReportProgress(rowCount * 100 / totalRows, String.Format("{0} of {1} rows imported.", rowCount, totalRows)); }
                }
            }
            #endregion
        }
    }
}
