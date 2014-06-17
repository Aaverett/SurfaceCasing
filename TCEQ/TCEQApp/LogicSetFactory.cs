using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using BEGWebAppLib;
/// <summary>
/// Sadly, it doesn't seem to be possible to 
/// </summary>
public class LogicSetFactory
{
    private ListDictionary _LogicTypes;
    private Assembly _asm;
    
    public LogicSetFactory()
    {
        _LogicTypes = new ListDictionary();

        Initialize();
    }

    public ILogicSet CreateLogicSet(object key)
    {
        //If they don't send us a key, we can't do much.
        if (key == null) return null;

        Type type = (Type)_LogicTypes[key];

        if (type != null)
        {
            object inst = _asm.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);
            
            if (inst == null) return null;

            ILogicSet prod = (ILogicSet)inst;

            return prod;
        }

        return null;
    }

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
                Type iFactoryProduct = type.GetInterface("ILogicSet");
                if (iFactoryProduct != null)
                {
                    //Create a temporary instance of that class.
                    object inst = asm.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                    if (inst != null)
                    {
                        //And generate the product classes key
                        ILogicSet keyDesc = (ILogicSet)inst;
                        object key = keyDesc.GetFactoryKey();
                        inst = null;

                        _LogicTypes.Add(key, type);
                    }
                }
            }
        }

        _asm = asm;
    }
    #endregion
}
