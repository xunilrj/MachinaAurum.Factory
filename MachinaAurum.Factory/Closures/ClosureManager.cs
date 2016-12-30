using MachinaAurum.Factory.LifetimeManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MachinaAurum.Factory.Closures
{
    class ClosureManager
    {
        AssemblyName AssemblyName;
        AssemblyBuilder AssemblyBuilder;
        ModuleBuilder ModuleBuilder;

        Dictionary<Type, Type> TypeMapping = new Dictionary<Type,Type>();
        Dictionary<Type, Type[]> TypeInstancesMapping = new Dictionary<Type, Type[]>();

        public ClosureManager()
        {
            AssemblyName = new AssemblyName();
            AssemblyName.Name = "MachinaAurum.Factory.Closures";

            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(AssemblyBuilder.GetName().Name, false);
        }

        public Tuple<Type, Type[]> CreateType(IFactory factory, Type rootType)
        {
            Type closureType = null;

            if (TypeMapping.TryGetValue(rootType, out closureType))
            {
                return Tuple.Create(closureType, TypeInstancesMapping[rootType]);
            }

            TypeBuilder typeBuilder = ModuleBuilder.DefineType(Guid.NewGuid().ToString(),
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(object));

            var instancesDictionary = new Dictionary<IHaveInstanceLifetimeManager, FieldInfo>();

            GetClosureCapturesRoot(factory, rootType, typeBuilder, instancesDictionary);

            var instancesTypes = instancesDictionary.Select(x => x.Value.FieldType).ToArray();

            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                instancesTypes);

            ILGenerator il = constructor.GetILGenerator();

            int i = 0;
            foreach (var item in instancesDictionary)
            {
                if (i == 0)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Stfld, item.Value);
                }
                else if (i == 1)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Stfld, item.Value);
                }
                else if (i == 2)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_3);
                    il.Emit(OpCodes.Stfld, item.Value);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_S, i + 1);
                    il.Emit(OpCodes.Stfld, item.Value);
                }

                ++i;
            }

            il.Emit(OpCodes.Ret);

            var buildDef = typeBuilder.DefineMethod("Build", MethodAttributes.Public, CallingConventions.HasThis, typeof(object), Type.EmptyTypes);
            il = buildDef.GetILGenerator();

            GenerateCodeRoot(factory, instancesDictionary, rootType, il);

            il.Emit(OpCodes.Ret);

            closureType = typeBuilder.CreateType();

            TypeMapping.Add(rootType, closureType);
            TypeInstancesMapping.Add(rootType, instancesTypes);

            return Tuple.Create(closureType, instancesTypes);
        }

        private void GetClosureCapturesRoot(IFactory factory, Type type, TypeBuilder typeBuilder, Dictionary<IHaveInstanceLifetimeManager, FieldInfo> instancesDictionary)
        {
            var newType = factory.GetImplementationType(type) ?? type;
            var ctor = newType.GetConstructors().First();

            foreach (var parameter in ctor.GetParameters())
            {
                GetClosureCaptures(factory, parameter.ParameterType, typeBuilder, instancesDictionary);
            }
        }

        private void GetClosureCaptures(IFactory factory, Type type, TypeBuilder typeBuilder, Dictionary<IHaveInstanceLifetimeManager, FieldInfo> instancesDictionary)
        {
            var manager = factory.GetLifetimeManager(type);

            if (manager is IHaveInstanceLifetimeManager)
            {
                var instanceManager = manager as IHaveInstanceLifetimeManager;

                if (instancesDictionary.ContainsKey(instanceManager) == false)
                {
                    instancesDictionary[instanceManager] = typeBuilder.DefineField(Guid.NewGuid().ToString(), type, FieldAttributes.Private);
                }

                return;
            }

            var newType = factory.GetImplementationType(type) ?? type;
            var ctor = newType.GetConstructors().First();

            foreach (var parameter in ctor.GetParameters())
            {
                GetClosureCaptures(factory, parameter.ParameterType, typeBuilder, instancesDictionary);
            }
        }

        private void GenerateCodeRoot(IFactory factory, Dictionary<IHaveInstanceLifetimeManager, FieldInfo> singletons, Type type, ILGenerator ilgen)
        {
            var newCurrent = factory.GetImplementationType(type) ?? type;
            var ctor = newCurrent.GetConstructors().First();

            foreach (var parameter in ctor.GetParameters())
            {
                GenerateCode(factory, singletons, parameter.ParameterType, ilgen);
            }

            ilgen.Emit(OpCodes.Newobj, ctor);
        }

        private void GenerateCode(IFactory factory, Dictionary<IHaveInstanceLifetimeManager, FieldInfo> singletons, Type current, ILGenerator ilgen)
        {
            var manager = factory.GetLifetimeManager(current);

            if (manager is IHaveInstanceLifetimeManager)
            {
                var instanceManager = manager as IHaveInstanceLifetimeManager;

                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldfld, singletons[instanceManager]);
            }
            else
            {
                var newCurrent = factory.GetImplementationType(current) ?? current;
                var ctor = newCurrent.GetConstructors().First();

                foreach (var parameter in ctor.GetParameters())
                {
                    GenerateCode(factory, singletons, parameter.ParameterType, ilgen);
                }

                ilgen.Emit(OpCodes.Newobj, ctor);
            }
        }        
    }
}

