using System.Runtime.CompilerServices;
using System.Windows.Markup;
using TestApp.Interfaces;
using TestApp.Messages;
using TestApp.Registry;

namespace TestApp.Collections
{
    internal class ComponentDictionary : Dictionary<Enum, IComponent>
    {
        public new void Add(Enum key, IComponent value)
        {
            if (key.GetType() != typeof(ComponentKey))
                throw new InvalidOperationException(CollectionExceptionMessage.ComponentDicInvalidKey);

            base.Add(key, value);
        }

        public new IComponent this[Enum key]
        {
            get => base[key];
            set
            {
                if (key.GetType() != typeof(ComponentKey))
                    throw new InvalidOperationException(CollectionExceptionMessage.ComponentDicInvalidKey);

                base[key] = value;
            }
        }

        internal void Initialise()
        {
            foreach (IComponent component in Values)
                component.Initialise();
        }

        internal void Update()
        {
            foreach (IComponent component in Values)
                component.Update();
        }

        internal void CleanUp()
        {
            foreach (IComponent component in Values)
                component.CleanUp();
        }
    }
}
