using System.Text;
using System.Windows.Forms;
using System.IO;






namespace InputFileCorpus
{
    internal partial class SettingsForm_InputFileCorpus : Form
    {


        #region Get and Set Options

        public string TextFileLocation { get; set; }
        public string SelectedEncoding { get; set; }
       #endregion



        public SettingsForm_InputFileCorpus(string CorpusFileLocation, string SelectedEncoding)
        {
            InitializeComponent();

            foreach (var encoding in Encoding.GetEncodings())
            {
                EncodingDropdown.Items.Add(encoding.Name);
            }

            try
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(SelectedEncoding);
            }
            catch
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(Encoding.Default.BodyName);
            }

            SelectedFileTextbox.Text = CorpusFileLocation;
            this.SelectedEncoding = SelectedEncoding;

        }












        private void SetFolderButton_Click(object sender, System.EventArgs e)
        {

  
            SelectedFileTextbox.Text = "";
  

            using (var dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Title = "Please choose the corpus file that you would like to read";
                dialog.FileName = "BUTTER Corpus.txt";
                dialog.Filter = "BUTTER Corpus File (*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedFileTextbox.Text = dialog.FileName;


                    try
                    {
                        using (var stream = File.OpenRead(dialog.FileName))
                        using (var reader = new StreamReader(stream, encoding: Encoding.GetEncoding(SelectedEncoding)))
                        {
                            
                        }

                    }
                    catch
                    {
                        MessageBox.Show("There was an error while trying to read your corpus file. It is possible that your file is not correctly formatted.", "Error reading corpus file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                }
            }
        }
















        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.SelectedEncoding = EncodingDropdown.SelectedItem.ToString();
            this.TextFileLocation = SelectedFileTextbox.Text;
            
            this.DialogResult = DialogResult.OK;

        }
    }
}
