
namespace HangmanAssignment;

public partial class HangmanGamePage : ContentPage
{
	public HangmanGamePage()
	{
		InitializeComponent();
		BindingContext = new myViewModel();
	}
}