using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OIEL.UserElementsControl.Base.LabelBase;
using System.Windows;

namespace OIEL.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelTag.xaml
    /// </summary>
    public partial class OPLLabelTag : System.Windows.Controls.UserControl
    {
        internal delegate void ChangedValueHandler<T>(T OldValue, T NewValue);

        #region Properties

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(System.Windows.Media.FontFamily), typeof(OPLLabelTag),
                new(
                    (sender, e) =>
                    {
                        ((OPLLabelTag)sender).IELTag.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт элемента
        /// </summary>
        public new System.Windows.Media.FontFamily FontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #region FontSize
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(OPLLabelTag),
                new(
                    (sender, e) =>
                    {
                        ((OPLLabelTag)sender).IELTag.FontSize = (double)e.NewValue;
                    }));

        /// <summary>
        /// Размер шрифта элемента
        /// </summary>
        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLLabelTag),
                new(
                    (sender, e) =>
                    {
                        ((OPLLabelTag)sender).IELTag.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемый текст в элементе
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region Tag
        private LabelTag _Tag;
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(LabelTag), typeof(OPLLabelTag),
                new(
                    (sender, e) =>
                    {
                        ((OPLLabelTag)sender)._Tag = (LabelTag)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемый текст в элементе
        /// </summary>
        public new LabelTag Tag
        {
            get => (LabelTag)base.GetValue(TagProperty);
            set
            {
                TagChanged.Invoke(_Tag, value);
                base.SetValue(TagProperty, value);
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Событие изменения тега
        /// </summary>
        internal event ChangedValueHandler<LabelTag> TagChanged;

        /// <summary>
        /// Базовый объект отображения тега
        /// </summary>
        public IELObjectBase IELObjectTag => IELTag;

        public OPLLabelTag()
        {
            InitializeComponent();
            //App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELTag);
            _Tag = new(string.Empty);
            TagChanged += (Old, New) =>
            {
                _Tag = New;
                _Tag.TagValueChanged += (OldV, NewV) =>
                {
                    IELTag.Text = NewV ?? string.Empty;
                };
                IELTag.Text = _Tag.ValueTag;
            };
        }
    }
}
