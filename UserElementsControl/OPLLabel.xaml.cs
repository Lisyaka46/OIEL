using IEL.CORE.Enums;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OIEL.UserElementsControl.Base.LabelBase;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Binding = System.Windows.Data.Binding;

namespace OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabel.xaml
    /// </summary>
    public partial class OPLLabelAction : IELObjectBase, IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Данные изображения объекта
        /// </summary>
        public ImageSource ImageSource
        {
            get => ImageElementLabel.Source;
            set => ImageElementLabel.Source = value;
        }

        /// <summary>
        /// Данные изображения выделения объекта
        /// </summary>
        public ImageSource ImageSelectSource
        {
            get => ImageSelect.Source;
            set => ImageSelect.Source = value;
        }

        #region Properties

        #region SourceLabel
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceLabelProperty =
            DependencyProperty.Register("SourceLabel", typeof(LabelAction), typeof(OPLLabelAction),
                new(
                    (sender, e) =>
                    {
                       // ((OPLLabelAction)sender).SourceLabel = (LabelAction)e.NewValue;
                    }));

        /// <summary>
        /// Элемент ярлыка
        /// </summary>
        public LabelAction SourceLabel
        {
            get => (LabelAction)GetValue(SourceLabelProperty);
            set => SetValue(SourceLabelProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Состояние выделенного элемента
        /// </summary>
        public bool Selected { get; private set; }

        public OPLLabelAction(LabelAction Label)
        {
            InitializeComponent();
            ImageFaviconLabel.Width = 20;
            ImageFaviconLabel.Height = 20;
            ImageFaviconLabel.Opacity = 0d;

            #region Background
            BorderSelectElement.Background = SourceBackground.SourceBrush;
            BorderMain.Background = SourceBackground.SourceBrush;
            #endregion

            #region BorderBrush
            BorderMain.BorderBrush = SourceBorderBrush.SourceBrush;
            #endregion

            #region Foreground
            TextBlockNameLabel.Foreground = SourceForeground.SourceBrush;
            TextBlockNumberSelect.Foreground = SourceForeground.SourceBrush;
            #endregion

            MouseEnter += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };
            MouseLeave += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Default, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Default, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Default, true);
            };
            MouseDown += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Used, false);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Used, false);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Used, false);
            };
            MouseLeftButtonUp += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };
            MouseRightButtonUp += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };

            Selected = false;
            BorderSelectElement.Opacity = 0d;
            SourceLabel = Label;
            SourceLabel.SetTag += (Old, New) =>
            {
            };
            SourceLabel.DeleteTag += (Old, New) =>
            {
            };
            UpdateLayout();
        }

        /// <summary>
        /// Обновление отображения данных
        /// </summary>
        public new void UpdateLayout()
        {
            TextBlockNameLabel.Text = SourceLabel.Name;
            base.UpdateLayout();
        }

        /// <summary>
        /// Включить состояние выделения ярлыка
        /// </summary>
        /// <param name="index">Отображаемый индекс</param>
        public void SelectOn()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            else
                BorderSelectElement.Opacity = 1d;
            Selected = true;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Включить состояние выделения ярлыка
        /// </summary>
        /// <param name="ListSource">Массив куда записывается выделяемый ярлык</param>
        public void SelectOn(ref List<OPLLabelAction> ListSource)
        {
            ListSource.Add(this);
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            else
                BorderSelectElement.Opacity = 1d;
            Selected = true;
            ImageSelect.Margin = new(0, 0, 0, 10);
            TextBlockNumberSelect.Text = ListSource.Count.ToString();
        }

        /// <summary>
        /// Выключить состояние выделения ярлыка
        /// </summary>
        public void SelectOff()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 0d, TimeSpan.FromMilliseconds(200d));
            else
                BorderSelectElement.Opacity = 0d;
            Selected = false;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Выключить состояние выделения ярлыка
        /// </summary>
        /// <param name="ListSource">Массив куда записывается выделяемый ярлык</param>
        public void SelectOff(ref List<OPLLabelAction> ListSource)
        {
            ListSource.Remove(this);
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 0d, TimeSpan.FromMilliseconds(200d));
            else
                BorderSelectElement.Opacity = 0d;
            Selected = false;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Отобразить индекс выделения объекта
        /// </summary>
        /// <param name="i"></param>
        public void SetIndexVisual(int i)
        {
            TextBlockNumberSelect.Text = i.ToString();
        }

        /// <summary>
        /// Установить иконку сайта на ярлык
        /// </summary>
        /// <param name="SourceUriIcon">Ссылка на домен иконки</param>
        public async Task SetFaviconIcon(Uri? SourceUriIcon)
        {
            if (SourceUriIcon == null)
            {
                ImageFaviconLabel.Source = null;
                return;
            }

            ImageFaviconLabel.Source = await DownloadFavicon(SourceUriIcon);

            if (ManagerAnimation != null)
            {
                ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(900d));
                ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, WidthProperty, 20d, 40d, TimeSpan.FromMilliseconds(1100d));
                ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, HeightProperty, 20d, 40d, TimeSpan.FromMilliseconds(1100d));
            }
            else
            {
                ImageFaviconLabel.Width = 40d;
                ImageFaviconLabel.Height = 40d;
                ImageFaviconLabel.Opacity = 1d;
            }
        }

        /// <summary>
        /// Установка иконки хоста сайта через собственный клиент
        /// </summary>
        /// <param name="url">Ссылка хоста: Сама преобразуется в управляемый DNS сервер хоста</param>
        /// <returns>Картинка которая ссылается на иконку управляемого сайта</returns>
        private static async Task<BitmapImage> DownloadFavicon(Uri url)
        {
            string faviconurl = "http://" + url.DnsSafeHost + "/favicon.ico";
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = await new HttpClient().GetStreamAsync(faviconurl);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        ///// <summary>
        ///// Создать визуальный элемент тега
        ///// </summary>
        ///// <param name="value">Значение отображаемое тега</param>
        ///// <returns></returns>
        //internal static OPLLabelTag CreateVisualTag(LabelTag NewTag)
        //{
        //    return new()
        //    {
        //        BorderThickness = new(1),
        //        Text = string.Empty,
        //        Padding = new(4, 2, 4, 2),
        //        FontSize = 16d,
        //        Tag = NewTag,
        //    };
        //}
    }
}
