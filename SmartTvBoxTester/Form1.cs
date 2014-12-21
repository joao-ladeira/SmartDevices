using SmartDevice.TV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTvBoxTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SmartTvRemote remote = new SmartTvRemote("192.168.1.84");

            //change to channel 14 example
            remote.SendKey(SmartTvRemote.SRKeyCode.Key1);
            remote.SendKey(SmartTvRemote.SRKeyCode.Key4);
        }
    }
}
