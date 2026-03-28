using System;
using System.Collections.Generic;
using System.Text;

namespace OIEL.UserElementsControl.Base.LabelBase
{
    /// <summary>
    /// Интерфейс реализации ярлыка
    /// </summary>
    public interface ILabelAction
    {
        internal delegate void VoidHandler();
        internal delegate void ValueChangedHandler<T>(T? OldValue, T? NewValue);
    }
}
