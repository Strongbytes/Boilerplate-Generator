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
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        internal string BaseText
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(BindableTextEditor), new PropertyMetadata((obj, args) =>
        {
            BindableTextEditor target = (BindableTextEditor)obj;

            if (target.BaseText == (string)args.NewValue)
            {
                return;
            }

            target.BaseText = (string)args.NewValue;
        }));

        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(TextProperty, BaseText);
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
