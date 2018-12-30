using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UmdParser;

namespace UMDFileConvertToTxtWindowsFormsApp
{
    public partial class UmdConvertForm : Form
    {
        private IUmdParser _umdParser;
        public UmdConvertForm()
        {
            InitializeComponent();
            _umdParser = new DefaultUmdParser();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var stream = File.OpenRead("a.umd"))
            {
                var dic = _umdParser.Parse(stream);
            }
        }
    }
}
