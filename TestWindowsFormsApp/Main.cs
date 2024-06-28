using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sang.Baidu.TranslateAPI;

namespace TestWindowsFormsApp
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void btnTr_Click(object sender, EventArgs e)
        {
            string appid = txtId.Text;
            string appkey = txtKey.Text;
            string text = textBox.Text;

            if (string.IsNullOrEmpty(appid) || string.IsNullOrEmpty(appkey) || string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            var translate = new BaiduTranslator(appid, appkey);
            translate.Translate(text,"zh").ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    MessageBox.Show(t.Exception.Message);
                    return;
                }

                BaiduTranslateResult result = t.Result;
                if (result.Success)
                {
                    this.Invoke(new Action(() =>
                    {
                        textBox.Text = result.Trans_Result[0].Dst;
                    }));
                }
            });
            
        }
    }
}
