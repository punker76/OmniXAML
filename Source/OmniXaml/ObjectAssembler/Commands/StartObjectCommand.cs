namespace OmniXaml.ObjectAssembler.Commands
{
    using System.Collections;
    using Typing;

    public class StartObjectCommand : Command
    {
        private readonly XamlType xamlType;
        private readonly object rootInstance;

        public StartObjectCommand(ObjectAssembler assembler, XamlType xamlType, object rootInstance) : base(assembler)
        {
            this.xamlType = xamlType;
            this.rootInstance = rootInstance;
        }

        public override void Execute()
        {
            if (StateCommuter.Level == 0)
            {
                throw new XamlParseException("An object cannot start after level zero has been reached. This condition may indicate that there are more than one object at the Root Level. Please, verify that there is ONLY one root object.");
            }

            if (ConflictsWithObjectBeingConfigured)
            {
                StateCommuter.RaiseLevel();
            }

            StateCommuter.Current.XamlType = xamlType;
            OverrideInstanceAndTypeInLevel1();
        }

        private bool ConflictsWithObjectBeingConfigured => StateCommuter.Current.HasInstance || StateCommuter.Current.IsGetObject;

        private void OverrideInstanceAndTypeInLevel1()
        {
            if (StateCommuter.Level == 1 && rootInstance != null)
            {
                StateCommuter tempQualifier = StateCommuter;
                tempQualifier.Current.Instance = rootInstance;

                var collection = rootInstance as ICollection;
                if (collection != null)
                {
                    tempQualifier.Current.Collection = collection;
                }
                var typeContext = Assembler.WiringContext.TypeContext;
                var xamlTypeOfInstance = typeContext.GetXamlType(rootInstance.GetType());
                StateCommuter.Current.XamlType = xamlTypeOfInstance;
            }
        }
    }
}