using Microsoft.Data.SqlClient;
using System.Configuration;

namespace ScheduledTask
{
    public partial class Form1 : Form
    {
        //  string conStr = "Data Source=mxchlac-sql04;Initial Catalog=PBS;User ID=sqlIT;Password=";
        string conStr = "Server=mxchlac-sql04;Database=LAIMS;User Id=sqlIIT;Password=Le@r2025!iitdata;MultipleActiveResultSets=True;TrustServerCertificate=True;";
        int fMin = 5 * 60; // 5 MIN
        int curr = 0;
        bool running = false;
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            fMin = Int32.Parse(numericUpDown1.Value.ToString()) * 60;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            running = !running;
            if (running)
            {
                curr = fMin;
                button1.BackColor = Color.Red;
                button1.Text = "End";
                toolStripStatusLabel1.Text = "Running";
            }
            else
            {
                button1.BackColor = Color.Lime;
                button1.Text = "Start";
                toolStripStatusLabel1.Text = "Stoped";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (running)
            {
                toolStripStatusLabel1.Text = fMin - curr + " seconds until next execution";
                if (curr >= fMin)
                {
                    curr = 0;
                    callProcedure();
                }
            }
            curr++;
        }
        private void callProcedure()
        {
            SqlConnection connection = new SqlConnection(conStr);
            DateTime D = DateTime.Now;
            try
            {
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    textBox1.Invoke(() => { textBox1.AppendText("[" + D.ToShortTimeString() + "] " + "Ejecutando PBS.dbo.ActualizarTablaTrabajosDetalle" + Environment.NewLine); });
                    SqlCommand cmd = new SqlCommand("EXEC PBS.dbo.ActualizarTablaTrabajosDetalle" + Environment.NewLine, connection);
                    cmd.ExecuteNonQuery();
                    textBox1.Invoke(() => { textBox1.AppendText("[" + D.ToShortTimeString() + "] " + "Fin de ejecucion" + Environment.NewLine); });
                }
            }
            catch (Exception ex)
            {
                textBox1.Invoke(() => { textBox1.AppendText("[" + D.ToShortTimeString() + "] " + "Error:" + ex.Message + Environment.NewLine); });
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            fMin = Int32.Parse(numericUpDown1.Value.ToString()) * 60;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fMin = Properties.Settings.Default.Delay;

            try
            {
                numericUpDown1.Value = fMin;

            }
            catch (Exception ex)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Delay = fMin;
           Properties.Settings.Default.Save();
        }
    }
}
