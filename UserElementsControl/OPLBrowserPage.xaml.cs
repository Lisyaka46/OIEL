using IEL.UserElementsControl.Base;
using OIEL.CORE.Browser;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;

namespace OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLBrowserPage.xaml
    /// </summary>
    public partial class OPLBrowserPage : System.Windows.Controls.UserControl, IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Массив объектов страниц
        /// </summary>
        private readonly List<OPLInlay> SourceInlays;

        /// <summary>
        /// Массив активных вкладок браузера
        /// </summary>
        public ReadOnlyCollection<OPLInlay> Inlays => SourceInlays.AsReadOnly();

        /// <summary>
        /// Количество вкладок в браузере страниц
        /// </summary>
        public int InlaysCount => SourceInlays.Count;

        private int _ActivateIndex = -1;
        /// <summary>
        /// Индекс активированной вкладки
        /// </summary>
        public int ActivateIndex => _ActivateIndex;

        /// <summary>
        /// Активная вкладка в браузере
        /// </summary>
        public OPLInlay? ActualInlay => ActivateIndex > -1 ? SourceInlays[ActivateIndex] : null;

        /// <summary>
        /// Делегат события без параметров
        /// </summary>
        public delegate void DelegateVoidHandler();

        /// <summary>
        /// Делегат события взаимодействия с описанием вкладки
        /// </summary>
        public delegate void DelegateDescriptionInlayHandler(FrameworkElement Element, string? Text);

        /// <summary>
        /// Делегат события активации действий над выбранной вкладкой
        /// </summary>
        public delegate void ActiveActionInInlay(OPLInlay Inlay);

        /// <summary>
        /// Событие закрытия последней вкладки браузера
        /// </summary>
        public event DelegateVoidHandler? EventCloseBrowser;

        /// <summary>
        /// Событие изменения активной вкладки
        /// </summary>
        public event DelegateVoidHandler? EventChangeActiveInlay;

        /// <summary>
        /// Событие закрытия вкладки
        /// </summary>
        public event DelegateVoidHandler? EventCloseInlay;

        /// <summary>
        /// Событие Добавления новой вкладки
        /// </summary>
        public event DelegateVoidHandler? EventAddInlay;

        /// <summary>
        /// Событие открытия действий над выбранной вкладкой
        /// </summary>
        public event ActiveActionInInlay? EventActiveActionInInlay;

        #region Properties

        #region Background
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(OPLBrowserPage),
                new(
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).BorderMain.Background = (Brush)e.NewValue;
                    }));

        /// <summary>
        /// Объект фона
        /// </summary>
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        #endregion

        #region BorderBrush
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(OPLBrowserPage),
                new(
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).BorderMain.BorderBrush = (Brush)e.NewValue;
                        ((OPLBrowserPage)sender).BorderMainPage.BorderBrush = (Brush)e.NewValue;
                        ((OPLBrowserPage)sender).BorderInlays.BorderBrush = (Brush)e.NewValue;
                    }));

        /// <summary>
        /// Цвет отображения границ элемента
        /// </summary>
        public new Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        #endregion

        #region FontSize
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(OPLBrowserPage),
                new(12d,
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).TextBlockNullPage.FontSize = (double)e.NewValue;
                    }));

        /// <summary>
        /// Размер текста в элементе
        /// </summary>
        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        #endregion

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(OPLBrowserPage),
                new(new FontFamily("Calibri"),
                    (sender, e) =>
                    {
                        ((OPLBrowserPage)sender).TextBlockNullPage.FontFamily = (FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт текста используемый в элементе
        /// </summary>
        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект представления стековых объектов - вкладок
        /// </summary>
        private StackPanel StackPanelInlays;

        /// <summary>
        /// Инициализировать объект интерфейса отображения страничных объектов
        /// </summary>
        public OPLBrowserPage()
        {
            InitializeComponent();
            TextBlockNullPage.Opacity = 0.4d;
            SourceInlays = [];
            StackPanelInlays = new()
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            };
            IELScrollViewerInlays.AutoUpdateVisibleHorizontalScroll = true;
            IELScrollViewerInlays.AutoUpdateVisibleVerticalScroll = false;
            IELScrollViewerInlays.Content = StackPanelInlays;

            IELButtonAddInlay.OnActivateMouseLeft += (sender, e) => EventAddInlay?.Invoke();
        }

        #region IELButtonAddInlay
        /// <summary>
        /// Установить картинку для кнопки добавления вкладки
        /// </summary>
        /// <param name="Source"></param>
        public void SetSourceImageButtonAddInlay(ImageSource Source) => IELButtonAddInlay.Source = Source;

        /// <summary>
        /// Получить объект кнопки добавления вкладки
        /// </summary>
        public IELObjectBase GetButtonAddInlay() => IELButtonAddInlay;
        #endregion

        #region ManipulateInlayAdd
        /// <summary>
        /// Добавить новую страницу
        /// </summary>
        /// <param name="Content">Добавляемая страница в баузер страниц</param>
        /// <param name="Activate">Активировать сразу или нет страницу</param>
        public OPLInlay AddInlayPage(PageBrowser Content, bool Activate = true)
        {
            OPLInlay InlaySource = CreateInlay(Content);
            InlaySource.MouseRightButtonUp += (sender, e) => EventActiveActionInInlay?.Invoke(InlaySource);

            SourceInlays.Add(InlaySource);
            StackPanelInlays.Children.Add(InlaySource);
            InlaySource.UpdateLayout();

            if (ManagerAnimation != null)
            {
                InlaySource.Width = 0d;
                DoubleAnimation animation = ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(800d);
                animation.To = 0d;
                TextBlockNullPage.BeginAnimation(OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
                animation.Duration = TimeSpan.FromMilliseconds(400d);
                animation.To = 1d;
                InlaySource.BeginAnimation(OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
                ManagerAnimation.DoubleAnimationType.AnimateEffect(InlaySource, WidthProperty,
                    InlaySource.ActualWidth + InlaySource.Margin.Left + InlaySource.Margin.Right, TimeSpan.FromMilliseconds(350d));
            }
            else
            {
                TextBlockNullPage.Opacity = 0d;
                InlaySource.Opacity = 1d;
            }
            //SourceDoubleAnimation.To = InlaySource.ActualWidth;
            //InlaySource.BeginAnimation(WidthProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);

            if (Activate) ActivateInlayIndex(SourceInlays.Count - 1);
            return InlaySource;
        }

        /// <summary>
        /// Создать вкладку в браузере
        /// </summary>
        /// <param name="Content">Страница ссылки</param>
        /// <returns>Созданная вкладка</returns>
        private OPLInlay CreateInlay(PageBrowser Content)
        {
            OPLInlay Inlay = new()
            {
                Text = Content.Title,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                BorderThickness = new(2),
                CornerRadius = new(8, 8, 0, 0),
                Margin = new(1, 0, 1, 0),
                Opacity = 0d,
                Content = Content,
            };
            Inlay.OnActivateCloseInlay += (sender, e) =>
            {
                //if (ScrollMouseUse) return;
                Inlay.IsEnabled = false;
                DeleteInlayPage(Inlay, ActivateIndex == SourceInlays.IndexOf(Inlay));
                EventCloseInlay?.Invoke();
            };
            Inlay.MouseLeftButtonUp += (sender, e) =>
            {
                //if (ScrollMouseUse)
                //{
                //    ScrollMouseUse = false;
                //    return;
                //}
                ActivateInlayInBrowserPage(Inlay.Content);
            };
            //Inlay.MouseHover += (sender, e) =>
            //{
            //    if (Inlay.PageElement == null) return;
            //    else if (Inlay.PageElement?.Description.Length == 0) return;
            //    EventOnDescriptionInlay?.Invoke(Inlay, Inlay.PageElement?.Description);
            //};
            //Inlay.MouseLeave += (sender, e) =>
            //{
            //    if (Inlay.SourceBrowsePage?.Description.Length == 0) return;
            //    EventOffDescriptionInlay?.Invoke();
            //};
            //Inlay.MouseDown += (sender, e) =>
            //{
            //    if (Inlay.SourceBrowsePage?.Description.Length == 0) return;
            //    EventOffDescriptionInlay?.Invoke();
            //};
            return Inlay;
        }
        #endregion

        #region ManipulateInlay
        /// <summary>
        /// Открыть страницу по индексу
        /// </summary>
        /// <param name="index">Индекс открываемой страницы</param>
        /// <exception cref="Exception">Исключение при пустой странице в найденой вкладке</exception>
        public void ActivateInlayIndex(Index index)
        {
            if (index.Value == ActivateIndex && SourceInlays[index].SourceBackground.GetUsedState()) return;
            PageBrowser Page = SourceInlays[index].Content ?? throw new Exception("Объект заголовка не может быть без страницы!");
            //SourceDoubleAnimation.Duration = TimeSpan.FromMilliseconds(300d);
            if (ActivateIndex > -1 && SourceInlays.Count > ActivateIndex)
            {
                OPLInlay BackInlay = SourceInlays[ActivateIndex];
                BackInlay.SourceBackground.SetUsedState(false);
                //SourceDoubleAnimation.To = 45d;
                //BackInlay.BeginAnimation(HeightProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }
            OPLInlay NextInlay = SourceInlays[index];
            //SourceDoubleAnimation.To = 50d;
            //NextInlay.BeginAnimation(HeightProperty, SourceDoubleAnimation, HandoffBehavior.SnapshotAndReplace);
            NextInlay.SourceBackground.SetUsedState(true);
            MainPageController.NextPage(Page, index.Value >= ActivateIndex);
            _ActivateIndex = index.Value;
            //Page.EventFocusPage?.Invoke(Page);
            EventChangeActiveInlay?.Invoke();
        }

        /// <summary>
        /// Открыть страницу по элементу
        /// </summary>
        /// <param name="Page">Открываемая вкладка страницы</param>
        /// <exception cref="Exception">Исключение при пустой странице в найденой вкладке</exception>
        public void ActivateInlayInBrowserPage(PageBrowser Page)
        {
            try
            {
                PageBrowser?[] Pages = [.. SourceInlays.Select((i) => i.Content)];
                ActivateInlayIndex(Array.IndexOf(Pages, Page));
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// Сделать поиск страниц по типу
        /// </summary>
        /// <typeparam name="T">Тип страницы поиска</typeparam>
        /// <returns>Найденные страницы</returns>
        public T?[]? SearchAllPageType<T>() where T : PageBrowser
        {
            if (InlaysCount == 0) return null;
            List<T?> values = [];
            foreach (OPLInlay Inlay in SourceInlays)
            {
                if (Inlay.Content?.GetType() == typeof(T)) values.Add((T?)Inlay.Content);
            }
            return values.Count == 0 ? null : [.. values];
        }

        /// <summary>
        /// Сделать поиск страницы по типу
        /// </summary>
        /// <typeparam name="T">Тип страницы поиска</typeparam>
        /// <returns>Найденная страница</returns>
        public T? SearchAnyPageType<T>() where T : PageBrowser
        {
            if (InlaysCount == 0) return null;
            foreach (OPLInlay Inlay in SourceInlays)
            {
                if (Inlay.Content?.GetType() == typeof(T)) return (T?)Inlay.Content;
            }
            return null;
        }

        /// <summary>
        /// Удалить вкладку в браузере
        /// </summary>
        /// <param name="inlay">Объект вкладки</param>
        /// <param name="ActivateNextInlay">Активировать ли следующую после удалённой вкладки вкладку</param>
        public void DeleteInlayPage(OPLInlay inlay, bool ActivateNextInlay = true)
        {
            if (SourceInlays.IndexOf(inlay) is int Index && Index == -1) return;
            int IndexNext = NextIndex(Index, InlaysCount - 1);
            OPLInlay ActualInlay = SourceInlays[Index];
            ActualInlay.Content?.Dispose();

            if (ManagerAnimation != null)
            {
                ManagerAnimation.DoubleAnimationType.AnimateEffect(ActualInlay, WidthProperty, 0d, TimeSpan.FromMilliseconds(350d));
                ManagerAnimation.ThicknessAnimationType.AnimateEffect(ActualInlay, MarginProperty, new(0), TimeSpan.FromMilliseconds(350d));
                DoubleAnimation animationDouble = ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
                animationDouble.Duration = TimeSpan.FromMilliseconds(400d);
                animationDouble.To = 0d;
                animationDouble.FillBehavior = FillBehavior.Stop;
                animationDouble.Completed += (sender, e) =>
                {
                    ActualInlay.Opacity = 0d;
                    StackPanelInlays.Children.Remove(ActualInlay);
                };
                ActualInlay.BeginAnimation(OpacityProperty, animationDouble);
            }
            else
            {
                ActualInlay.Opacity = 0d;
                StackPanelInlays.Children.Remove(ActualInlay);
            }
            SourceInlays.RemoveAt(Index);

            if (ActivateNextInlay)
            {
                if (IndexNext == -1)
                {
                    _ActivateIndex = -1;
                    MainPageController.ClosePage();
                    EventCloseBrowser?.Invoke();
                }
                else
                {
                    ActivateInlayIndex(IndexNext);
                }
            }
            else if (ActivateIndex >= Index) _ActivateIndex--;
            if (InlaysCount == 0)
            {
                if (ManagerAnimation != null)
                {
                    DoubleAnimation animationDouble = ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
                    animationDouble.Duration = TimeSpan.FromMilliseconds(800d);
                    animationDouble.To = 0.4d;
                    TextBlockNullPage.BeginAnimation(OpacityProperty, animationDouble, HandoffBehavior.SnapshotAndReplace);
                }
                else
                    TextBlockNullPage.Opacity = 0.4d;
            }
        }

        /// <summary>
        /// Узнать следующий индекс элемента
        /// </summary>
        /// <param name="ActualIndex">Текущий индекс</param>
        /// <param name="Count">Количество элементов</param>
        /// <returns>Ожидаемый индекс элемента</returns>
        private static int NextIndex(int ActualIndex, int Count) => ActualIndex < Count ? ActualIndex : --ActualIndex;
    }
}
