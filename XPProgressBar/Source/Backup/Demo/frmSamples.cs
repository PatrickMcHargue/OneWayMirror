using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Framework.Controls;

namespace XpProgressBarSamples
{
	public class frmSamples : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private Framework.Controls.XpProgressBar Prog1;
		private Framework.Controls.XpProgressBar Prog3;
		private Framework.Controls.XpProgressBar Prog2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmSamples()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.Prog1 = new Framework.Controls.XpProgressBar();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.Prog3 = new Framework.Controls.XpProgressBar();
			this.button4 = new System.Windows.Forms.Button();
			this.Prog2 = new Framework.Controls.XpProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(22, 10);
			this.trackBar1.Maximum = 100;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(300, 45);
			this.trackBar1.TabIndex = 3;
			this.trackBar1.TickFrequency = 3;
			this.trackBar1.Value = 81;
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// Prog1
			// 
			this.Prog1.ColorBackGround = System.Drawing.Color.White;
			this.Prog1.ColorBarBorder = System.Drawing.Color.Blue;
			this.Prog1.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(64)));
			this.Prog1.ColorText = System.Drawing.Color.White;
			this.Prog1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Prog1.GradientStyle = Framework.Controls.GradientMode.Horizontal;
			this.Prog1.Location = new System.Drawing.Point(20, 55);
			this.Prog1.Name = "Prog1";
			this.Prog1.Position = 81;
			this.Prog1.PositionMax = 100;
			this.Prog1.PositionMin = 0;
			this.Prog1.Size = new System.Drawing.Size(302, 39);
			this.Prog1.SteepDistance = ((System.Byte)(0));
			this.Prog1.SteepWidth = ((System.Byte)(2));
			this.Prog1.TabIndex = 0;
			this.Prog1.Text = "File 81 of 100";
			this.Prog1.TextShadowAlpha = ((System.Byte)(100));
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(329, 17);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(92, 36);
			this.button1.TabIndex = 1;
			this.button1.Text = "&CPU Usage";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(329, 63);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(92, 36);
			this.button2.TabIndex = 2;
			this.button2.Text = "A&nimation";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(329, 109);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(92, 36);
			this.button3.TabIndex = 4;
			this.button3.Text = "&Properties";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// Prog3
			// 
			this.Prog3.ColorBackGround = System.Drawing.Color.White;
			this.Prog3.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.Prog3.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.Prog3.ColorText = System.Drawing.Color.Black;
			this.Prog3.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Prog3.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.Prog3.Location = new System.Drawing.Point(20, 151);
			this.Prog3.Name = "Prog3";
			this.Prog3.Position = 81;
			this.Prog3.PositionMax = 100;
			this.Prog3.PositionMin = 0;
			this.Prog3.Size = new System.Drawing.Size(302, 39);
			this.Prog3.TabIndex = 5;
			this.Prog3.Text = "Marcos Meli";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(329, 153);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(92, 36);
			this.button4.TabIndex = 6;
			this.button4.Text = "&GradientMode";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// Prog2
			// 
			this.Prog2.ColorBackGround = System.Drawing.Color.White;
			this.Prog2.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(224)), ((System.Byte)(192)));
			this.Prog2.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(64)), ((System.Byte)(0)));
			this.Prog2.ColorText = System.Drawing.Color.White;
			this.Prog2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Prog2.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.Prog2.Location = new System.Drawing.Point(20, 102);
			this.Prog2.Name = "Prog2";
			this.Prog2.Position = 81;
			this.Prog2.PositionMax = 100;
			this.Prog2.PositionMin = 0;
			this.Prog2.Size = new System.Drawing.Size(302, 39);
			this.Prog2.SteepDistance = ((System.Byte)(0));
			this.Prog2.SteepWidth = ((System.Byte)(2));
			this.Prog2.TabIndex = 7;
			this.Prog2.Text = "Fixed Text";
			this.Prog2.TextShadowAlpha = ((System.Byte)(100));
			// 
			// frmSamples
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(434, 201);
			this.Controls.Add(this.Prog1);
			this.Controls.Add(this.Prog2);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.Prog3);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.trackBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmSamples";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " Full Customizable XpProgressBar";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.Run(new frmSamples());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
				trackBar1.Value = 81;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			(new Sample_CPU()).Show();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			(new Sample_Animation()).Show();
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			(new Sample_Properties()).Show();
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			(new Sample_GradientMode()).Show();
		}

		private void trackBar1_ValueChanged(object sender, System.EventArgs e)
		{
			Prog1.Text = "File " + trackBar1.Value.ToString() + " of 100";
			Prog1.Position = trackBar1.Value;
			Prog2.Position = trackBar1.Value;
			Prog3.Position = trackBar1.Value;
		
		}

	}
}
