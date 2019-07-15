using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quiziet
{
    public class Global
    {
        public static string currentUser = "";
        public static string salt = "";
        public static string cryptoKey = "mHwB7Pauls9kvGbfS";
        static Random randomGenerator = new Random();
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;

        public static byte[] GetHashFunc(string password, string Knownsalt = "") //Hashes input, can take no argument for when salt is not needed to be generated
        {
            if (Knownsalt == "")
            {
                //Generate new salt
                RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
                byte[] salt = new byte[16];
                provider.GetBytes(salt);
                Global.salt = Convert.ToBase64String(salt);
                
                // Generate and return the hash
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                return pbkdf2.GetBytes(36);
            }
            else
            {
                //Regenerate and return hash with known salt
                byte[] salt = Convert.FromBase64String(Knownsalt);
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                return pbkdf2.GetBytes(36);
            }
        }
        
        internal static string GenerateRandomString() //Simply generates a random string of characters by generating random bytes of data and translating that back to Base64 digits
        {
            byte[] randomBytes = new byte[4];
            randomGenerator.NextBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static void UploadTeacherFileReg(string teacherUserName) //Performs the creation of the FTP teacher directory when teacher user is created
        { 
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645; //Ensures program doesn't timeout
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            clientFTP.CreateDirectory($@"/Classes/{teacherUserName}");
            clientFTP.Disconnect();
        }

        public static void UploadStudentFileReg(string studentUserName) //Performs the creation of the FTP strudent directory when student user is created
        {
            FtpClient clientFTP = new FtpClient();
            clientFTP.Host = "ftp://82.10.84.171";
            clientFTP.Port = 54443;
            clientFTP.Credentials.UserName = "user";
            clientFTP.Credentials.Password = "qwerty";
            clientFTP.DataConnectionReadTimeout = 2147483645;//Ensures program doesn't timeout
            clientFTP.ConnectTimeout = 2147483645;
            clientFTP.DataConnectionConnectTimeout = 2147483645;
            clientFTP.ReadTimeout = 2147483645;
            clientFTP.Connect();
            clientFTP.CreateDirectory($@"/Students/{studentUserName}");
            clientFTP.Disconnect();
        }

        public static List<string[]> CSVToArray(string filePath) //Converts a csv from a file path to a list of string arrays, returning the list of string arrays
        {
            List<string[]> data = new List<string[]>();
            using (StreamReader file = new StreamReader(filePath))
            {
                while (!file.EndOfStream)//runs through whole csv file
                {
                    string line = file.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        data.Add(line.Split('|'));  //Uses | as a delimiter
                    }
                }
            }
            return data;            
        }

        public static void ArrayToCSV(List<string[]> array) //Converts a list of string arrayy to a csv file in the temporary directory of the client computer
        {           
            using (StreamWriter outfile = new StreamWriter(Path.GetTempPath() + "tempcsv.csv"))
            {
                for (int i = 0; i < array.Count; i++) //runs through all the string arrays
                {
                    string content = "";
                    for (int j = 0; j < array[i].Length; j++) //runs through each string in array
                    {
                        if (j != array[i].Length-1)
                        {
                            content += array[i][j].ToString() + "|";    //Uses | as a delimiter
                        }
                        else
                        {
                            content += array[i][j].ToString();
                        }
                    }
                    outfile.WriteLine(content);
                }
                outfile.Close();
            }            
        }
                
        //Encrypt
        public static string EncryptString(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector); //Translates each character of a string to UTF8 equivalent bytes, assigns the initialising vector bytes to an array of bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);  //Performs the same thing but on the plaintext
            PasswordDeriveBytes password = new PasswordDeriveBytes(Global.cryptoKey, null); //Takes the Global.cryptoKey and is passed for the initialisation of a 
                                                                                      //PasswordDeriveBytes object (from the Security.Cryptography library)
            byte[] keyBytes = password.GetBytes(keysize / 8); //Using the PasswordDeriveBytes object with the passed key, returns an array of bytes with a length of keysize/8 (256/8, so 32 bytes)
            RijndaelManaged symmetricKey = new RijndaelManaged(); //Initialises a RijndaelManaged object (from the Security.Cryptography library)
            symmetricKey.Mode = CipherMode.CBC; //Sets the "mode" of the symertric algorithm, being CBC - Cipher Block Chaining
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);//Takes the key and initVector (in their UTF8 byte form) and creates a Rijndael 
                                                                                                 //encryptor object, assigns this to a Cryptographic transformation
            MemoryStream memoryStream = new MemoryStream(); //Simply initialising a stream of data object
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);//Assigns a cryptoStream object the memoryStream as the target data stream, 
                                                                                                          //encryptor as the transformation to use, and the Write mode as the mode to use
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length); //Writes output of the transformation taking the painTextBytes as input, this is written to the memoryStream
            cryptoStream.FlushFinalBlock(); //Flushes any remaining redundant data
            byte[] cipherTextBytes = memoryStream.ToArray(); //Assigns the memoryStream data (now ciphered) to an array of bytes
            memoryStream.Close(); //Closes memoryStream
            cryptoStream.Close(); //Closes CryptoStream
            return Convert.ToBase64String(cipherTextBytes); //Translates the byte data to string data and returns appropriately
        }
        //Decrypt
        public static string DecryptString(string cipherText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector); //Translates each character of a string to UTF8 equivalent bytes, assigns the initialising vector bytes to an array of bytes
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText); //Converts the string data to byte data as an array of bytes, assigning while initialising cipherTextBytes
            PasswordDeriveBytes password = new PasswordDeriveBytes(Global.cryptoKey, null); //Takes the Global.cryptoKey and is passed for the initialisation of a 
                                                                                      //PasswordDeriveBytes object (from the Security.Cryptography library)
            byte[] keyBytes = password.GetBytes(keysize / 8); //Using the PasswordDeriveBytes object with the passed key, returns an array of bytes with a length of keysize/8 (256/8, so 32 bytes)
            RijndaelManaged symmetricKey = new RijndaelManaged(); //Initialises a RijndaelManaged object (from the Security.Cryptography library)
            symmetricKey.Mode = CipherMode.CBC; //Sets the "mode" of the symertric algorithm, being CBC - Cipher Block Chaining
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);//Takes the key and initVector (in their UTF8 byte form) and creates a Rijndael 
                                                                                                 //decryptor object, assigns this to a Cryptographic transformation
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes); //Simply initialising a stream of data object, passing the cipherTextBytes to ensure it has the same size as the byte array
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read); //Assigns a cryptoStream object the memoryStream as the target data stream, 
                                                                                                          //decryptor as the transformation to use, and the Read mode as the mode to use
            byte[] plainTextBytes = new byte[cipherTextBytes.Length]; //initialises an array of bytes with length equivalent to the cipherTextBytes array
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length); //Returns and thus assigns the size of the total nuymber of bytes read into the buffer, the decrypted 
                                                                                                  //bytes are now in the targeted bytes of plainTextBytes
            memoryStream.Close(); //Closes memoryStream
            cryptoStream.Close(); //Closes CryptoStream
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount); //translates the bytes to string, from the 0 index to the maximum size the of the total nuymber of bytes
        }

    }
}