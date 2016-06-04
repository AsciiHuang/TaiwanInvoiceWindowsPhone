using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;

namespace TaiwanInvoice
{
    public class UtilityHelper
    {
        public static void SaveIsoStorageData(String fileName, String fieData)
        {
            try
            {
                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoFile.FileExists(fileName))
                {
                    isoFile.DeleteFile(fileName);
                }
                IsolatedStorageFileStream file = isoFile.CreateFile(fileName);
                if (!"".Equals(fieData))
                {
                    Byte[] byteArray = Encoding.UTF8.GetBytes(fieData);
                    file.Write(byteArray, 0, byteArray.Length);
                }
                file.Close();
                file.Dispose();
                isoFile.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static String LoadIsoStorageData(String fileName)
        {
            String strRes = "";
            try
            {
                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoFile.FileExists(fileName))
                {
                    IsolatedStorageFileStream fStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoFile);
                    if (fStream != null && fStream.Length > 0)
                    {
                        Byte[] btReadBuf = new Byte[(int)fStream.Length];
                        btReadBuf.Initialize();
                        int nCurrentRead = fStream.Read(btReadBuf, 0, btReadBuf.Length);
                        if (nCurrentRead == (int)fStream.Length)
                        {
                            // 正常讀丸
                            strRes = Encoding.UTF8.GetString(btReadBuf, 0, btReadBuf.Length);
                        }
                    }
                    fStream.Close();
                    fStream.Dispose();
                }
                isoFile.Dispose();
            }
            catch (Exception)
            {
                strRes = "";
            }
            return strRes;
        }

        public static DateTime GetContentUpdateTime()
        {
            DateTime dtRes = new DateTime(0);
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.SETTING_PREVIOUS_CONTENT_UPDATE_TIME))
            {
                dtRes = (DateTime)IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_PREVIOUS_CONTENT_UPDATE_TIME];
            }
            return dtRes;
        }

        public static void SetContentUpdateTime()
        {
            IsolatedStorageSettings.ApplicationSettings[Constants.SETTING_PREVIOUS_CONTENT_UPDATE_TIME] = DateTime.Now;
            try
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch (Exception)
            {
            }
        }
    }
}
