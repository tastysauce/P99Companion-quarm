using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for NotesControl.xaml
    /// </summary>
    public partial class NotesControl : UserControl, IEventSubscriber<NoteUpdateEvent>
    {
        private readonly INoteService _noteService;

        public NotesControl()
        {
            InitializeComponent();
            _noteService = DependencyInjector.Resolve<INoteService>();
            var eventService = DependencyInjector.Resolve<IEventService>();

            LoadNotes();

            eventService.AddSubscriber(this);
        }

        private void LoadNotes()
        {
            var notes = _noteService.GetAllNotes().OrderBy(x => x.ServerName).ThenBy(x => x.CharacterName).ToList();
            ResultsListView.ItemsSource = notes;
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content as NoteItem;

            var editNoteWindow = new EditNoteWindow();
            editNoteWindow.Note = item;
            editNoteWindow.ShowDialog();
        }

        public Task Handle(NoteUpdateEvent value)
        {
            LoadNotes();
            return Task.CompletedTask;
        }
    }
}
