using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Raylogic_Firmworks_Encoder
{
    public partial class Form1 : Form
    {
        // Variables
        int pass_count;
        byte limit_count;
        string temp_fil = "";
        bool got_file;
        // Classes

        public Form1()
        {
            InitializeComponent();
            pass_count = 0;
            disable_controls();
            limit_count = 0;
            got_file = false;
        }
        
        private void enable_controls()
        {
            groupBox2.Enabled = true;
        }

        private void disable_controls()
        {
            groupBox2.Enabled = false;
        }

        private void button1_Click (object sender, EventArgs e)
        {
            if((textBox1.Text == "admin") && (textBox2.Text == "ad"))
            {
                enable_controls();
                groupBox1.Enabled = false;
            }
            else
            {
                pass_count++;
                if(pass_count == 4)
                {
                    MessageBox.Show("Maximum Number of Password tries exceeded !");
                    System.Environment.Exit(1);
                }
                disable_controls();
            }
        }

        private void button2_Click (object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Hex files (*.hex)|*.hex|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";        
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
                got_file = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.FileName != "")
            {                
                if (comboBox1.SelectedIndex != -1)
                {
                    if (got_file == true)
                    {
                        byte[] cx = EncryptFile(openFileDialog1.FileName);
                        save_file(cx);
                        textBox4.Text = temp_fil;
                        got_file = false;
                    }
                    else
                    {
                        MessageBox.Show("Please choose a valid file first !!");
                    }
                }
                else
                {
                    MessageBox.Show("Please choose a traget Device !!");
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Fpa files (*.fpa)|*.fpa|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }            
            byte[] fx = DecryptFile(openFileDialog1.FileName);
        }

        public byte[] EncryptFile(string filnam)
        {            
            string password = "abcd1234";
            byte[] bytesToBeEncrypted = File.ReadAllBytes(filnam);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = Crypto.AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            return bytesEncrypted;
        }

        public byte[] DecryptFile(string filnam)
        {            
            string password = "abcd1234";
            byte[] allBytes = File.ReadAllBytes(filnam);
            string st = Encoding.UTF8.GetString(allBytes, 0, 20);
            limit_count = allBytes[20];

            byte[] bytesToBeDecrypted;
            bytesToBeDecrypted = new byte[allBytes.Length - 21];
            Buffer.BlockCopy(allBytes, 21, bytesToBeDecrypted, 0, allBytes.Length - 21);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesDecrypted = Crypto.AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            return bytesDecrypted;           
        }

        private void save_file(byte[] cx)
        {
            byte limit_count;

           byte[] hx = Encoding.UTF8.GetBytes("Raylogic FPA Project");
           byte[] px = Encoding.UTF8.GetBytes(comboBox1.SelectedItem.ToString());
            
            if (checkBox1.Checked == false)
            {
                limit_count = 0;
            }
            else
            {
                limit_count = (byte)numericUpDown1.Value;
            }

            byte fx = limit_count;
            int a = cx.Count();
            int b = hx.Count();           
            byte[] final = new byte[a + b + 1];
            int z = 0;
            for (int i = 0; i < b; i++)
            {
                final[z++] = hx[i];
            }
            final[z++] = fx;
            for (int i=0;i<a;i++)
            {
                final[z++] = cx[i];
            }          
            
            string dir = Path.GetDirectoryName(openFileDialog1.FileName);
            string fil = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);           
            temp_fil = dir + "\\" + fil + ".fpa";
            File.WriteAllBytes(temp_fil , final);            
        }       
    }
    public class Crypto
    {
        //private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");
        private static byte[] _salt = Encoding.ASCII.GetBytes("inXx#s*^%#HGX_opa?j");
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        /// 
        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }        
    }
}
