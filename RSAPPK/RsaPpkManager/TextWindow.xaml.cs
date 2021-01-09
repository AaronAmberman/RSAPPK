using System.Windows;

namespace RsaPpkManager
{
    public partial class TextWindow : Window
    {
        public string Text 
        { 
            get
            {
                return text.Text;
            }
            set
            {
                text.Text = value;
            }
        }

        public TextWindow()
        {
            InitializeComponent();
        }
    }
}
