using ICSharpCode.AvalonEdit;
using System;
using System.ComponentModel;
using System.Windows;

namespace BoilerplateGenerator.Controls
{
    public class BindableTextEditor : TextEditor, INotifyPropertyChanged
    {
        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text == value)
                {
                    return;
                }

                base.Text = value;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(BindableTextEditor), new PropertyMetadata((obj, args) =>
        {
            var target = (BindableTextEditor)obj;
            string newValue = (string)args.NewValue;

            if (target.Text == newValue)
            {
                return;
            }

            target.Text = newValue;
        }));

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(Text));
            base.OnTextChanged(e);
        }

        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
