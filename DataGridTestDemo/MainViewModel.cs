using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridTestDemo
{
    public partial class MainViewModel:ObservableObject
    {
        DataGrid _datagrid;
        /// <summary>
        /// 数据记录数
        /// </summary>
        [ObservableProperty]
        private int _totalPoint=100;
        /// <summary>
        /// 通道数
        /// </summary>
        [ObservableProperty]
        private int _totalChannel=64;
        /// <summary>
        /// 生成数据模板
        /// </summary>
        public FuncDataTemplate<PointModel> GenderDataTemplate { get; }
        /// <summary>
        /// 表格数据
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<PointModel> _DataGridModels=new ObservableCollection<PointModel>();
        /// <summary>
        /// TreeDataGrid数据
        /// </summary>
        [ObservableProperty]
        private FlatTreeDataGridSource<PointModel> _PointDatas;
        /// <summary>
        /// 测试数据
        /// </summary>
        private ObservableCollection<PointModel> TestDatas= new ObservableCollection<PointModel>(); 
        public MainViewModel(DataGrid datagrid)
        {
            _datagrid = datagrid;
            GenderDataTemplate = new FuncDataTemplate<PointModel>(
           (model) => model is not null,
           (model) => BuildGenderPresenter(model));
            Init();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Task.Run(() => 
            {
                for (global::System.Int32 j = 0; j < TotalPoint; j++)
                {
                    TestDatas.Add(new PointModel());
                    TestDatas[j].Values.Add(Random.Shared.Next(-180, 180));
                    TestDatas[j].Values.Add(Random.Shared.Next(-180, 180));
                    TestDatas[j].Values.Add(Random.Shared.Next(-180, 180));
                }
            });
        }
        public void GenderDataGridData()
        {
            _DatasCount = 0;
            DataGridModels.Clear();
            for (int i = 0; i < TotalPoint; i++)
            {
                DataGridModels.Add(new PointModel() {Index=i+1 });
            }
            // 创建序号列
            var indexColumn = new DataGridTextColumn
            {
                Width = DataGridLength.Auto,
                Header = "序号",
                IsReadOnly = true,
                Binding = new Binding("Index")
                {
                }
            };
            _datagrid.Columns.Add(indexColumn);
            for (int i = 0; i < TotalChannel; i++)
            {
                var custom = new DataGridTemplateColumn()
                {
                    Width = DataGridLength.SizeToHeader,
                    Header = CreateGrid($"通道{i+1}"),
                    IsReadOnly = true,
                    CellTemplate = GenderDataTemplate,
                };
                _datagrid.Columns.Add(custom);
                _DatasCount = +3;
                for (global::System.Int32 j = 0; j < TotalPoint; j++)
                {
                    
                    DataGridModels[j].Values.Add(TestDatas[j].Values[0]);
                    DataGridModels[j].Values.Add(TestDatas[j].Values[1]);
                    DataGridModels[j].Values.Add(TestDatas[j].Values[2]);
                }
            }
        }

        public void GenderTreeDataGridData()
        {
            DataGridModels.Clear();
            _DatasCount = 0;
            for (int i = 0; i < TotalPoint; i++)
            {
                DataGridModels.Add(new PointModel() { Index = i + 1 });
            }
            PointDatas = new FlatTreeDataGridSource<PointModel>(DataGridModels);
            PointDatas.Columns.Add(new TextColumn<PointModel, int>("序号", a => a.Index, (o, a) => o.Index = a,
                   GridLength.Auto));
            for (int i = 0; i < TotalChannel; i++)
            {
                PointDatas.Columns.Add(new TemplateColumn<PointModel>(CreateGrid($"通道{i + 1}"), GenderDataTemplate,null,
                    GridLength.Auto));
                _DatasCount = +3;
                for (global::System.Int32 j = 0; j < TotalPoint; j++)
                {

                    DataGridModels[j].Values.Add(TestDatas[j].Values[0]);
                    DataGridModels[j].Values.Add(TestDatas[j].Values[1]);
                    DataGridModels[j].Values.Add(TestDatas[j].Values[2]);
                }
            }
        }

        private Grid CreateGrid(string disPlayHeader)
        {
            Grid grid = new Grid()
            {
                MinWidth=300,
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 实收金额文本（第一行）
            var amountText = new TextBlock
            {
                Text = disPlayHeader,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(amountText);

            // 第二行的子 Grid
            var subGrid = new Grid();
            Grid.SetRow(subGrid, 1);

            subGrid.ColumnDefinitions.Add(new ColumnDefinition()); // 列 0
            subGrid.ColumnDefinitions.Add(new ColumnDefinition()); // 列 1
            subGrid.ColumnDefinitions.Add(new ColumnDefinition()); // 列 2
            var cashText = CreateCenteredText("X", 0);
            var electronicText = CreateCenteredText("Y", 1);
            var totalText = CreateCenteredText("Z", 2);

            subGrid.Children.Add(cashText);
            subGrid.Children.Add(electronicText);
            subGrid.Children.Add(totalText);

            // 添加分割线
            //subGrid.Children.Add(new GridSplitter
            //{
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    Width = 1,
            //});

            //subGrid.Children.Add(new GridSplitter
            //{
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    Width = 1,
            //    [Grid.ColumnProperty] = 1 // 设置第二列
            //});

            grid.Children.Add(subGrid);
            return grid;
        }
        private TextBlock CreateCenteredText(string text, int column)
        {
            return new TextBlock
            {
                Text = text,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                [Grid.ColumnProperty] = column
            };
        }
        volatile int _DatasCount;
        private Control BuildGenderPresenter(PointModel person)
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,*,*")
            };
            // 第一列的 TextBlock  
            var textBlock1 = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            textBlock1.Bind(TextBlock.TextProperty, new Binding($"Values[{_DatasCount}]")
            {
                FallbackValue = "绑定失败",
                TargetNullValue = "无数据"
            });
            Grid.SetColumn(textBlock1, 0);
            grid.Children.Add(textBlock1);

            // 第二列的 TextBlock  
            var textBlock2 = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            textBlock2.Bind(TextBlock.TextProperty, new Binding($"Values[{_DatasCount + 1}]")
            {
                FallbackValue = "绑定失败",
                TargetNullValue = "无数据"
            });
            Grid.SetColumn(textBlock2, 1);
            grid.Children.Add(textBlock2);

            // 第三列的 TextBlock  
            var textBlock3 = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            textBlock3.Bind(TextBlock.TextProperty, new Binding($"Values[{_DatasCount + 2}]")
            {
                FallbackValue = "绑定失败",
                TargetNullValue = "无数据"
            });
            Grid.SetColumn(textBlock3, 2);
            grid.Children.Add(textBlock3);

            // 确保返回值  
            return grid;
        }
    }
    public partial class PointModel : ObservableObject
    {

        [ObservableProperty]
        private int _Index;

        [ObservableProperty]
        private ObservableCollection<double> _Values=new ObservableCollection<double>();
    }
}
