namespace SparkWsTest
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
            this.btnInsertRecord = new System.Windows.Forms.Button();
            this.lblInsertRecord = new System.Windows.Forms.Label();
            this.btnRetrieveRecord = new System.Windows.Forms.Button();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnInsertRecord
            // 
            this.btnInsertRecord.Location = new System.Drawing.Point(12, 29);
            this.btnInsertRecord.Name = "btnInsertRecord";
            this.btnInsertRecord.Size = new System.Drawing.Size(107, 23);
            this.btnInsertRecord.TabIndex = 0;
            this.btnInsertRecord.Text = "InsertRecord";
            this.btnInsertRecord.UseVisualStyleBackColor = true;
            this.btnInsertRecord.Click += new System.EventHandler(this.btnInsertRecord_Click);
            // 
            // lblInsertRecord
            // 
            this.lblInsertRecord.AutoSize = true;
            this.lblInsertRecord.Location = new System.Drawing.Point(136, 34);
            this.lblInsertRecord.Name = "lblInsertRecord";
            this.lblInsertRecord.Size = new System.Drawing.Size(78, 13);
            this.lblInsertRecord.TabIndex = 1;
            this.lblInsertRecord.Text = "lblInsertRecord";
            // 
            // btnRetrieveRecord
            // 
            this.btnRetrieveRecord.Location = new System.Drawing.Point(12, 80);
            this.btnRetrieveRecord.Name = "btnRetrieveRecord";
            this.btnRetrieveRecord.Size = new System.Drawing.Size(107, 23);
            this.btnRetrieveRecord.TabIndex = 2;
            this.btnRetrieveRecord.Text = "RetrieveRecord(s)";
            this.btnRetrieveRecord.UseVisualStyleBackColor = true;
            this.btnRetrieveRecord.Click += new System.EventHandler(this.btnRetrieveRecord_Click);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(136, 85);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(80, 13);
            this.lblRecordCount.TabIndex = 3;
            this.lblRecordCount.Text = "lblRecordCount";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.btnRetrieveRecord);
            this.Controls.Add(this.lblInsertRecord);
            this.Controls.Add(this.btnInsertRecord);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInsertRecord;
        private System.Windows.Forms.Label lblInsertRecord;
        private System.Windows.Forms.Button btnRetrieveRecord;
        private System.Windows.Forms.Label lblRecordCount;
    }
}

