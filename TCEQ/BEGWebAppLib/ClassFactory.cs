using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;


namespace BEGWebAppLib
{
    /// <summary>
    /// This singleton class is used to create objects of an arbitrary class.
    /// </summary>
    public abstract class ClassFactory
    {
        private ClassFactory()
        {
            m_ProductTypes = new ListDictionary();
            Initialize();
        }

        public static ClassFactory Instance
        {
            get
            {
                return Singleton<ClassFactory>.Instance;
            }
        }

        
        #region Constructor

        private ClassFactory()
        {
            m_ProductTypes = new ListDictionary();
          
            Initialize();
        }

        #endregion

        #region variables

        private ListDictionary m_ProductTypes;
    
        private Assembly m_asm;

        #endregion

        #region methods
        
        public IFactoryProduct CreateProduct(object key)
        {
            if (key == null)
            {
                throw new NullReferenceException("Invalid key supplied.  Must be non-null");
            }

            //Get the info about the type the user wants.
            Type type = (Type)m_ProductTypes[key];

            //Make sure we found a valid
            if (type != null)
            {
                object inst = m_asm.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                if (inst == null) throw new NullReferenceException("Null product instance.  Unable to create necessary product class.");

                IFactoryProduct product;

                try
                {
                    product = (IFactoryProduct)inst;
                }
                catch
                {
                    product = null;
                }

                return product;
            }

            return null;
        }
        
        #endregion

        #region helpers
        private void Initialize()
        {
            Assembly asm = Assembly.GetCallingAssembly();

            //Get a list of all the types in this assembly.
            Type[] allTypes = asm.GetTypes();
            foreach (Type type in allTypes)
            {
                //Only examine classes that aren't abstract
                if (type.IsClass && !type.IsAbstract)
                {
                    //If a class implements the IFactoryProduct interface, which allows retrieve of the product class key...
                    Type iFactoryProduct = type.GetInterface("IFactoryProduct");
                    if (iFactoryProduct != null)
                    {
                        //Create a temporary instance of that class.
                        object inst = asm.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                        if (inst != null)
                        {
                            //And generate the product classes key
                            IFactoryProduct keyDesc = (IFactoryProduct)inst;
                            object key = keyDesc.GetFactoryKey();
                            inst = null;

                            //Say we have several different kinds of objects (Cars, trucks and airplanes) and within each kind, several classes (Corvette, Civic, Camry, etc).  
                            //Note:  It looks to me like the names of the interfaces could be written into a database or something
                            //such that this part is done with a loop at runtime.
                            Type prodInterface = type.GetInterface("ILogicSet");
                            if (prodInterface != null)
                            {
                                m_ProductTypes.Add(key, type);
                            }
                        }
                    }
                }
            }

            m_asm = asm;
        }
        #endregion
    }
}
