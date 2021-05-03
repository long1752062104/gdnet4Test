using Net.Client;
using Net.Event;
using Net.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            BufferPool.Size = 1024 * 1024;
            NDebug.BindLogAll(Console.WriteLine);
        }

        CancellationTokenSource t;

        bool run;

        private void button1_Click(object sender, EventArgs e)
        {
            run = !run;
            if (!run)
            {
                t?.Cancel();
                t?.Dispose();
                button1.Text = "启动";
            }
            else 
            {
                dataGridView1.Rows.Clear();
                var ip = textBox2.Text;
                var port = int.Parse(textBox3.Text);
                var cliNum = int.Parse(textBox4.Text);
                var dataLen = int.Parse(textBox1.Text);
                for (int i = 0; i < cliNum; i++) 
                {
                    dataGridView1.Rows.Add();
                }
                t = UdpClient.Testing(ip, port, cliNum, dataLen, (clis) => {
                    for (int i = 0; i < clis.Count; i++)
                    {
                        dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                        dataGridView1.Rows[i].Cells[0].Value = clis[i].Identify;
                        dataGridView1.Rows[i].Cells[1].Value = clis[i].sendSize;
                        dataGridView1.Rows[i].Cells[2].Value = clis[i].revdSize;
                        dataGridView1.Rows[i].Cells[3].Value = clis[i].sendNum;
                        dataGridView1.Rows[i].Cells[4].Value = clis[i].revdNum;
                        dataGridView1.Rows[i].Cells[5].Value = clis[i].resolveNum;
                        dataGridView1.Rows[i].Cells[6].Value = clis[i].fps;
                    }
                });
                button1.Text = "关闭";
            }
        }
    }
}
