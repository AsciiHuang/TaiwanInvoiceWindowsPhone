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
using System.Collections.Generic;
using System.Globalization;

namespace TaiwanInvoice
{
    public class InvoiceParser
    {
        public InvoiceParser()
        {
        }

        public static List<ItemViewModel> Parse(String invoice)
        {
            List<ItemViewModel> listRes = new List<ItemViewModel>();

            try
            {
                int mon = invoice.IndexOf("月統一發票中獎號碼單", 0);
                int isno = 0;
                int Seek = 0;
                String strMon = invoice.Substring(mon - 8, 17).Trim();
                if (strMon != null)
                {
                    strMon = strMon.Trim(new char[] { '<', '>' });
                    strMon = strMon.Replace("統一發票", "");
                    if (!"".Equals(strMon))
                    {
                        listRes.Add(new ItemViewModel() { Title = "發票中獎號碼資訊", Description = strMon });
                    }
                }

                double dubleVal = 0;
                int nSpecialIdx = invoice.IndexOf("特別", 0);
                if (nSpecialIdx > 0)
                {
                    // 特別獎不一定每次都有…需特別判斷處理
                    for (Seek = nSpecialIdx; Seek < invoice.Length; Seek++)
                    {
                        if (double.TryParse(invoice.Substring(Seek, 1), NumberStyles.Integer, null, out dubleVal))
                        {
                            isno++;
                            if (isno == 8)
                            {
                                break;
                            }
                        }
                        else
                        {
                            isno = 0;
                        }
                    }
                    String strSpecialNumber = invoice.Substring(Seek - 7, 8);
                    if (strSpecialNumber != null && !"".Equals(strSpecialNumber))
                    {
                        listRes.Add(new ItemViewModel() { Title = "特別獎", Description = strSpecialNumber });
                    }
                }
                else
                {
                    listRes.Add(new ItemViewModel() { Title = "特別獎", Description = "本月無加開特別獎" });
                }

                int nSuperIdx = invoice.IndexOf("特獎", 0);
                for (Seek = nSuperIdx; Seek < invoice.Length; Seek++)
                {
                    if (double.TryParse(invoice.Substring(Seek, 1), NumberStyles.Integer, null, out dubleVal))
                    {
                        isno++;
                        if (isno == 8)
                        {
                            break;
                        }
                    }
                    else
                    {
                        isno = 0;
                    }
                }
                String strSuperNumber = invoice.Substring(Seek - 7, 8);
                if (strSuperNumber != null && !"".Equals(strSuperNumber))
                {
                    listRes.Add(new ItemViewModel() { Title = "特獎", Description = strSuperNumber });
                }

                int nBigIdx = invoice.IndexOf("頭獎", 0);
                string[] aStrBigNumber = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    for (Seek = nBigIdx; Seek < invoice.Length; Seek++)
                    {
                        if (double.TryParse(invoice.Substring(Seek, 1), NumberStyles.Integer, null, out dubleVal))
                        {
                            isno++;
                            if (isno == 8)
                            {
                                break;
                            }
                        }
                        else
                        {
                            isno = 0;
                        }
                    }
                    aStrBigNumber[i] = invoice.Substring(Seek - 7, 8);
                    nBigIdx = Seek;
                }
                if (aStrBigNumber[0] != null && !"".Equals(aStrBigNumber[0]))
                {
                    String description = String.Format("{0}\n{1}\n{2}", aStrBigNumber[0], aStrBigNumber[1], aStrBigNumber[2]);
                    listRes.Add(new ItemViewModel() { Title = "頭獎", Description = description });
                }

                int nExIdx = invoice.IndexOf("增開", 0);
                string[] aStrExNumber = new string[2];
                aStrExNumber[0] = "";
                aStrExNumber[1] = "";
                if (nExIdx > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (Seek = nExIdx; Seek < invoice.Length; Seek++)
                        {
                            if (double.TryParse(invoice.Substring(Seek, 1), NumberStyles.Integer, null, out dubleVal))
                            {
                                isno++;
                                if (isno == 3)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                isno = 0;
                            }
                        }

                        if (isno == 3)
                        {
                            aStrExNumber[i] = invoice.Substring(Seek - 2, 3);
                            nExIdx = Seek;
                        }
                    }
                }
                if (aStrExNumber[0] != null && !"".Equals(aStrExNumber[0]))
                {
                    String description = String.Format("{0}\n{1}", aStrExNumber[0], aStrExNumber[1]);
                    listRes.Add(new ItemViewModel() { Title = "增開六獎", Description = description });
                }
            }
            catch (Exception)
            {
            }

            return listRes;
        }
    }
}
