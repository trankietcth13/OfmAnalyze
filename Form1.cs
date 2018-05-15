using OFMProfileAnalyze.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OFMProfileAnalyze
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            classtaskinfo argument = (classtaskinfo)e.Argument;
            utilities.LogInit(null);
            try
            {
                if (argument.etask == enumtask.task_analyzeserverdata)
                {
                    string args = (string)argument.args;
                    classserverdata classserverdata = new classserverdata(Convert.FromBase64String(args), null, false);
                }
                else if (argument.etask == enumtask.task_export_ld)
                {
                    string[] strArray = (string[])argument.args;
                    foreach (string str2 in strArray)
                    {
                        this._ldtask(str2);
                    }
                    utilities.logInfo("Finished! ");
                }
                else if (argument.etask == enumtask.task_importenum)
                {
                    string dir = (string)argument.args;
                    utilities.autoimportenum(dir, true, false);
                }
                else if (argument.etask == enumtask.task_importnwscan)
                {
                }

            }
            catch (Exception exception)
            {
                utilities.logerror(argument.etask + " >> " + exception);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.richTextBox2.AppendText((string)e.UserState);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            innovaenums.WriteLogFileEnumUsed();
            utilities.Finished();
            MessageBox.Show("Ho\x00e0n Th\x00e0nh!!!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            innovaenums.init(enumManufacturer.emanufacturer_MAX);
            this.textBox1.Text = Settings.Default.rootenumfolder;
            if (utilities.autoimportenum(null, false, false) == null)
            {
                MessageBox.Show("Pls import Innova Enum Before Use!");
            }
            this.comboBox1.Items.Clear();
            foreach (InnovaServerService.classservermarket classservermarket in InnovaServerService.getlistsupportedmarket())
            {
                this.comboBox1.Items.Add(classservermarket.market);
            }
            this.comboBox1.SelectedIndex = 0;
            classlistofmprofile[] classlistofmprofileArray = InnovaServerService.getlistofmprofilemanual(null);
            if (classlistofmprofileArray != null)
            {
                foreach (classlistofmprofile classlistofmprofile in classlistofmprofileArray)
                {
                    this.listBox5.Items.Add(classlistofmprofile.ymme);
                }
            }
            VehicleProfileBackup.Init();
            this.rtxt_pathconfig.Text = Settings.Default.nwscanconfig;
            this.rtxtpathnwscandb.Text = Settings.Default.nwscanfolderdb;
            this.txtrequiredsystems.Text = Settings.Default.nwscanrequiredsystem;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = this.richTextBox1.Text;
            utilities.SaveProfile(text);
            if (!this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, text));
            }
        }

        private void _ldtask(string manufacturex)
        {
            try
            {
                string str;
                utilities.logInfo("Building " + manufacturex);
                byte[] info = Convert.FromBase64String(InnovaServerService.LDProfile_GetData(manufacturex, out str).ToString());
                if ((info != null) && (info.Length != 0))
                {
                    classserverdata classserverdata = new classserverdata(info, str, true);
                }
            }
            catch (Exception exception)
            {
                utilities.logwarning("have something wrong " + exception);
            }
        }

        private void enumroot_button2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           // InnovaServerService.UrlSet(this.r);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                InnovaServerService.setmarket((InnovaServerService.enummarket)Enum.Parse(typeof(InnovaServerService.enummarket), this.comboBox1.Text));
            }
            catch (Exception)
            {

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                localdb.classHondaSpecial.SetHondaSignalIDConfig(this.checkBox1.Checked);
                object obj2 = InnovaServerService.VehicleProfile_GetData(null, true);
                utilities.logInfo(obj2.ToString());
                if (!this.backgroundWorker1.IsBusy)
                {
                    this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, obj2.ToString()));
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("base64profile " + exception);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                string[] userargs = innovaenums.getListManufacture();
                if (!this.backgroundWorker1.IsBusy)
                {
                    this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_export_ld, userargs));
                }
            }
            catch (Exception)
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InnovaServerService.YmmeGetYears();
            this.update_itemlistymme(this.listBox1, InnovaServerService.YmmeGetYears());
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string queryVehicleProfileFromCustomList = InnovaServerService.GetQueryVehicleProfileFromCustomList(this.listBox5.SelectedItem.ToString());
                if (queryVehicleProfileFromCustomList != null)
                {
                    object obj2 = InnovaServerService.VehicleProfile_GetData(queryVehicleProfileFromCustomList, false);
                    utilities.logInfo(obj2.ToString());
                    if (!this.backgroundWorker1.IsBusy)
                    {
                        this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, obj2.ToString()));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                localdb.classHondaSpecial.SetHondaSignalIDConfig(this.checkBox1.Checked);
                object obj2 = InnovaServerService.VehicleProfile_GetData(null, false);
                utilities.logInfo(obj2.ToString());
                if (!this.backgroundWorker1.IsBusy)
                {
                    this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, obj2.ToString()));
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("base64profile " + exception);
            }
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            InnovaServerService.YmmeSetEngine(this.listBox4.SelectedItem.ToString());
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.update_itemlistymme(this.listBox4, InnovaServerService.YmmeGetEngine(this.listBox3.SelectedItem.ToString()));
        }

        private void update_itemlistymme(ListBox lstox, object obj)
        {
            try
            {
                lstox.Items.Clear();
                foreach (InnovaServerService.serverdatarestapi serverdatarestapi in (InnovaServerService.serverdatarestapi[])obj)
                {
                    lstox.Items.Add(serverdatarestapi._text);
                }
            }
            catch (Exception exception)
            {
                utilities.logwarning("update_itemlistymme" + exception.ToString());
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.update_itemlistymme(this.listBox3, InnovaServerService.YmmeGetModel(this.listBox2.SelectedItem.ToString()));
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.update_itemlistymme(this.listBox2, InnovaServerService.YmmeGetMake(this.listBox1.SelectedItem.ToString()));
        }

        private void txtrequiredsystems_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            nwscanconfigfile_sync _sync = new nwscanconfigfile_sync(this.rtxt_pathconfig.Text, this.rtxtpathnwscandb.Text, this.txtrequiredsystems.Text);
            Settings.Default.nwscanconfig = this.rtxt_pathconfig.Text;
            Settings.Default.nwscanfolderdb = this.rtxtpathnwscandb.Text;
            Settings.Default.nwscanrequiredsystem = this.txtrequiredsystems.Text;
            Settings.Default.Save();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ProcessStartInfo info;
            //    string str2;
            //    bool flag2;
            //    info = new ProcessStartInfo
            //    {
            //        FileName = @"K:\nws\Firmware\branches\NWSCAN_DEV\NWSCAN_MW\7_Analyzer\ofmanalyzer\bin\Debug\netcoreapp1.0\win10-x64\ofmanalyzer.exe",
            //        Arguments = this.rtxt_base64_bk.Text + " ",
            //        Arguments = info.Arguments + "full ",
            //        Arguments = info.Arguments + "false ",
            //        Arguments = info.Arguments + "1000000 ",
            //        Arguments = info.Arguments.Trim(),
            //        UseShellExecute = false,
            //        RedirectStandardOutput = true
            //    };
            //    Process process = Process.Start(info);
            //    goto Label_00D7;
            //    Label_00A6:
            //    str2 = process.StandardOutput.ReadLine();
            //    if (str2 == null)
            //    {
            //        goto Label_00DC;
            //    }
            //    this.richTextBox2.AppendText(str2 + "\r\n");
            //    Label_00D7:
            //    flag2 = true;
            //    goto Label_00A6;
            //    Label_00DC:
            //    process.Close();
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception);
            //}

        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<classbase64tracking> list = VehicleProfileBackup.getlistbkvehicleprofile();
            if (list != null)
            {
                this.listbox_vehiclebk.Items.Clear();
                foreach (classbase64tracking classbasetracking in list)
                {
                    this.listbox_vehiclebk.Items.Add(classbasetracking.getdispinfo());
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string text = this.rtxt_base64_bk.Text;
            if (!this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, text));
            }

        }

        private void listbox_vehiclebk_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.rtxt_base64_bk.Text = "";
                classbase64tracking classbasetracking = VehicleProfileBackup.getvehicleprofileofymme(this.listbox_vehiclebk.SelectedItem.ToString());
                this.rtxt_base64_bk.Text = classbasetracking.base64profile;
            }
            catch (Exception)
            {
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            object obj2 = InnovaServerService.VehicleProfile_GetData("(vin:\"" + this.txtvinnws.Text + "\",featureSet:\"oemdtc\",forceUpdate:true)", true);
            utilities.logInfo(obj2.ToString());
            if (!this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.RunWorkerAsync(new classtaskinfo(enumtask.task_analyzeserverdata, obj2.ToString()));
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            utilities.Finished();
        }
    }
}
