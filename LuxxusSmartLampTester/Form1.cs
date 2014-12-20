using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SmartDevice.Light;
using SmartDevice.Light.Luxxus;

namespace LuxxusSmartLampTest
{
    public partial class Form1 : Form
    {
        ISmartLightController smartLightController;

        public Form1()
        {
            InitializeComponent();

            this.smartLightController = new LuxxusSmartLightController();
        }
        
        private void buttonSet_Click(object sender, EventArgs e)
        {
            uint deviceId = 0;

            if (uint.TryParse(textBoxLightId.Text, out deviceId))
            {
                byte intensity = (byte)trackBarIntensity.Value;

                SmartLight[] lights = smartLightController.GetLights();

                foreach (SmartLight light in lights)
                {
                    light.State.Intensity = intensity;
                    light.State.Color = new SmartLightColor(panelLampColor.BackColor.R, panelLampColor.BackColor.G, panelLampColor.BackColor.B);
                }

                smartLightController.SetLights(lights);
            }
        }

        private void buttonChangeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult dRes = colorDialog.ShowDialog();

            if (dRes == System.Windows.Forms.DialogResult.OK)
            {
                panelLampColor.BackColor = colorDialog.Color;
            }
        }

    }
}
