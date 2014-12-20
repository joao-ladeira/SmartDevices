namespace LuxxusSmartLampTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSet = new System.Windows.Forms.Button();
            this.trackBarIntensity = new System.Windows.Forms.TrackBar();
            this.textBoxLightId = new System.Windows.Forms.TextBox();
            this.buttonChangeColor = new System.Windows.Forms.Button();
            this.panelLampColor = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSet
            // 
            this.buttonSet.Location = new System.Drawing.Point(13, 103);
            this.buttonSet.Name = "buttonSet";
            this.buttonSet.Size = new System.Drawing.Size(461, 80);
            this.buttonSet.TabIndex = 0;
            this.buttonSet.Text = "Set";
            this.buttonSet.UseVisualStyleBackColor = true;
            this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
            // 
            // trackBarIntensity
            // 
            this.trackBarIntensity.Location = new System.Drawing.Point(13, 41);
            this.trackBarIntensity.Maximum = 255;
            this.trackBarIntensity.Name = "trackBarIntensity";
            this.trackBarIntensity.Size = new System.Drawing.Size(462, 56);
            this.trackBarIntensity.TabIndex = 2;
            this.trackBarIntensity.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // textBoxLightId
            // 
            this.textBoxLightId.Location = new System.Drawing.Point(13, 13);
            this.textBoxLightId.Name = "textBoxLightId";
            this.textBoxLightId.Size = new System.Drawing.Size(461, 22);
            this.textBoxLightId.TabIndex = 3;
            this.textBoxLightId.Text = "2614203273";
            this.textBoxLightId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonChangeColor
            // 
            this.buttonChangeColor.Location = new System.Drawing.Point(13, 189);
            this.buttonChangeColor.Name = "buttonChangeColor";
            this.buttonChangeColor.Size = new System.Drawing.Size(461, 33);
            this.buttonChangeColor.TabIndex = 4;
            this.buttonChangeColor.Text = "Change Color";
            this.buttonChangeColor.UseVisualStyleBackColor = true;
            this.buttonChangeColor.Click += new System.EventHandler(this.buttonChangeColor_Click);
            // 
            // panelLampColor
            // 
            this.panelLampColor.BackColor = System.Drawing.Color.White;
            this.panelLampColor.Location = new System.Drawing.Point(13, 229);
            this.panelLampColor.Name = "panelLampColor";
            this.panelLampColor.Size = new System.Drawing.Size(461, 36);
            this.panelLampColor.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 277);
            this.Controls.Add(this.panelLampColor);
            this.Controls.Add(this.buttonChangeColor);
            this.Controls.Add(this.textBoxLightId);
            this.Controls.Add(this.trackBarIntensity);
            this.Controls.Add(this.buttonSet);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSet;
        private System.Windows.Forms.TrackBar trackBarIntensity;
        private System.Windows.Forms.TextBox textBoxLightId;
        private System.Windows.Forms.Button buttonChangeColor;
        private System.Windows.Forms.Panel panelLampColor;
    }
}

