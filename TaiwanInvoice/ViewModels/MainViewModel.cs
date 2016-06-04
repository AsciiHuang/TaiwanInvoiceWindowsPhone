using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;


namespace TaiwanInvoice
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public delegate void ViewActionHandler();
        public event ViewActionHandler LoadDataStarted;
        public event ViewActionHandler LoadDataCompleted;
        public event ViewActionHandler LoadDataError;

        public MainViewModel()
        {
            CurrentItems = new ObservableCollection<ItemViewModel>();
            PreviousItems = new ObservableCollection<ItemViewModel>();
            RemarkItems = new ObservableCollection<ItemViewModel>();
            InfoItems = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> CurrentItems { get; private set; }
        public ObservableCollection<ItemViewModel> PreviousItems { get; private set; }
        public ObservableCollection<ItemViewModel> RemarkItems { get; private set; }
        public ObservableCollection<ItemViewModel> InfoItems { get; private set; }

        public String UpdateTimeText
        {
            get
            {
                DateTime dt = UtilityHelper.GetContentUpdateTime();
                String text = String.Format("最近更新時間：{0}", dt.ToString("yyyy/MM/dd-HH:mm:ss", CultureInfo.InvariantCulture));
                return text;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            LoadData(false);
        }

        public void LoadData(Boolean force)
        {
            if (LoadDataStarted != null)
            {
                LoadDataStarted();
            }

            Boolean bGetReomoteData = false;

            if (force)
            {
                // 強制重取
                bGetReomoteData = true;
            }
            else
            {
                // 檢查時間是否超過 1 週
                DateTime preTime = UtilityHelper.GetContentUpdateTime();
                Double dblDiffHours = (DateTime.Now - preTime).TotalHours;
                if (dblDiffHours > (24 * 7))
                {
                    // 距上次已超過一週，重取資料，除非發生錯誤
                    bGetReomoteData = true;
                }
                else
                {
                    // 拿 local file 來顯示
                    String strCurrData = UtilityHelper.LoadIsoStorageData(Constants.CURRENT_INVOICE_DATA_FILE);
                    String strPreData = UtilityHelper.LoadIsoStorageData(Constants.PREVIOUS_INVOICE_DATA_FILE);
                    if ("".Equals(strCurrData) || "".Equals(strPreData))
                    {
                        // 沒資料，強制重取
                        bGetReomoteData = true;
                    }
                    else
                    {
                        // 有資料，直接到 OnCompleted 去做 parse
                        OnAPICurrDataAPILoadCompleted(strCurrData, false);
                        OnAPIPreDataAPILoadCompleted(strPreData);
                    }
                }
            }

            if (bGetReomoteData)
            {
                BaseAPI apiCurrData = new BaseAPI();
                apiCurrData.APILoadCompleted += new BaseAPI.APILoadHandler(OnAPICurrDataAPILoadCompleted);
                apiCurrData.GetStringResponse(Constants.CURRENT_INVOICE_URL);

                BaseAPI apiPreData = new BaseAPI();
                apiPreData.APILoadCompleted += new BaseAPI.APILoadHandler(OnAPIPreDataAPILoadCompleted);
                apiPreData.GetStringResponse(Constants.PREVIOUS_INVOICE_URL);
            }

            #region 填入獎項
            RemarkItems.Clear();
            RemarkItems.Add(new ItemViewModel() { Title = "特別獎 (1000萬元)", Description = "同期統一發票收執聯8位數號碼與特別獎中獎號碼相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "特獎 (200萬元)", Description = "同期統一發票收執聯8位數號碼與特獎中獎號碼相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "頭獎 (20萬元)", Description = "同期統一發票收執聯8位數號碼與頭獎中獎號碼相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "二獎 (4萬元)", Description = "同期統一發票收執聯末3位數號碼與頭獎中獎號碼末7位相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "三獎 (1萬元)", Description = "同期統一發票收執聯末3位數號碼與頭獎中獎號碼末6位相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "四獎 (4千元)", Description = "同期統一發票收執聯末3位數號碼與頭獎中獎號碼末5位相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "五獎 (1千元)", Description = "同期統一發票收執聯末3位數號碼與頭獎中獎號碼末4位相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "六獎 (2百元)", Description = "同期統一發票收執聯末3位數號碼與頭獎中獎號碼末3位相同者" });
            RemarkItems.Add(new ItemViewModel() { Title = "增開六獎 (2百元)", Description = "同期統一發票收執聯末3位數號碼與增開六獎號碼相同者" });
            #endregion

            #region 填入資訊項目
            InfoItems.Clear();
            InfoItems.Add(new ItemViewModel() { Icon = "reset.png", Title = "強制更新資訊" });
            InfoItems.Add(new ItemViewModel() { Icon = "goto.png", Title = "查看本期領獎注意事項" });
            InfoItems.Add(new ItemViewModel() { Icon = "goto.png", Title = "財政部網頁-本期號碼" });
            InfoItems.Add(new ItemViewModel() { Icon = "goto.png", Title = "財政部網頁-上期號碼" });
            InfoItems.Add(new ItemViewModel() { Icon = "tel.png", Title = "財政部統一發票服務專線" });
            #endregion

            IsDataLoaded = true;
        }

        private void OnAPICurrDataAPILoadCompleted(String result)
        {
            OnAPICurrDataAPILoadCompleted(result, true);
        }

        private void OnAPICurrDataAPILoadCompleted(String result, Boolean remote)
        {
            Boolean useLocalFile = false;
            Boolean bError = false;
            if ("".Equals(result))
            {
                // 發生錯誤
                useLocalFile = true;
            }
            else
            {
                List<ItemViewModel> list = InvoiceParser.Parse(result);
                if (list.Count > 0)
                {
                    UtilityHelper.SaveIsoStorageData(Constants.CURRENT_INVOICE_DATA_FILE, result);
                    DateTime now = DateTime.Now;
                    if (remote)
                    {
                        UtilityHelper.SetContentUpdateTime();
                    }
                    NotifyPropertyChanged("UpdateTimeText");
                    UpdateItemList(list, CurrentItems);
                }
                else
                {
                    // 發生錯誤
                    useLocalFile = true;
                }
            }

            if (useLocalFile)
            {
                String strCurrData = UtilityHelper.LoadIsoStorageData(Constants.CURRENT_INVOICE_DATA_FILE);
                if ("".Equals(strCurrData))
                {
                    bError = true;
                }
                else
                {
                    List<ItemViewModel> list = InvoiceParser.Parse(strCurrData);
                    if (list.Count > 0)
                    {
                        UpdateItemList(list, CurrentItems);
                    }
                }
            }

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted();
            }

            if (bError)
            {
                if (LoadDataError != null)
                {
                    LoadDataError();
                }
            }
        }

        private void OnAPIPreDataAPILoadCompleted(String result)
        {
            Boolean useLocalFile = false;
            if ("".Equals(result))
            {
                // 發生錯誤
                useLocalFile = true;
            }
            else
            {
                List<ItemViewModel> list = InvoiceParser.Parse(result);
                if (list.Count > 0)
                {
                    UtilityHelper.SaveIsoStorageData(Constants.PREVIOUS_INVOICE_DATA_FILE, result);
                    UpdateItemList(list, PreviousItems);
                }
                else
                {
                    // 發生錯誤
                    useLocalFile = true;
                }
            }

            if (useLocalFile)
            {
                String strCurrData = UtilityHelper.LoadIsoStorageData(Constants.PREVIOUS_INVOICE_DATA_FILE);
                if (!"".Equals(strCurrData))
                {
                    List<ItemViewModel> list = InvoiceParser.Parse(strCurrData);
                    if (list.Count > 0)
                    {
                        UpdateItemList(list, PreviousItems);
                    }
                }
            }
        }

        private void UpdateItemList(List<ItemViewModel> list, ObservableCollection<ItemViewModel> viewList)
        {
            viewList.Clear();
            foreach (ItemViewModel item in list)
            {
                viewList.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}