using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Framework.Controls;

namespace XpProgressBarSamples

{
	public class Sample_Animation : System.Windows.Forms.Form
	{
		private System.Timers.Timer timer1;
		private XpProgressBar prog1;
		private XpProgressBar prog2;
		private XpProgressBar prog3;
		private System.Timers.Timer timer2;
		private System.Timers.Timer timer3;
		private XpProgressBar prog4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Sample_Animation()
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
			this.timer1 = new System.Timers.Timer();
			this.timer2 = new System.Timers.Timer();
			this.timer3 = new System.Timers.Timer();
			this.prog4 = new Framework.Controls.XpProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timer2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timer3)).BeginInit();
			this.SuspendLayout();
			// 
			// prog1
			// 
			this.prog1.ColorBackGround = System.Drawing.Color.White;
			this.prog1.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog1.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog1.ColorText = System.Drawing.Color.Black;
			this.prog1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog1.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.prog1.Location = new System.Drawing.Point(16, 24);
			this.prog1.Name = "prog1";
			this.prog1.Position = 20;
			this.prog1.PositionMax = 100;
			this.prog1.PositionMin = 0;
			this.prog1.Size = new System.Drawing.Size(334, 44);
			this.prog1.TabIndex = 5;
			this.prog1.Text = "13 %";
			// 
			// prog2
			// 
			this.prog2.ColorBackGround = System.Drawing.Color.White;
			this.prog2.ColorBarBorder = System.Drawing.Color.White;
			this.prog2.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(192)), ((System.Byte)(192)));
			this.prog2.ColorText = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.prog2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog2.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.prog2.Location = new System.Drawing.Point(16, 88);
			this.prog2.Name = "prog2";
			this.prog2.Position = 80;
			this.prog2.PositionMax = 100;
			this.prog2.PositionMin = 0;
			this.prog2.Size = new System.Drawing.Size(336, 38);
			this.prog2.SteepDistance = ((System.Byte)(1));
			this.prog2.SteepWidth = ((System.Byte)(4));
			this.prog2.TabIndex = 6;
			this.prog2.Text = "10";
			// 
			// prog3
			// 
			this.prog3.ColorBackGround = System.Drawing.Color.White;
			this.prog3.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.prog3.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.prog3.ColorText = System.Drawing.Color.Gainsboro;
			this.prog3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog3.Location = new System.Drawing.Point(16, 144);
			this.prog3.Name = "prog3";
			this.prog3.Position = 35;
			this.prog3.PositionMax = 100;
			this.prog3.PositionMin = 0;
			this.prog3.Size = new System.Drawing.Size(336, 35);
			this.prog3.SteepDistance = ((System.Byte)(0));
			this.prog3.SteepWidth = ((System.Byte)(3));
			this.prog3.TabIndex = 7;
			this.prog3.Text = "This is a Fixed Text with Antialising";
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 70;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// timer2
			// 
			this.timer2.Enabled = true;
			this.timer2.Interval = 50;
			this.timer2.SynchronizingObject = this;
			this.timer2.Elapsed += new System.Timers.ElapsedEventHandler(this.timer2_Elapsed);
			// 
			// timer3
			// 
			this.timer3.Enabled = true;
			this.timer3.SynchronizingObject = this;
			this.timer3.Elapsed += new System.Timers.ElapsedEventHandler(this.timer3_Elapsed);
			// 
			// prog4
			// 
			this.prog4.ColorBackGround = System.Drawing.Color.White;
			this.prog4.ColorBarBorder = System.Drawing.Color.Blue;
			this.prog4.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(64)));
			this.prog4.ColorText = System.Drawing.Color.White;
			this.prog4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.prog4.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.prog4.Location = new System.Drawing.Point(16, 200);
			this.prog4.Name = "prog4";
			this.prog4.Position = 35;
			this.prog4.PositionMax = 100;
			this.prog4.PositionMin = 0;
			this.prog4.Size = new System.Drawing.Size(336, 35);
			this.prog4.SteepDistance = ((System.Byte)(0));
			this.prog4.SteepWidth = ((System.Byte)(3));
			this.prog4.TabIndex = 8;
			this.prog4.Text = "Full Customizable ProgressBar";
			// 
			// Sample_Animation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 245);
			this.Controls.Add(this.prog4);
			this.Controls.Add(this.prog3);
			this.Controls.Add(this.prog2);
			this.Controls.Add(this.prog1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Sample_Animation";
			this.Text = "  Animation Sample";
			this.Load += new System.EventHandler(this.Sample_Animation_Load);
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.timer2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.timer3)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		int c1;
		int c2;
		int c3;

		int p1 = 1;
		int p2 = -1;
		int p3 = 1;
		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (c1 == prog1.PositionMax)
			{
				p1 = -1;
			}
			else
			if (c1 == prog1.PositionMin)
			{
				p1 = 1;
			}
			c1 += p1;

			prog1.Text = c1.ToString() + " %";
			prog1.Position = c1;
		
		}

		private void Sample_Animation_Load(object sender, System.EventArgs e)
		{
			c1 = prog1.Position;
			c2 = prog2.Position;
			c3 = prog3.Position;
		
		}

		private void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (c2 == prog2.PositionMax)
			{
				p2 = -1;
			}
			else
				if (c2 == prog2.PositionMin)
			{
				p2 = 1;
			}
			c2 += p2;

			prog2.Text = c2.ToString();
			prog2.Position = c2;
		

		
		}

		private void timer3_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (c3 == prog3.PositionMax)
			{
				p3 = -1;
			}
			else
				if (c3 == prog3.PositionMin)
			{
				p3 = 1;
			}
			c3 += p3;

			prog3.Position = c3;
			prog4.Position = 100 - c3;
		}
	}
}
