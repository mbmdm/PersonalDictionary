using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDictionary
{
    public interface IApplet
    {
        /// <summary>
        /// Визуализирует диалог. Как правило разработчик в данном методе должен вызывать ShowDialog()
        /// </summary>
        void Run();
        /// <summary>
        /// Имя, которое будет отображаться в контекстном меню Notify
        /// </summary>
        /// <returns></returns>
        string DisplayName();
        /// <summary>
        /// Используется для определения положения строки в контекстном меню Notify. Задавать рекомендуется кратное 10, чтобы можно было в будущем иметь гибкость по настройке порядка следования элементов меню.
        /// </summary>
        /// <returns></returns>
        int Position();
        /// <summary>typedef(...)</summary>
        /// <returns></returns>
        Type ToType();
        /// <summary>
        /// Если возвращает true, то именно этот диалог будет открываться по 2х клику на Notify
        /// </summary>
        /// <returns></returns>
        bool IsMainDialog();
    }
}
