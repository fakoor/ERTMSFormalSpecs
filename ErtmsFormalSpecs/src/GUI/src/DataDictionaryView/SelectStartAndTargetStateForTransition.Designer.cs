﻿namespace GUI.DataDictionaryView
{
    partial class SelectStartAndTargetStateForTransition
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectStartAndTargetStateForTransition));
            this.label1 = new System.Windows.Forms.Label();
            this.startStatesComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.endStatesComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start state";
            // 
            // startStatesComboBox
            // 
            this.startStatesComboBox.FormattingEnabled = true;
            this.startStatesComboBox.Location = new System.Drawing.Point(107, 12);
            this.startStatesComboBox.Name = "startStatesComboBox";
            this.startStatesComboBox.Size = new System.Drawing.Size(194, 21);
            this.startStatesComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "End state";
            // 
            // endStatesComboBox
            // 
            this.endStatesComboBox.FormattingEnabled = true;
            this.endStatesComboBox.Location = new System.Drawing.Point(107, 39);
            this.endStatesComboBox.Name = "endStatesComboBox";
            this.endStatesComboBox.Size = new System.Drawing.Size(194, 21);
            this.endStatesComboBox.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(15, 67);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(286, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SelectStartAndTargetStateForTransition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 103);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.endStatesComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.startStatesComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectStartAndTargetStateForTransition";
            this.Text = "Select start and target states for transition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox startStatesComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox endStatesComboBox;
        private System.Windows.Forms.Button okButton;
    }
}