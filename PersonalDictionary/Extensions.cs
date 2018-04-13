using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDictionary
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static WordProgress CalcProgress(Word w)
        {
            //Получаем список всех апплетов, данные из которых буду учитываться при анализе изученности слова
            string active_applets = Settings.Get()[Settings.Applets_to_calc_full_progress];
            int count_active_applets = Settings.Get()[Settings.Applets_to_calc_full_progress].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries).Length;


            DB db = DB.GetInstance();

            
            List<KeyValuePair<Word, int>> fullProgress = new List<KeyValuePair<Word, int>>(); //Собираем весь прогресс по заданному слову
            foreach (var d in db.ApplestsData)
                if (active_applets.Contains(d.AppletID))
                    foreach (var wp in d.WordProgress)
                        if (wp.Key == w)
                            fullProgress.Add(wp);

            int sum = 0; // Cуммарный прогресс по всем апплетам
            foreach (var item in fullProgress)
                sum+= (item.Value > 100) ? 100 : item.Value;

            double resoult = (double)sum / (count_active_applets * 100);

            if (resoult == 0)
                return WordProgress.No;
            else if (resoult < 0.5)
                return WordProgress.Low;
            else if (resoult >= 0.5 && resoult < 1)
                return WordProgress.High;
            else return WordProgress.Full;
        }
    }

    public enum WordProgress { No=1, Low, High, Full};
}
