using AllOverIt.Extensions;
using AllOverIt.Reactive.Extensions;
using ObservableDemo.Entities;
using ObservableDemo.Models;

namespace ObservableDemo
{
    internal class Program
    {
        static void Main()
        {
            // Create a non-observable parent
            var parentEntity = GetAFamilyParent();

            // Create observable proxy's
            var observableParent = new PersonModel(parentEntity);

            // Event handlers
            observableParent.PropertyChanging += ObservableParent_PropertyChanging;
            observableParent.PropertyChanged += ObservableParent_PropertyChanged;

            // WhenPropertyChanging subscriptions
            var firstNameChangingSubscription = observableParent
                .WhenPropertyChanging(parent => parent.FirstName)
                .Subscribe(firstName =>
                {
                    Console.WriteLine($"WhenPropertyChanging: FirstName from {firstName}");
                });

            var lastNameChangingSubscription = observableParent
                .WhenPropertyChanging(parent => parent.LastName)
                .Subscribe(lastName =>
                {
                    Console.WriteLine($"WhenPropertyChanging: LastName from {lastName}");
                });

            var fullNameChangingSubscription = observableParent
                .WhenPropertyChanging(parent => parent.FullName)
                .Subscribe(fullName =>
                {
                    Console.WriteLine($"WhenPropertyChanging: FullName from {fullName}");
                });

            // WhenPropertyChanged subscriptions
            var firstNameChangedSubscription = observableParent
                .WhenPropertyChanged(parent => parent.FirstName)
                .Subscribe(firstName =>
                {
                    Console.WriteLine($"WhenPropertyChanged: FirstName to {firstName}");
                });

            var lastNameChangedSubscription = observableParent
                .WhenPropertyChanged(parent => parent.LastName)
                .Subscribe(lastName =>
                {
                    Console.WriteLine($"WhenPropertyChanged: LastName to {lastName}");
                });

            var fullNameChangedSubscription = observableParent
                .WhenPropertyChanged(parent => parent.FullName)
                .Subscribe(fullName =>
                {
                    Console.WriteLine($"WhenPropertyChanged: FullName to {fullName}");
                });

            // Initial state
            Console.WriteLine($"Entity: FirstName = {parentEntity.FirstName}");
            Console.WriteLine($"Entity: LastName = {parentEntity.LastName}");
            Console.WriteLine($"Entity: FullName = {parentEntity.FullName}");

            Console.WriteLine();

            Console.WriteLine("Event handlers and subscriptions will be raised...");

            observableParent.FirstName = "Mathew";
            observableParent.LastName = "Robson";

            Console.WriteLine();

            // Dispose of these subscriptions to show they don't fire again
            firstNameChangingSubscription.Dispose();
            lastNameChangingSubscription.Dispose();
            fullNameChangingSubscription.Dispose();

            firstNameChangedSubscription.Dispose();
            lastNameChangedSubscription.Dispose();
            fullNameChangedSubscription.Dispose();

            Console.WriteLine("Only the event handlers will be raised...");

            observableParent.FirstName = "Peter";
            observableParent.LastName = "Clarkson";

            Console.WriteLine();

            // Show the wrapped 'entity' has been updated
            // Showing access to the underlying model (of observableParent) for when you don't have direct access (to parentEntity)
            Console.WriteLine($"Entity: FirstName = {observableParent.Model.FirstName}");
            Console.WriteLine($"Entity: LastName = {parentEntity.LastName}");
            Console.WriteLine($"Entity: FullName = {parentEntity.FullName}");

            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ObservableParent_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            var value = sender!.GetPropertyValue<string>(e.PropertyName!);

            Console.WriteLine($"PropertyChanging: {e.PropertyName} from {value}");
        }

        private static void ObservableParent_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var value = sender!.GetPropertyValue<string>(e.PropertyName!);

            Console.WriteLine($"PropertyChanged: {e.PropertyName} to {value}");
        }

        private static PersonEntity GetAFamilyParent()
        {
            return new PersonEntity
            {
                FirstName = "Roger",
                LastName = "Robertson"
            };
        }
    }
}