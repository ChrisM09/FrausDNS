using PropertyChanged;
using System.ComponentModel;

namespace FrausDNS.ViewModel
{

    /// <summary>
    /// Base View Model that implements INotifyPropertyChanged property and uses FodyWeavers to 
    /// auto generate OnPropertyChanged Events.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is fired when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
