using System;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private static CspParameters cspp = new CspParameters();
        private static AesCryptoServiceProvider cryptoAES;
        const string keyName = "Key";
        private static RSACryptoServiceProvider rsa;
        public static string keyFile = null;

        public Form1()
        {
            InitializeComponent();
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            cryptoAES = new AesCryptoServiceProvider();
            cryptoAES.BlockSize = 128;
            cryptoAES.KeySize = 128;
            cryptoAES.Mode = CipherMode.CBC;

        }

        private void EncryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "All files (*.*)|*.*|Шифр (*.crpt)|*.crpt";

            openFile.Title = "Выберите файл для шифрования.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Шифр (*.crpt)|*.crpt|All files (*.*)|*.*";

                    saveFile.Title = "Укажите имя для зашифрованного файла.";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        string encryptFileName = saveFile.FileName;
                        if (encryptFileName != null)
                        {
                            Encrypt(fileName, encryptFileName, keyFile);
                        }
                    }
                }
            }
        }

        private static void Encrypt(string fileName, string encryptFileName, string keyFile)
        {
            if (keyFile != null)
            {
                StreamReader sr = new StreamReader(keyFile);
                rsa.FromXmlString(sr.ReadToEnd());
                sr.Close();
            }
            ICryptoTransform transform = cryptoAES.CreateEncryptor();
            byte[] keyEncrypted = rsa.Encrypt(cryptoAES.Key, false);

            int lKey = keyEncrypted.Length;
            byte[] LenK = BitConverter.GetBytes(lKey);
            int lIV = cryptoAES.IV.Length;
            byte[] LenIV = BitConverter.GetBytes(lIV);

            FileStream outFs = new FileStream(encryptFileName, FileMode.Create);
            outFs.Write(LenK, 0, 4);
            outFs.Write(LenIV, 0, 4);
            outFs.Write(keyEncrypted, 0, lKey);
            outFs.Write(cryptoAES.IV, 0, lIV);

            CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write);
            int count = 0;
            int offset = 0;

            // blockSizeBytes can be any arbitrary size.
            int blockSize = cryptoAES.BlockSize / 8;
            byte[] data = new byte[blockSize];
            int bytesRead = 0;

            FileStream inFs = new FileStream(fileName, FileMode.Open);
            do
            {
                count = inFs.Read(data, 0, blockSize);
                offset += count;
                outStreamEncrypted.Write(data, 0, count);
                bytesRead += blockSize;
            }
            while (count > 0);
            inFs.Close();
            outStreamEncrypted.FlushFinalBlock();
            outStreamEncrypted.Close();
        }

        private void DecryptFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Шифр (*.crpt)|*.crpt|All files (*.*)|*.*";

            openFile.Title = "Выберите файл для расшифрования.";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFile.FileName;
                if (fileName != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "All files (*.*)|*.*|Шифр (*.crpt)|*.crpt";

                    saveFile.Title = "Укажите имя для расшифрованного файла.";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        string encryptFileName = saveFile.FileName;
                        if (encryptFileName != null)
                        {
                            Decrypt(fileName, encryptFileName);
                        }
                    }
                }
            }
        }

        private static void Decrypt(string fileName, string encryptFileName)
        {
            FileStream inFs = new FileStream(fileName, FileMode.Open);

            byte[] LenK = new byte[4];
            inFs.Read(LenK, 0, 4);
            byte[] LenIV = new byte[4];
            inFs.Read(LenIV, 0, 4);

            int lenK = BitConverter.ToInt32(LenK, 0);
            int lenIV = BitConverter.ToInt32(LenIV, 0);

            int startC = lenK + lenIV + 8;
            int lenC = (int)inFs.Length - startC;

            byte[] KeyEncrypted = new byte[lenK];
            byte[] IV = new byte[lenIV];

            inFs.Seek(8, SeekOrigin.Begin);
            inFs.Read(KeyEncrypted, 0, lenK);
            inFs.Seek(8 + lenK, SeekOrigin.Begin);
            inFs.Read(IV, 0, lenIV);

            byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

            ICryptoTransform transform = cryptoAES.CreateDecryptor(KeyDecrypted, IV);

            FileStream outFs = new FileStream(encryptFileName, FileMode.Create);

            int count = 0;
            int offset = 0;

            int blockSizeBytes = cryptoAES.BlockSize / 8;//AES - это блочный шифр, поэтому простой текст (128 бит) данных сначала преобразуется в матрицу 4×4. Каждый элемент матрицы имеет размер 8 битов, в результате чего матрица содержит 8×16 = 128 бит.
            byte[] data = new byte[blockSizeBytes];


            inFs.Seek(startC, SeekOrigin.Begin);
            CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write);
            do
            {
                count = inFs.Read(data, 0, blockSizeBytes);
                offset += count;
                outStreamDecrypted.Write(data, 0, count);
            }
            while (count > 0);

            outStreamDecrypted.FlushFinalBlock();
            outStreamDecrypted.Close();
            outFs.Close();
            inFs.Close();
        }

        private void ExportPublicKey_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Введите путь к файлу для экспортирования ключа";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                keyFile = saveFile.FileName;

                if (keyFile != null)
                {
                    StreamWriter sw = new StreamWriter(keyFile);
                    sw.Write(rsa.ToXmlString(radioButton2.Checked));
                    sw.Close();
                }
            }
        }

        private void ImportPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Введите файл для импорта открытого ключа";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                keyFile = openFile.FileName;
            }
        }
    }
}
