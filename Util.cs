
using System;
using System.Collections.Generic;
using System.Reflection;

public class Util {

    public static Dictionary<string, MemberInfo> dict = new Dictionary<string, MemberInfo>();

    public static object getPrivateProperty(Type t, object obj, string propertyName) {
        string key = t.ToString() + ":" + propertyName;
        if (!dict.ContainsKey(key)) {
            dict[key] = t.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(nonPublic: true);
        }
        return ((MethodInfo)dict[key]).Invoke(obj, null);
    }

}
