using System;
using PluginContracts;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using OutputHelperLib;
using System.Linq;

namespace InputFileCorpus
{
    public class InputFilesCorpus : InputPlugin
    {

        public string[] InputType { get; } = { "Corpus File" };
        public string OutputType { get; } = "String";
        public StreamReader InputStream { get; set; }
        //public object Output { get; set; }
        public bool KeepStreamOpen { get; } = true;
        public string IncomingTextLocation { get; set; }
        public string SelectedEncoding { get; set; } = "utf-8";
        ulong RowCounter { get; set; } = 0;
        public bool InheritHeader { get; } = false;
        public Dictionary<int, string> OutputHeaderData { get; set;  } = new Dictionary<int, string>(){
                                                                                            {0, "Text"}
                                                                                        };
        public int TextCount { get; set; }
        private int TextCountPadding { get; set; } = 10;
        public string[] header { get; set; }
        private int CurrentRow { get; set; }


        #region Plugin Details and Info

        public string PluginName { get; } = "Read Text from Corpus File";
        public string PluginType { get; } = "Load File(s)";
        public string PluginVersion { get; } = "1.0.1";
        public string PluginAuthor { get; } = "Ryan L. Boyd (ryan@ryanboyd.io)";
        public string PluginDescription { get; } = "This plugin will read text(s) from a BUTTER corpus file (.txt). Use the Plugin Settings to select the corpus file that contains your texts. This plugin should always be at the top level of your Analysis Pipeline. For example:" + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "\tRead Text from Corpus File" + Environment.NewLine +
            "\t |" + Environment.NewLine +
            "\t |-- Tokenize Texts" + Environment.NewLine +
            "\t |" + Environment.NewLine +
            "\t |-- etc.";
        public bool TopLevel { get; } = true;
        public string PluginTutorial { get; } = "https://youtu.be/acjtMw0r1yw";

        public Icon GetPluginIcon
        {
            get
            {
                return Properties.Resources.icon;
            }
        }

        #endregion


        #region Settings and ChangeSettings() Method
        private bool ScanSubfolders { get; } = false;
               
        public void ChangeSettings()
        {
            using (var form = new SettingsForm_InputFileCorpus(IncomingTextLocation, SelectedEncoding))
            {


                form.Icon = Properties.Resources.icon;
                form.Text = PluginName;


                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedEncoding = form.SelectedEncoding;
                    IncomingTextLocation = form.TextFileLocation;
                }
            }

        }
        #endregion


        public Payload RunPlugin(Payload Incoming)
        {

            Payload pData = new Payload();
            pData.FileID = Incoming.FileID;
            pData.SegmentID = Incoming.SegmentID;
                        
            try
            {

                RowObject row = (RowObject)Incoming.ObjectList[0];
                string lineText = row.Text;

                pData.StringList.Add(lineText);
                pData.FileID = row.RowID; //lineText.Substring(0, Math.Min(lineText.Length, 50));
                pData.SegmentNumber.Add(1);
                return (pData);
            }
            catch
            {
                return (new Payload());
            }
        }





        public IEnumerable TextEnumeration()
        {
            RowCounter = 1;

            try
            {
                var lines = ReadLines(IncomingTextLocation, Encoding.GetEncoding(SelectedEncoding));
                return lines;

            }
            catch
            {
                MessageBox.Show("There was an error while trying to read your corpus file. It is possible that your corpus is not correctly formatted.", "Error reading corpus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                IEnumerable<IList<string[]>> nada = Enumerable.Empty<IList<string[]>>();
                return (nada);
            }


        }


        //for input streams, we use the Initialize() method to tally up the number of items to be analyzed
        public void Initialize()
        {

            RowCounter = 0;
            TextCount = 0;

            var lines = ReadLines(IncomingTextLocation, Encoding.GetEncoding(SelectedEncoding));

            foreach (var line in lines)
            {
                try
                {
                    TextCount++;
                }
                catch
                {
                        
                }


            }

            TextCountPadding = TextCount.ToString().Length;

        }




        //https://stackoverflow.com/a/13312954
        private IEnumerable<RowObject> ReadLines(string InputFile,
                                     Encoding encoding)
        {
            using (var stream = File.OpenRead(InputFile))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return new RowObject(RowCounter++, line, TextCountPadding);
                }
            }
        }




        public class RowObject
        {
            public string RowID { get; set; }
            public string Text { get; set; }

            public RowObject(ulong rowNum, string rowText, int paddingValue)
            {

                RowID = "Row" + rowNum.ToString().PadLeft(paddingValue, '0');
                Text = rowText;
            }

        }





        public bool InspectSettings()
        {
            if (string.IsNullOrEmpty(IncomingTextLocation))
            {
                return false;
            }
            else if (!File.Exists(IncomingTextLocation))
            {
                MessageBox.Show("Your selected input file does not appear to exist anymore. Has it been deleted/moved?", "Cannot Find Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }
            
        }



        public Payload FinishUp(Payload Input)
        {
            return (Input);
        }



        #region Import/Export Settings
        public void ImportSettings(Dictionary<string, string> SettingsDict)
        {
            SelectedEncoding = SettingsDict["SelectedEncoding"];
            IncomingTextLocation = SettingsDict["IncomingTextLocation"];
        }

        public Dictionary<string, string> ExportSettings(bool suppressWarnings)
        {
            Dictionary<string, string> SettingsDict = new Dictionary<string, string>();
            SettingsDict.Add("SelectedEncoding", SelectedEncoding);
            SettingsDict.Add("IncomingTextLocation", IncomingTextLocation);
            return (SettingsDict);
        }
        #endregion

    }
}
