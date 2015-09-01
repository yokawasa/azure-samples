using System;
using System.IO;
using System.Linq;
using System.Configuration;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace ams_list_assets
{
    class Program
    {
        private static readonly string _accountKey = ConfigurationManager.AppSettings["accountKey"];
        private static readonly string _accountName = ConfigurationManager.AppSettings["accountName"];

        static void help()
        {
            string helpstr = @"Usage:
ams-list-assets [OutputFile(csv)]

";
            Console.WriteLine(helpstr);
        }

        static int doListAllAssets(string outputFile)
        {
            MediaServicesCredentials credentials = new MediaServicesCredentials(
                                        _accountName, _accountKey);
            CloudMediaContext _context = new CloudMediaContext(credentials);

            try
            {
                int selectlimit = 1000;
                int offset = 0;
                int selectposition = 0;

                int assetcount = _context.Assets.Count();
                Console.WriteLine("Assets Total Count:" + assetcount.ToString());

                StreamWriter writer = new StreamWriter(outputFile);

                string FieldDefines = "\"AssetName=\",\"AssetID\",\"LastModified\",\"StroageAccountName\",ContentKeyId\",\"ContentKeyType\"";
                writer.WriteLine(FieldDefines);

                while (true)
                {
                    foreach (IAsset asset in _context.Assets.Skip(offset).Take(selectlimit))
                    {
                        selectposition++;
                        string line = "";
                        var contentkeys = asset.ContentKeys;
                        if (contentkeys.Count() > 0)
                        {
                            foreach (IContentKey contentkey in contentkeys)
                            {
                                line = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                        asset.Name,
                                        asset.Id,
                                        asset.StorageAccountName,
                                        contentkey.Id,
                                        contentkey.ContentKeyType
                                        );
                                break;
                            }
                        }
                        else
                        {
                            line = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                    asset.Name,
                                    asset.Id,
                                    asset.StorageAccountName,
                                    "", ""
                                    );
                        }
                        writer.WriteLine(line);
                    }
                    if (selectposition == selectlimit)
                    {
                        offset += selectlimit;
                        selectposition = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }


        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                help();
                return -1;
            }
            string outputFile = args[0];

            string waitMessage = "This may take a few seconds to a few minutes depending on how many assets you have."
                + Environment.NewLine + Environment.NewLine
                + "Please wait..."
                + Environment.NewLine;
            Console.Write(waitMessage);

            return doListAllAssets(outputFile);
        }
    }
}