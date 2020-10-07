using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Framework.Controls;


namespace XpProgressBarSamples
{
	public class Sample_Properties : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PropertyGrid propGrid;
		private Framework.Controls.XpProgressBar pgbTest;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Sample_Properties()
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
			this.propGrid = new System.Windows.Forms.PropertyGrid();
			this.pgbTest = new Framework.Controls.XpProgressBar();
			this.SuspendLayout();
			// 
			// propGrid
			// 
			this.propGrid.CommandsVisibleIfAvailable = true;
			this.propGrid.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.propGrid.LargeButtons = false;
			this.propGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propGrid.Location = new System.Drawing.Point(8, 56);
			this.propGrid.Name = "propGrid";
			this.propGrid.SelectedObject = this.pgbTest;
			this.propGrid.Size = new System.Drawing.Size(352, 400);
			this.propGrid.TabIndex = 5;
			this.propGrid.Text = "propertyGrid1";
			this.propGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// pgbTest
			// 
			this.pgbTest.ColorBackGround = System.Drawing.Color.White;
			this.pgbTest.ColorBarBorder = System.Drawing.Color.Blue;
			this.pgbTest.ColorBarCenter = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(64)));
			this.pgbTest.ColorText = System.Drawing.Color.Yellow;
			this.pgbTest.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.pgbTest.GradientStyle = Framework.Controls.GradientMode.Vertical;
			this.pgbTest.Location = new System.Drawing.Point(8, 8);
			this.pgbTest.Name = "pgbTest";
			this.pgbTest.Position = 80;
			this.pgbTest.PositionMax = 100;
			this.pgbTest.PositionMin = 0;
			this.pgbTest.Size = new System.Drawing.Size(351, 35);
			this.pgbTest.SteepDistance = ((System.Byte)(0));
			this.pgbTest.SteepWidth = ((System.Byte)(3));
			this.pgbTest.TabIndex = 6;
			this.pgbTest.Text = "www.CodeProject.com";
			this.pgbTest.TextShadowAlpha = ((System.Byte)(240));
			// 
			// Sample_Properties
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(368, 461);
			this.Controls.Add(this.pgbTest);
			this.Controls.Add(this.propGrid);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Sample_Properties";
			this.Text = " Properties Sample";
			this.Load += new System.EventHandler(this.Sample_Properties_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Sample_Properties_Load(object sender, System.EventArgs e)
		{
			propGrid.CollapseAllGridItems();
		}

	}
}
