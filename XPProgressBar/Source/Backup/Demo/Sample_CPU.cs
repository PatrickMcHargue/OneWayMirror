using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Framework.Controls;

namespace XpProgressBarSamples
{
	public class Sample_CPU : System.Windows.Forms.Form
	{
		private XpProgressBar pgbCPU;
		private System.Windows.Forms.Timer tmrCPU;
		private System.Diagnostics.PerformanceCounter pfcCPU;
		private System.ComponentModel.IContainer components;

		public Sample_CPU()
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
			this.components = new System.ComponentModel.Container();
			this.pgbCPU = new XpProgressBar();
			this.tmrCPU = new System.Windows.Forms.Timer(this.components);
			this.pfcCPU = new System.Diagnostics.PerformanceCounter();
			((System.ComponentModel.ISupportInitialize)(this.pfcCPU)).BeginInit();
			this.SuspendLayout();
			// 
			// pgbCPU
			// 
			this.pgbCPU.ColorBackGround = System.Drawing.Color.White;
			this.pgbCPU.ColorBarBorder = System.Drawing.Color.FromArgb(((System.Byte)(170)), ((System.Byte)(240)), ((System.Byte)(170)));
			this.pgbCPU.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(10)), ((System.Byte)(150)), ((System.Byte)(10)));
			this.pgbCPU.ColorText = System.Drawing.Color.Black;
			this.pgbCPU.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.pgbCPU.GradientStyle = GradientMode.Vertical;
			this.pgbCPU.Location = new System.Drawing.Point(8, 8);
			this.pgbCPU.Name = "pgbCPU";
			this.pgbCPU.Position = 60;
			this.pgbCPU.PositionMax = 100;
			this.pgbCPU.PositionMin = 0;
			this.pgbCPU.Size = new System.Drawing.Size(282, 35);
			this.pgbCPU.SteepDistance = 0;
			this.pgbCPU.SteepWidth = 3;
			this.pgbCPU.TabIndex = 3;
			this.pgbCPU.Text = "CPU 13 %";
			// 
			// tmrCPU
			// 
			this.tmrCPU.Enabled = true;
			this.tmrCPU.Interval = 1000;
			this.tmrCPU.Tick += new System.EventHandler(this.tmrCPU_Tick);
			// 
			// pfcCPU
			// 
			this.pfcCPU.CategoryName = "Processor";
			this.pfcCPU.CounterName = "% Processor Time";
			this.pfcCPU.InstanceName = "_Total";
			// 
			// Sample_CPU
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(298, 50);
			this.Controls.Add(this.pgbCPU);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Sample_CPU";
			this.Text = "Processor Time Sample";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.Sample_CPU_Load);
			((System.ComponentModel.ISupportInitialize)(this.pfcCPU)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void tmrCPU_Tick(object sender, System.EventArgs e)
		{
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			int CpuTime = Convert.ToInt32(pfcCPU.NextValue());

			pgbCPU.Text = "     CPU Usage: "  + CpuTime.ToString() + " %";
			pgbCPU.Position = CpuTime;
		}

		private void Sample_CPU_Load(object sender, System.EventArgs e)
		{
			UpdatePosition();
		}
	}
}
