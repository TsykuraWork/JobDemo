using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FeistelCipher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Dictionary<int, char> itc = new Dictionary<int, char>();
        Dictionary<char, int> cti = new Dictionary<char, int>();
        string criptKey = "";
        string abc = "";//язык беру из файла

        private void Form1_Load(object sender, EventArgs e)
        {
            //заполняю словари
            string path = @"..\..\..\cript.txt";
            if (!File.Exists(@path))
            {
                MessageBox.Show("Отсутствует часть файлов.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-660);
            }
            foreach (string str in File.ReadLines(path))
            {
                if (criptKey == "")
                {
                    criptKey = str;
                    continue;
                }
                if(abc == "")
                {
                    abc = str;
                    break;
                }
            }
            
            int i = 0;//Формирую ассоциативные словари
            foreach(char ch in abc.ToCharArray())
            {
                i++;
                itc.Add(i, ch);
                cti.Add(ch, i);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Текст|*.txt";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = myDialog.FileName;
                string text = "";
                foreach (string str in File.ReadLines(fileName))
                {
                    text += str + "\r\n";
                }
                textBox1.Text = text;
            }
        }
        /// <summary>
        /// Метод цикличной записи ключа
        /// </summary>
        /// <param name="len">Длина шифруемого текста</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        private string LoopKey(int len,string key)
        {
            int keyLen = key.Length; //Длина ключа
            for (int i = 0; i < len / keyLen + 1; i++)
                key += key;
            key = key.Substring(0, len);
            return key;
        }

        private void buttonCript1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                char[] arr = textBox1.Text.ToCharArray();
                //циклично заполняю ключ
                char[] arrKey = LoopKey(arr.Length, criptKey).ToCharArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    //получаю порядковый номер буквы в алфавите(алфавит получен из файла и загружен в словари)
                    if (cti.ContainsKey(arr[i]) == true)
                        arr[i] = itc[(cti[arr[i]] + cti[arrKey[i]] - 1) % 33 + 1];
                    else
                    {
                        if (cti.ContainsKey(char.ToLower(arr[i])) == true)
                            arr[i] = char.ToUpper(itc[(cti[char.ToLower(arr[i])] + cti[arrKey[i]] - 1) % 33 + 1]);
                        else
                            continue;
                    }
                }
                textBox1.Text = new string(arr);
            }
        }

        private void buttonDeCript1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                char[] arr = textBox1.Text.ToCharArray();
                //циклично заполняю ключ
                char[] arrKey = LoopKey(arr.Length, criptKey).ToCharArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    //получаю порядковый номер буквы в алфавите(алфавит получен из файла и загружен в словари)
                    if (cti.ContainsKey(arr[i]) == true)
                    {
                        int temp = cti[arr[i]] - cti[arrKey[i]] - 1;
                        if (temp >= 0)
                            arr[i] = itc[temp % 33 + 1];
                        else
                            arr[i] = itc[33 + temp % 33 + 1];
                    }
                    else
                    {
                        if (cti.ContainsKey(char.ToLower(arr[i])) == true)
                        {
                            int temp = cti[char.ToLower(arr[i])] - cti[arrKey[i]] - 1;
                            if (temp >= 0)
                                arr[i] = char.ToUpper(itc[temp % 33 + 1]);
                            else
                                arr[i] = char.ToUpper(itc[33 + temp % 33 + 1]);
                        }
                        else
                            continue;
                    }
                }
                textBox1.Text = new string(arr);
            }
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog myDialog = new SaveFileDialog();
            myDialog.Filter = "Текст|*.txt";

            string fname = "CriptFile";
            int i = 0;
            while (File.Exists(fname + ++i + ".txt")) { continue; }
            fname += i + ".txt";

            myDialog.FileName = fname;
            if (myDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllText(myDialog.FileName, textBox1.Text);
}
        bool IsCriptFeidelDid = false;
        private void buttonCript2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                if (IsCriptFeidelDid == false)//если не шифрован
                {
                    IsCriptFeidelDid = true;//шифрован
                    Encoding enc = Encoding.UTF8;
                    byte[] criptResult;
                    byte[] key;
                    criptText.Clear();
                    feistelKeys.Clear();
                    dopLength = 0;//для заполнения пустоты
                    byte[] barr = enc.GetBytes(textBox1.Text);//все байты шифруемого текста 
                    byte[] block = new byte[16];//128б = 16Б

                    byte[] barr16 = new byte[barr.Length - barr.Length % 16 + 16];//помещаем текст в контейнер, размерность которого кратна 16Б

                    for (int i = 0; i < barr16.Length; i++)
                    {
                        if (i < barr.Length)
                        {
                            barr16[i] = barr[i];//распределем текст по блокам
                        }
                        else//если последний блок не полный(т.е. текст не занимает всё пространство блока)
                        {
                            dopLength++;//лишнюю часть заполняем 0
                            barr16[i] = 0;
                        }
                    }

                    for (int i = 0; i < barr16.Length - 15; i += 16)//выделяем блоки по 16Б
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            block[j] = barr16[i + j];//и заполняем каждый байт
                        }
                        key = CreateRoundKey(block);//создаётся ключ зависящий от текста (для 8-ми раундов)
                        criptResult = CriptFeistel(block, key);//шифровка блока
                        feistelKeys.Add(key);//сохраняем ключ
                        criptText.Add(criptResult);//сохраняем результат шифрования
                    }

                    textBox1.Text = "";
                    foreach (byte[] Block in criptText)//выводим результат на экран
                    {
                        textBox1.Text += enc.GetString(Block);//отправляю по 4 байта
                    }
                }
            }
        }

        List<byte[]> criptText = new List<byte[]>();
        List<byte[]> feistelKeys = new List<byte[]>();
        int dopLength;
        private byte[] CreateRoundKey(byte[] block)
        {
            byte[] createdKey = new byte[16];//размер ключа равен размеру блока
            short shortKey;//так как необходимо реализовать 8 раундов преобразования, каждый ключ будет размером в 2Б 
            List<byte[]> bytelist = new List<byte[]>();
            short[] prost = { 2, 3, 5, 7, 11, 13, 17, 19 };

            byte[] byteKey = new byte[2];
            short shortTextPiece;
            for (int i = 0, c = 0; i < 8; i++, c += 2)
            {
                byteKey[0] = block[c];//Каждая пара байт текста сопровождается своим ключом
                byteKey[1] = block[c + 1];
                shortTextPiece = BitConverter.ToInt16(byteKey, 0);//перевожу байты текста в шорт
                shortKey = (short)(shortTextPiece * (prost[i] * prost[i]));//создаю ключ зависящий от текста
                bytelist.Add(BitConverter.GetBytes(shortKey));
            }

            int j = 0;
            foreach (byte[] bt in bytelist)
            {
                createdKey[j] = bt[0];
                j++;
                createdKey[j] = bt[1];
                j++;
            }
            return createdKey;// возвращаю 8 ключей для 8-ми рандов
        }
        /// <summary>
        /// //Исключающее или для подблока и ключа
        /// </summary>
        /// <param name="subblock">Подблок 4Б</param>
        /// <param name="key">Ключ 2Б</param>
        /// <returns></returns>
        private byte[] RoundFunc(byte[] subblock, byte[] key)
        {
            byte[] result = new byte[4];
            byte temp = subblock[0];
            temp ^= key[0];
            result[0] = temp;

            temp = subblock[1];
            temp ^= key[0];
            result[1] = temp;

            temp = subblock[2];
            temp ^= key[1];
            result[2] = temp;

            temp = subblock[3];
            temp ^= key[1];
            result[3] = temp;

            return result;
        }
        /// <summary>
        ///  Метод шифровки алгоритмом "Сеть Фейстеля"
        /// </summary>
        /// <param name="block">Блок 16Б</param>
        /// <param name="key">Ряд ключей 16Б(каждый по 2Б)</param>
        /// <returns></returns>
        private byte[] CriptFeistel(byte[] block, byte[] key)
        {
            byte[] x1, x2, x3, x4, temp;
            byte[] result = new byte[16];

            //делим блок на 4 подблока так как ширина канала 32б = 4Б
            x1 = new byte[4];
            x2 = new byte[4];
            x3 = new byte[4];
            x4 = new byte[4];
            temp = new byte[4];

            //Заполняем подблоки данными из блока
            for (int i = 0; i < 4; i++)
            {
                x1[i] = block[i];
                x2[i] = block[i + 4];
                x3[i] = block[i + 8];
                x4[i] = block[i + 12];
            }

            byte[] roundkey = new byte[2];
            byte[] temp2;

            for (int i = 0, j = 0; i < 8; i++, j += 2)
            {
                //меняем значение ключа в каждом раунде
                roundkey[0] = key[j];
                roundkey[1] = key[j + 1];

                //Применяем Функцию к 1-му, 2-му и 3-му подблоку, после чего поочерёдно результаты через XOR применяем к 4-му подблоку и сдвигаем блоки влево(Type 2)
                temp = x1;
                temp2 = xor(RoundFunc(x3, roundkey), xor(RoundFunc(x2, roundkey), xor(RoundFunc(x1, roundkey), x4)));
                x1 = x2;
                x2 = x3;
                x3 = temp2;
                x4 = temp;
            }
            //Заполняем блок результатами 8-ми раундов
            for (int i = 0; i < 4; i++)
            {
                result[i] = x1[i];
                result[i + 4] = x2[i];
                result[i + 8] = x3[i];
                result[i + 12] = x4[i];
            }
            return result;//возвращаем зашифрованный блок
        }

        /// <summary>
        /// Обратное преобразование алгоритмом "Сеть Фейстеля"
        /// </summary>
        /// <param name="Bblock">Блок 4Б</param>
        /// <param name="key">Ключ 2Б (8 этапов)</param>
        /// <returns></returns>
        private byte[] DecriptFeistel(byte[] Bblock, byte[] key)
        { 
            byte[] x1, x2, x3, x4, temp, temp2;

            x1 = new byte[4];
            x2 = new byte[4];
            x3 = new byte[4];
            x4 = new byte[4];

            //Заполнем подблоки данными из блока
            for (int i = 0; i < 4; i++)
            {
                x1[i] = Bblock[i];
                x2[i] = Bblock[i + 4];
                x3[i] = Bblock[i + 8];
                x4[i] = Bblock[i + 12];
            }

            byte[] roundkey = new byte[2];

            //производим преобразования в обратном порядке
            for (int i = 7, j = 15; i >= 0; i--, j -= 2)
            {
                //Меняем ключи на каждом этапе
                roundkey[0] = key[j - 1];
                roundkey[1] = key[j];

                //Применяем Функцию к 2-му, 1-му и 4-му подблоку, после чего поочерёдно результаты через XOR применяем к 3-му подблоку и сдвигаем блоки вправо(Type 2)
                temp = x4;

                x3 = xor(RoundFunc(x2, roundkey), xor(RoundFunc(x1, roundkey), xor(RoundFunc(x4, roundkey), x3)));

                x4 = x3;
                x3 = x2;
                x2 = x1;
                x1 = temp;
            }
            //Заполняю блок дешифрованными даннми
            byte[] result = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                result[i] = x1[i];
                result[i + 4] = x2[i];
                result[i + 8] = x3[i];
                result[i + 12] = x4[i];
            }
            return result;//Возвращаю дешифрованный блок
        }
        /// <summary>
        /// Исключающее или блока 4Б
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private byte[] xor(byte[] a, byte[] b)
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = a[i];
                result[i] ^= b[i];
            }
            return result;
        }
        private void buttonDeCript2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBox1.Text))
            {
                if (IsCriptFeidelDid == true)
                {
                    IsCriptFeidelDid = false;//дешифрован

                    if (feistelKeys.Count != 0)
                    {
                        textBox1.Text = "";
                        Encoding enc = Encoding.UTF8;
                        List<byte[]> deCriptText = new List<byte[]>();
                        for (int i = 0; i < feistelKeys.Count; i++)
                        {
                            deCriptText.Add(DecriptFeistel(criptText[i], feistelKeys[i]));
                        }
                        int l = 0;
                        foreach (byte[] Block in deCriptText)
                        {
                            l += Block.Length;//узнаём длину текста
                        }
                        byte[] result = new byte[l - dopLength];//избавляемся от пустых байт
                        int j = 0;
                        foreach (byte[] Block in deCriptText)
                        {
                            foreach (byte bl in Block)
                            {
                                if (j < result.Length)
                                {
                                    result[j] = bl;
                                    j++;
                                }
                            }
                        }
                        string str = System.Text.Encoding.UTF8.GetString(result);
                        textBox1.Text += str.Substring(0, str.Length / 4);
                        textBox1.Text += str.Substring(str.Length / 4, str.Length / 4);
                        textBox1.Text += str.Substring(2 * str.Length / 4, str.Length / 4);
                        textBox1.Text += str.Substring(3 * str.Length / 4, str.Length / 4);
                  
                    }
                }
            }
        }
    }
}