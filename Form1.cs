using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;
using System.Diagnostics;
namespace AutoAGR
{
    public partial class Form1 : Form
    {
        
        decimal framerate = 60;
        bool playerCamera = false;
        bool otherPlayers = false;
        bool weapons = false;
        bool projectiles = false;
        bool fpm = false;
        bool exitgame = false;
        string demPath, agrPath, tfPath;
        decimal startingTick = 1;
        bool errors = false;
        string hlaePath;


        //  CONTROL BUTTONS
        private void Pbox_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Pbox_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //  FORM EVENTS
        public Form1()
        {
            InitializeComponent();
            initCustomFont();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  INIT VALUES
            framerate = 60;
            playerCamera = false;
            otherPlayers = false;
            weapons = false;
            projectiles = false;
            fpm = false;
            exitgame = false;
            demPath = "";
            agrPath = "";
            tfPath = "";
            startingTick = 1;
            errors = false;

            //  IF CONF FILE EXISTS READ PREVIOUS CONFIGURATION
            if (File.Exists("configuration.txt"))
            {
                FileStream f = File.Open("configuration.txt", FileMode.Open);
                StreamReader r = new StreamReader(f);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    var parts = line.Split(new char[] { ' ' },2);
                    switch (parts[0])
                    {
                        case "F":
                            framerate = Decimal.Parse(parts[1]);
                            nud_framerate.Value = framerate;
                            break;

                        case "PC":
                            playerCamera = Convert.ToInt32(parts[1]) != 0;
                            rbtn_PC_Y.Checked = playerCamera;
                            rbtn_PC_N.Checked = !playerCamera;
                            break;

                        case "OP":
                            otherPlayers = Convert.ToInt32(parts[1]) != 0;
                            rbtn_OP_Y.Checked = otherPlayers;
                            rbtn_OP_N.Checked = !otherPlayers;
                            break;

                        case "W":
                            weapons = Convert.ToInt32(parts[1]) != 0;
                            rbtn_W_Y.Checked = weapons;
                            rbtn_W_N.Checked = !weapons;
                            break;

                        case "P":
                            projectiles = Convert.ToInt32(parts[1]) != 0;
                            rbtn_P_Y.Checked = projectiles;
                            rbtn_P_N.Checked = !projectiles;
                            break;

                        case "FPM":
                            fpm = Convert.ToInt32(parts[1]) != 0;
                            rbtn_RFPM_Y.Checked = fpm;
                            rbtn_RFPM_N.Checked = !fpm;
                            break;

                        case "E":
                            exitgame = Convert.ToInt32(parts[1]) != 0;
                            rbtn_E_Y.Checked = exitgame;
                            rbtn_E_N.Checked = !exitgame;
                            break;

                        case "PDEM":
                            demPath = parts[1];
                            txtDem.Text = demPath;
                            break;

                        case "PAGR":
                            agrPath = parts[1];
                            txtAgr.Text = agrPath;
                            break;

                        case "PTF":
                            tfPath = parts[1];
                            txtCfg.Text = tfPath;
                            break;

                        case "PHLAE":
                            hlaePath = parts[1];
                            txtHLAE.Text = hlaePath;
                            break;

                        case "ST":
                            startingTick = Decimal.Parse(parts[1]);
                            nud_startingTick.Value = startingTick;
                            break;

                    }
                }
                r.Close();
                f.Close();
            }
        }

        //  FORM CONTROLS

        private void Nud_framerate_ValueChanged(object sender, EventArgs e)
        {
            framerate = nud_framerate.Value;
        }

        private void Nud_startingTick_ValueChanged(object sender, EventArgs e)
        {
            startingTick = nud_startingTick.Value;
        }

        private void Rbtn_PC_Y_CheckedChanged(object sender, EventArgs e)
        {
            playerCamera = rbtn_PC_Y.Checked;
        }

        private void Rbtn_OP_Y_CheckedChanged(object sender, EventArgs e)
        {
            otherPlayers = rbtn_OP_Y.Checked;
        }

        private void Rbtn_W_Y_CheckedChanged(object sender, EventArgs e)
        {
            weapons = rbtn_W_Y.Checked;
        }

        private void Rbtn_P_Y_CheckedChanged(object sender, EventArgs e)
        {
            projectiles = rbtn_P_Y.Checked;
        }

        private void Rbtn_RFPM_Y_CheckedChanged(object sender, EventArgs e)
        {
            fpm = rbtn_RFPM_Y.Checked;
        }

        private void Rbtn_E_Y_CheckedChanged(object sender, EventArgs e)
        {
            exitgame = rbtn_E_Y.Checked;
        }

        private void TxtDem_TextChanged(object sender, EventArgs e)
        {
            demPath = txtDem.Text;
        }

        private void TxtAgr_TextChanged(object sender, EventArgs e)
        {
            agrPath = txtAgr.Text;
        }

        private void TxtCfg_TextChanged(object sender, EventArgs e)
        {
            tfPath = txtCfg.Text;
        }

        private void Pbox_Dem_Click(object sender, EventArgs e)
        {
            dialog_dem.Reset();
            dialog_dem.DefaultExt = ".dem";
            dialog_dem.Filter = "DEM Files|*.dem";
            dialog_dem.Multiselect = false;
            dialog_dem.CheckFileExists = true;
            dialog_dem.CheckPathExists = true;
            dialog_dem.ShowDialog();
            demPath = dialog_dem.FileName;
            txtDem.Text = demPath;
        }

        private void Pbox_Agr_Click(object sender, EventArgs e)
        {
            dialog_agr.Reset();
            dialog_agr.DefaultExt = ".agr";
            dialog_agr.Filter = "AGR Files|*.agr";
            dialog_agr.AddExtension = true;
            dialog_agr.ShowDialog();
            agrPath = dialog_agr.FileName;
            txtAgr.Text = agrPath;
        }

        private void Pbox_Tf_Click(object sender, EventArgs e)
        {
            dialog_tf.Reset();
            dialog_tf.ShowDialog();
            tfPath = dialog_tf.SelectedPath;
            txtCfg.Text = tfPath;
        }

        private void Pbox_Hlae_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog_hlae = new FolderBrowserDialog();
            dialog_hlae.ShowDialog();
            hlaePath = dialog_hlae.SelectedPath;
            txtHLAE.Text = hlaePath;
        }

        private void TxtHLAE_TextChanged(object sender, EventArgs e)
        {
            hlaePath = txtHLAE.Text;
        }

        private void BtnHLAE_Click(object sender, EventArgs e)
        {
            if (hlaePath == "")
            {
                MessageBox.Show("Path for HLAE folder has not been specified.");
                return;
            }
            try
            {
                if (Directory.Exists(hlaePath))
                {
                    if (File.Exists(hlaePath + "\\hlae.exe") && File.Exists(hlaePath + "\\AfxHookSource.dll"))
                    {
                        ProcessStartInfo si = new ProcessStartInfo();
                        si.FileName = hlaePath + "\\hlae.exe";
                        string programPath = tfPath + "\\..\\hl2.exe";
                        si.Arguments = "-customLoader -noGui -autoStart -hookDllPath \"" + hlaePath + "\\AfxHookSource.dll\" -programPath \"" + programPath + "\" -cmdLine \"-novid -fullscreen -width 64 -height 40 -steam -insecure +sv_lan 1 -console -game  tf +exec AutoAGR\"" ;
                        Process.Start(si);
                    }
                    else
                    {
                        MessageBox.Show("Some files are missing!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid path to HLAE folder.");
                }
            }
            catch (System.NotSupportedException)
            {
                MessageBox.Show("Invalid path to HLAE folder.");
            }

        }

        private void Btn_save_Click(object sender, EventArgs e)
        {

            if (demPath == "")
            {
                MessageBox.Show("Path for DEM file has not been specified.");
                return;
            }
            try
            {
                FileInfo fi = new FileInfo(demPath);
                if (!fi.Exists || fi.Extension != ".dem")
                {
                    MessageBox.Show("Invalid path for DEM file.");
                    return;
                }
            }
            catch (System.NotSupportedException)
            {
                MessageBox.Show("Invalid File Path for DEM file");
                return;
            }

            if (agrPath == "")
            {
                MessageBox.Show("Path for AGR file has not been specified.");
                return;
            }
            try
            {
                FileInfo fi = new FileInfo(agrPath);
                if (fi.Exists)
                {
                    MessageBox.Show("AGR file already exists.");
                    return;
                }
                else if (fi.Extension != ".agr")
                {
                    MessageBox.Show("Invalid path for AGR file.");
                    return;
                }

            }
            catch (System.NotSupportedException)
            {
                MessageBox.Show("Invalid File Path for AGR file");
                return;
            }

            if (tfPath == "")
            {
                MessageBox.Show("Path for CFG folder was not specified.");
                return;
            }
            try
            {
                if (Directory.Exists(tfPath))
                {
                    if (!Directory.Exists(tfPath + "\\cfg"))
                    {
                        Directory.CreateDirectory(tfPath + "\\cfg");
                    }

                    writeConfFile();

                    string filePath = tfPath + "\\cfg\\AutoAGR.cfg";
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                    string contents = "";
                    contents = getContents();
                    File.WriteAllText(filePath, contents);
                }
                else
                {
                    errors = true;
                }
            }
            catch (System.NotSupportedException)
            {
                MessageBox.Show("Invalid path to folder.");
                return;
            }
            if (!errors)
            {
                MessageBox.Show("File created successfully!");
            }
            else
            {
                MessageBox.Show("Failed to create file.");
            }
        }

        private void BtnWindowed_Click(object sender, EventArgs e)
        {
            if (hlaePath == "")
            {
                MessageBox.Show("Path for HLAE folder has not been specified.");
                return;
            }
            try
            {
                if (Directory.Exists(hlaePath))
                {
                    if (File.Exists(hlaePath + "\\hlae.exe") && File.Exists(hlaePath + "\\AfxHookSource.dll"))
                    {
                        ProcessStartInfo si = new ProcessStartInfo();
                        si.FileName = hlaePath + "\\hlae.exe";
                        string programPath = tfPath + "\\..\\hl2.exe";
                        si.Arguments = "-customLoader -noGui -autoStart -hookDllPath \"" + hlaePath + "\\AfxHookSource.dll\" -programPath \"" + programPath + "\" -cmdLine \"-novid -window -width 640 -height 480 -steam -insecure +sv_lan 1 -console -game  tf +exec AutoAGR\"";
                        Process.Start(si);
                    }
                    else
                    {
                        MessageBox.Show("Some files are missing!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid path to HLAE folder.");
                }
            }
            catch (System.NotSupportedException)
            {
                MessageBox.Show("Invalid path to HLAE folder.");
            }
        }

        //  MAKING FORM DRAGGABLE
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }


        //  PRIVATE FUNCTIONS

        private void initCustomFont()
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("TF2secondary.ttf");
            var panels = this.panel1.Controls.OfType<Panel>().ToList();
            foreach (var panel in panels)
            {
                var labels = panel.Controls.OfType<Label>().ToList();
                var radioBtns = panel.Controls.OfType<RadioButton>().ToList();

                foreach (var label in labels)
                {
                    label.Font = new Font(pfc.Families[0], label.Font.Size); ;
                }
                foreach (var radioBtn in radioBtns)
                {
                    radioBtn.Font = new Font(pfc.Families[0], radioBtn.Font.Size);
                }
            }
            nud_framerate.Font = new Font(pfc.Families[0], nud_framerate.Font.Size);
            nud_startingTick.Font = new Font(pfc.Families[0], nud_startingTick.Font.Size);
            btn_save.Font = new Font(pfc.Families[0], btn_save.Font.Size);
            txtDem.Font = new Font(pfc.Families[0], txtDem.Font.Size);
            txtAgr.Font = new Font(pfc.Families[0], txtAgr.Font.Size);
            txtCfg.Font = new Font(pfc.Families[0], txtCfg.Font.Size);
            txtHLAE.Font = new Font(pfc.Families[0], txtCfg.Font.Size);
        }

        private string getContents()
        {
            string str = "";
            str += "host_timescale 0\n";
            str += "host_framerate " + framerate + "\n";
            str += "mirv_agr enabled 1\n";
            str += "mirv_agr recordCamera " + Convert.ToInt32(playerCamera) + "\n";
            str += "mirv_agr recordPlayers " + Convert.ToInt32(otherPlayers) + "\n";
            str += "mirv_agr recordWeapons " + Convert.ToInt32(weapons) + "\n";
            str += "mirv_agr recordProjectiles " + Convert.ToInt32(projectiles) + "\n";
            str += "mirv_agr recordViewmodel " + Convert.ToInt32(fpm) + "\n";
            str += "demo_quitafterplayback " + Convert.ToInt32(exitgame) + "\n";
            str += "playdemo \"" + demPath + "\"\n";
            str += "demo_pause\n";
            str += "demo_gototick " + startingTick + "\n";
            str += "mirv_agr start \"" + agrPath + "\"\n";
            str += "demo_resume\n";
            return str;

        }

        private void writeConfFile()
        {
            FileStream f = File.Create("configuration.txt");
            StreamWriter s = new StreamWriter(f);
            s.WriteLine("F " + framerate);
            s.WriteLine("PC " + Convert.ToInt32(playerCamera));
            s.WriteLine("OP " + Convert.ToInt32(otherPlayers));
            s.WriteLine("W " + Convert.ToInt32(weapons));
            s.WriteLine("P " + Convert.ToInt32(projectiles));
            s.WriteLine("FPM " + Convert.ToInt32(fpm));
            s.WriteLine("E " + Convert.ToInt32(exitgame));
            s.WriteLine("PDEM " + demPath);
            s.WriteLine("PAGR " + agrPath);
            s.WriteLine("PTF " + tfPath);
            s.WriteLine("PHLAE " + hlaePath);
            s.WriteLine("ST " + startingTick);
            s.Close();
            f.Close();
        }


    }
}
