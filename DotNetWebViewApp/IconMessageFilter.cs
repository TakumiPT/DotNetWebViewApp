using System.Windows.Forms;

namespace DotNetWebViewApp
{
    public class IconMessageFilter : IMessageFilter
    {
        private readonly Icon appIcon;

        public IconMessageFilter(Icon appIcon)
        {
            this.appIcon = appIcon;
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_CREATE = 0x0001;

            if (m.Msg == WM_CREATE)
            {
                Form form = Form.FromHandle(m.HWnd) as Form;
                if (form != null && form.Icon == null)
                {
                    form.Icon = appIcon;
                }
            }

            return false;
        }
    }
}
