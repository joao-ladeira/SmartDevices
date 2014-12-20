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

            SmartTvRemote remote = new SmartTvRemote();
        }
    }
}
