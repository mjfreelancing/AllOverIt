using AllOverIt.Reactive;
using ObservableDemo.Entities;

namespace ObservableDemo.Models
{
    // An observable model, wrapping a non-observable model
    internal sealed class PersonModel : ObservableProxy<PersonEntity>
    {
        public PersonModel(PersonEntity parent)
            : base(parent)
        {
        }

        public string FirstName
        {
            get => Model.FirstName;
            set
            {
                // onChanging and onChanged are optional
                _ = RaiseAndSetIfChanged(Model.FirstName, value,
                    (model, newValue) => model.FirstName = newValue,            // Updates the wrapped model
                    onChanging: () => RaisePropertyChanging(nameof(FullName)),
                    onChanged: () => RaisePropertyChanged(nameof(FullName)));
            }
        }

        public string LastName
        {
            get => Model.LastName;
            set
            {
                // onChanging and onChanged are optional
                _ = RaiseAndSetIfChanged(Model.LastName, value,
                    (model, newValue) => model.LastName = newValue,             // Updates the wrapped model
                    onChanging: () => RaisePropertyChanging(nameof(FullName)),
                    onChanged: () => RaisePropertyChanged(nameof(FullName)));
            }
        }

        public string FullName => Model.FullName;
    }
}