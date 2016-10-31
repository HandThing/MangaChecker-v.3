﻿using System.Windows.Controls;
using MangaCheckerV3.ViewModels;

namespace MangaCheckerV3.Views {
	/// <summary>
	///     Interaction logic for AddMangaView.xaml
	/// </summary>
	public partial class AddMangaView : UserControl {
		public AddMangaView() {
			InitializeComponent();
			DataContext = new AddMangaViewModel();
		}
	}
}