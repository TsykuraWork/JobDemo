
namespace FeistelCipher
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCript2 = new System.Windows.Forms.Button();
            this.buttonDeCript2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(887, 379);
            this.textBox1.TabIndex = 0;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(38, 407);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(406, 51);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Open file";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(38, 464);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(406, 51);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save as";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCript2
            // 
            this.buttonCript2.Location = new System.Drawing.Point(469, 407);
            this.buttonCript2.Name = "buttonCript2";
            this.buttonCript2.Size = new System.Drawing.Size(406, 51);
            this.buttonCript2.TabIndex = 6;
            this.buttonCript2.Text = "Cipher";
            this.buttonCript2.UseVisualStyleBackColor = true;
            this.buttonCript2.Click += new System.EventHandler(this.buttonCript2_Click);
            // 
            // buttonDeCript2
            // 
            this.buttonDeCript2.Location = new System.Drawing.Point(469, 464);
            this.buttonDeCript2.Name = "buttonDeCript2";
            this.buttonDeCript2.Size = new System.Drawing.Size(406, 51);
            this.buttonDeCript2.TabIndex = 7;
            this.buttonDeCript2.Text = "Dechipher";
            this.buttonDeCript2.UseVisualStyleBackColor = true;
            this.buttonDeCript2.Click += new System.EventHandler(this.buttonDeCript2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 525);
            this.Controls.Add(this.buttonDeCript2);
            this.Controls.Add(this.buttonCript2);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "CF_Tsykura";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCript2;
        private System.Windows.Forms.Button buttonDeCript2;
    }
}

