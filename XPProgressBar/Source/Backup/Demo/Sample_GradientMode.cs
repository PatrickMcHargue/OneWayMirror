using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using Framework.Controls;
using Timer = System.Timers.Timer;

namespace XpProgressBarSamples
{
	public class Sample_GradientMode : Form
	{
		private XpProgressBar prog1;
		private Label label1;
		private Timer timer1;
		private XpProgressBar prog2;
		private XpProgressBar prog3;
		private XpProgressBar prog4;
		private XpProgressBar prog5;
		private Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Sample_GradientMode()
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
				if(components != null)
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
			this.prog1 = new Framework.Controls.XpProgressBar();
			this.prog2 = new Framework.Controls.XpProgressBar();
			this.prog3 = new Framework.Controls.XpProgressBar();
			this.prog4 = new Framework.Controls.XpProgressBar();
			this.prog5 = new Framework.Controls.XpProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.timer1 = new System.Timers.Timer();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this.SuspendLayout();
			// 
			// prog1
			// 
			this.prog1.ColorBackGround = System.Drawing.Color.White;
			this.prog1.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog1.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog1.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(240)), ((System.Byte)(240)), ((System.Byte)(240)));
			this.prog1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog1.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.prog1.Location = new System.Drawing.Point(16, 40);
			this.prog1.Name = "prog1";
			this.prog1.Position = 100;
			this.prog1.PositionMax = 100;
			this.prog1.PositionMin = 0;
			this.prog1.Size = new System.Drawing.Size(276, 32);
			this.prog1.SteepDistance = ((System.Byte)(0));
			this.prog1.SteepWidth = ((System.Byte)(1));
			this.prog1.TabIndex = 9;
			this.prog1.Text = "Vertical";
			// 
			// prog2
			// 
			this.prog2.ColorBackGround = System.Drawing.Color.White;
			this.prog2.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog2.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog2.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(240)), ((System.Byte)(240)), ((System.Byte)(240)));
			this.prog2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog2.Location = new System.Drawing.Point(16, 80);
			this.prog2.Name = "prog2";
			this.prog2.Position = 100;
			this.prog2.PositionMax = 100;
			this.prog2.PositionMin = 0;
			this.prog2.Size = new System.Drawing.Size(276, 32);
			this.prog2.SteepDistance = ((System.Byte)(0));
			this.prog2.SteepWidth = ((System.Byte)(1));
			this.prog2.TabIndex = 10;
			this.prog2.Text = "VerticalCenter";
			// 
			// prog3
			// 
			this.prog3.ColorBackGround = System.Drawing.Color.White;
			this.prog3.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog3.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog3.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(240)), ((System.Byte)(240)), ((System.Byte)(240)));
			this.prog3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog3.GradientStyle = Framework.Controls.GradientMode.Horizontal;
			this.prog3.Location = new System.Drawing.Point(16, 120);
			this.prog3.Name = "prog3";
			this.prog3.Position = 100;
			this.prog3.PositionMax = 100;
			this.prog3.PositionMin = 0;
			this.prog3.Size = new System.Drawing.Size(276, 32);
			this.prog3.SteepDistance = ((System.Byte)(0));
			this.prog3.SteepWidth = ((System.Byte)(1));
			this.prog3.TabIndex = 11;
			this.prog3.Text = "Horizontal";
			// 
			// prog4
			// 
			this.prog4.ColorBackGround = System.Drawing.Color.White;
			this.prog4.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog4.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog4.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(240)), ((System.Byte)(240)), ((System.Byte)(240)));
			this.prog4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog4.GradientStyle = Framework.Controls.GradientMode.HorizontalCenter;
			this.prog4.Location = new System.Drawing.Point(16, 160);
			this.prog4.Name = "prog4";
			this.prog4.Position = 100;
			this.prog4.PositionMax = 100;
			this.prog4.PositionMin = 0;
			this.prog4.Size = new System.Drawing.Size(276, 32);
			this.prog4.SteepDistance = ((System.Byte)(0));
			this.prog4.SteepWidth = ((System.Byte)(1));
			this.prog4.TabIndex = 12;
			this.prog4.Text = "HorizontalCenter";
			// 
			// prog5
			// 
			this.prog5.ColorBackGround = System.Drawing.Color.White;
			this.prog5.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog5.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog5.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(240)), ((System.Byte)(240)), ((System.Byte)(240)));
			this.prog5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog5.GradientStyle = Framework.Controls.GradientMode.Diagonal;
			this.prog5.Location = new System.Drawing.Point(16, 200);
			this.prog5.Name = "prog5";
			this.prog5.Position = 100;
			this.prog5.PositionMax = 100;
			this.prog5.PositionMin = 0;
			this.prog5.Size = new System.Drawing.Size(276, 32);
			this.prog5.SteepDistance = ((System.Byte)(0));
			this.prog5.SteepWidth = ((System.Byte)(1));
			this.prog5.TabIndex = 13;
			this.prog5.Text = "Diagonal";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 24);
			this.label1.TabIndex = 14;
			this.label1.Text = "Diferents Values For The \"GradientMode\" Property";
			// 
			// timer1
			// 
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.button1.Location = new System.Drawing.Point(112, 248);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 32);
			this.button1.TabIndex = 15;
			this.button1.Text = "Start";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Sample_GradientMode
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 287);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.prog5);
			this.Controls.Add(this.prog4);
			this.Controls.Add(this.prog3);
			this.Controls.Add(this.prog2);
			this.Controls.Add(this.prog1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Sample_GradientMode";
			this.Text = "GradientStyle Sample";
			this.Load += new System.EventHandler(this.Sample_GradientMode_Load);
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		int c2;
		int p2 = 1;

		private void timer1_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (c2 == prog2.PositionMax)
			{
				p2 = -1;
			}
			else
				if (c2 == 1)
			{
				p2 = 1;
			}
			c2 += p2;

			prog1.Position = c2;
			prog2.Position = c2;
			prog3.Position = c2;
			prog4.Position = c2;
			prog5.Position = c2;

			Application.DoEvents();
		}


		private void Sample_GradientMode_Load(object sender, EventArgs e)
		{
			c2 = prog2.Position;
		}

		private void button1_Click(object sender, EventArgs e)
		{

			timer1.Enabled = ! timer1.Enabled;
			if (timer1.Enabled)
			{
				button1.Text = "Pause";
			}
			else
			{
				button1.Text = "Start";
			}

		}

	}
}
