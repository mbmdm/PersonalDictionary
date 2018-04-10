using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDictionary
{
    public static class Testing
    {
        public static void Clear()
        {
            DB.GetInstance().Init();
        }

        public static void RegisterApplet(AppletData data)
        {
            DB.GetInstance().RegisterApplet(data);
        }

        public static AppletData GetAppletData(string uid)
        {
            return new AppletData() { AppletID = uid };
        }

    }
}
