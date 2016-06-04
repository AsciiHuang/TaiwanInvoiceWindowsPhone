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

namespace TaiwanInvoice
{
    public class Constants
    {
        public const String CURRENT_INVOICE_URL = "http://invoice.etax.nat.gov.tw/etaxinfo_1.htm"; // 當期發票
        public const String PREVIOUS_INVOICE_URL = "http://invoice.etax.nat.gov.tw/etaxinfo_2.htm"; // 上期發票

        public const String CURRENT_INVOICE_DATA_FILE = "TWInvoiceCurrentData"; // 當期備份
        public const String PREVIOUS_INVOICE_DATA_FILE = "TWInvoicePreviousData"; // 上期備份

        public const String SETTING_PREVIOUS_CONTENT_UPDATE_TIME = "TWInvoiceContentUpdateTime"; // 上次取資料的時間，超過一週才試著重取
    }
}
