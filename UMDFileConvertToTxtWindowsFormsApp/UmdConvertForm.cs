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
        OpenFileDialog _openFileDialog = new OpenFileDialog();
        SaveFileDialog _saveFileDialog = new SaveFileDialog();
        UmdFile _umdFile = null;
        public UmdConvertForm()
        {
            InitializeComponent();
            _umdParser = new DefaultUmdParser();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var fileName = _openFileDialog.FileName;
            using (var stream = File.OpenRead(fileName))
            {
                _umdFile = _umdParser.Parse(stream);
                //封面
                if (_umdFile.Cover != null)
                    picCover.Image = Image.FromStream(new MemoryStream(_umdFile.Cover.CoverBuffer));
                //文件属性
                txtFileProperties.Clear();
                foreach (var item in _umdFile.GetFileMetaData())
                {
                    txtFileProperties.AppendText($"{item.ToString()}\r\n");
                }
                //目录
                lbMenu.DataSource = _umdFile.ChapterTitle.ChapterTitle;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _openFileDialog.Dispose();
            base.OnClosed(e);
        }

        private void lbMenu_SelectedValueChanged(object sender, EventArgs e)
        {
            var index = lbMenu.SelectedIndex;
            richTxtContent.Clear();
            richTxtContent.Text = _umdFile.GetChapterContent(index);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _saveFileDialog.Filter = "文本文件(*.txt)|*.txt";
            _saveFileDialog.FileName = _umdFile.Title.Title + ".txt"; ;
            if (_umdFile == null)
            {
                MessageBox.Show("请先打开需要转换的umd文件");
                return;
            }
            if (_saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var fileName = _saveFileDialog.FileName;
            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                //fs.Write(new byte[] { 0xff, 0xfe }, 0, 2);
                //foreach (var item in _umdFile.Content.ContentBuffer)
                //{
                //    fs.Write(item, 0, item.Length);
                //}

                foreach (var item in _umdFile.Content.ContentBuffer)
                {
                    sw.Write(Encoding.Unicode.GetString(item).Replace("\u2029", "\n"));
                }
            }
            MessageBox.Show("save success");
        }
    }
}
