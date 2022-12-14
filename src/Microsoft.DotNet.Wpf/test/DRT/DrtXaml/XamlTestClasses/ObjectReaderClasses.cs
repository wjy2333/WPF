// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Elements
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml.Schema;

    public class TwoDictionaries
    {
        public DerivedDictionary One { get; set; }
        public DerivedDictionary Two { get; set; }
    }

    public class DerivedDictionary : Dictionary<string, string>
    {
        public DerivedDictionary()
        {
        }
    }

    public class NameReferencedHoldsTwoElements
    {
        public Element One
        {
            get;
            set;
        }

        [TypeConverter(typeof(NameReferenceConverter))]
        public Element Two
        {
            get;
            set;
        }
    }

    public class NameElement : Element
    {
        [TypeConverter(typeof(HoldsOneElementConverter))]
        public HoldsOneElement Container { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class NameElementWithRuntimeName : Element
    {
        public string Name { get; set; }

        [TypeConverter(typeof(HoldsOneElementConverter))]
        public HoldsOneElement Container { get; set; }
    }

    public class HoldsOneElementConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var container = value as HoldsOneElement;
            var service = context.GetService(typeof(IXamlNameProvider)) as IXamlNameProvider;
            string name = service.GetName(container.Element);
            return name;
        }
    }

    public class UselessConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProperty("Element")]
    public class UselessConverterContainer
    {
        [TypeConverter(typeof(UselessConverter))]
        public Element Element { get; set; }
    }

    public class ReadWriteDictionaryContainer
    {
        public Hashtable Dictionary
        {
            get;
            set;
        }
    }

    public class ReadWriteNonSelfInstantiatingCollection
    {
        public RunCollection Collection
        {
            get;
            set;
        }
    }

    public class RunCollectionContainer
    {
        RunCollection col = new RunCollection();
        public RunCollection Col
        {
            get
            {
                return col;
            }
        }
    }

    [ContentProperty("Text")]
    public class MyRun
    {
        public string Text
        {
            get;
            set;
        }
    }

    [ContentWrapper(typeof(MyRun))]
    public class RunCollection : IEnumerable
    {
        int index = 0;
        MyRun[] runs = new MyRun[5];
        
        public void Add(MyRun run)
        {
            runs[index++] = run;
        }

        public IEnumerator GetEnumerator()
        {
            return new MyRunEnumerator(runs);
        }

        class MyRunEnumerator : IEnumerator
        {
            int index = -1;
            MyRun[] runs;

            public MyRunEnumerator(MyRun[] runs)
            {
                this.runs = runs;
            }

            public object Current
            {
                get
                {
                    return runs[index];
                }
            }

            public bool MoveNext()
            {
                if (index == 4)
                {
                    return false;
                }
                index++;
                return true;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }

    public class WhitespaceSigRunCollectionContainer
    {
        WhitespaceSigRunCollection col = new WhitespaceSigRunCollection();
        public WhitespaceSigRunCollection Col
        {
            get
            {
                return col;
            }
        }
    }

    [WhitespaceSignificantCollection]
    [ContentWrapper(typeof(MyRun))]
    public class WhitespaceSigRunCollection : IEnumerable
    {
        int index = 0;
        MyRun[] runs = new MyRun[5];

        public void Add(MyRun run)
        {
            runs[index++] = run;
        }

        public IEnumerator GetEnumerator()
        {
            return new MyRunEnumerator(runs);
        }

        class MyRunEnumerator : IEnumerator
        {
            int index = -1;
            MyRun[] runs;

            public MyRunEnumerator(MyRun[] runs)
            {
                this.runs = runs;
            }

            public object Current
            {
                get
                {
                    return runs[index];
                }
            }

            public bool MoveNext()
            {
                if (index == 4)
                {
                    return false;
                }
                index++;
                return true;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }    

    [DictionaryKeyProperty("DkpProp")]
    public class ClassWithDKP
    {
        public string DkpProp { get; set; }
    }

    [DictionaryKeyProperty("DkpProp")]
    public class ClassWithReadOnlyDKP
    {
        string prop;
        public string DkpProp { get { return prop; } }

        public void SetDkpProp(string s)
        {
            prop = s;
        }
    }

    public class ClassWithCollectionWithPrivateSetter
    {
        ArrayList arrayList = new ArrayList() { 1, "Hello" };
        public ArrayList ArrayList
        {
            get
            {
                return arrayList;
            }

            private set
            {
                arrayList = value;
            }
        }
    }

    public class ClassWithInvalidMemberNameForXml
    {
        public string Pro‿p1 { get; set; }
    }

    public struct MyStruct
    {
        public string Category { get; set; }
    }

    public class TryCatchFinally
    {
        public string Try { get; set; }

        [DependsOn("Try")]
        public string Catch { get; set; }

        [DependsOn("Catch")]
        public string Finally { get; set; }
    }

    [ContentProperty("Catch")]
    public class TryCatchFinallyWithContentProperty
    {
        [DependsOn("Catch")]
        public string Finally { get; set; }

        [DependsOn("Try")]
        public string Catch { get; set; }

        public string Try { get; set; }
    }

    public class Clothing
    {
        public string Gloves { get; set; }

        [DependsOn("Vest")]
        [DefaultValue(null)]
        public string Suit { get; set; }

        public string Clone { get; set; }

        [DependsOn("Suit")]
        [DependsOn("Pants")]
        [DependsOn("Clone")]
        public string Coat { get; set; }

        public string Underwear { get; set; }

        [DependsOn("Shirt")]
        public string Tie { get; set; }

        [DependsOn("Tie")]
        [DefaultValue(null)]
        public string Vest { get; set; }

        public string Hat { get; set; }
        public string Socks { get; set; }

        public string Shirt { get; set; }

        [DependsOn("Underwear")]
        public string Pants { get; set; }
    }

    [ContentProperty("Hat")]
    public class ComplexClothing
    {
        public string Gloves { get; set; }

        [DependsOn("Vest")]
        public ComplexType Suit { get; set; }

        public string Clone { get; set; }

        [DependsOn("Suit")]
        [DependsOn("Pants")]
        [DependsOn("Clone")]
        public string Coat { get; set; }

        public ComplexType Underwear { get; set; }

        [DependsOn("Shirt")]
        public ClassWithMEConverter Tie { get; set; }

        [DependsOn("Tie")]
        public string Vest { get; set; }

        public string Hat { get; set; }
        public string Socks { get; set; }

        public string Shirt { get; set; }

        [DependsOn("Underwear")]
        public string Pants { get; set; }
    }

    public class CircularDependency
    {
        [DependsOn("Egg")]
        public string Chicken { get; set; }

        [DependsOn("Chicken")]
        public string Egg { get; set; }
    }

    public class ClassWithTCOnParent : ClassWithTypeConverter
    {
    }

    public class ClassWithTCOnGrandparent : ClassWithTCOnParent
    {
    }

    public class Annotation
    {
        public int JobId { get; set; }
    }

    public class Annotations : ICollection<Annotation>
    {
        private Dictionary<int, Annotation> dict = new Dictionary<int, Annotation>();
        public Annotation Find(int jobId)
        {
            if (dict.ContainsKey(jobId))
                return dict[jobId];
            else
                return null;
        }

        public void Add(Annotation item)
        {
            dict.Add(item.JobId, item);
            //((ICollection<Annotation>)this).Add(item);
        }

        public IEnumerator<Annotation> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        void ICollection<Annotation>.Clear()
        {
            dict.Clear();
        }

        bool ICollection<Annotation>.Contains(Annotation item)
        {
            foreach (Annotation annotation in dict.Values)
            {
                if (annotation == item)
                    return true;

            }
            return false;
        }

        void ICollection<Annotation>.CopyTo(Annotation[] array, int arrayIndex)
        {
            foreach (Annotation annotation in array)
            {
                dict.Add(annotation.JobId, annotation);
            }
        }

        int ICollection<Annotation>.Count
        {
            get { return dict.Count; }
        }

        bool ICollection<Annotation>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<Annotation>.Remove(Annotation item)
        {
            return dict.Remove(item.JobId);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }
    }

    public class CustomStackPanel : StackPanel
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }
    }

    public static class ArrayAttachedPropertyOwner
    {
        static AttachableMemberIdentifier stringsPropertyName = new AttachableMemberIdentifier(typeof(ArrayAttachedPropertyOwner), "Strings");

        public static string[] GetStrings(object target)
        {
            string[] value;
            if (AttachablePropertyServices.TryGetProperty(target, stringsPropertyName, out value))
            {
                return value;
            }
            return new string[] { };
        }

        public static void SetStrings(object target, string[] strings)
        {
            AttachablePropertyServices.SetProperty(target, stringsPropertyName, strings);
        }
    }

    [TypeConverter(typeof(TigerConverter))]
    public struct Tiger
    {
        string name;
        public Tiger(string name)
        {
            this.name = name;
        }

        public string NickName
        {
            get { return this.name; }
        }
    }

    public class TigerConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Tiger(value as string);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Tiger)
            {
                return ((Tiger)value).NickName;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }

    public class TigerContainer
    {
        Tiger tiger;

        public Tiger Tiger
        {
            get { return tiger; }
            set { tiger = value; }
        }
    }

    public class MyClassConverterCSD : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new ClassWithTypeConverter(int.Parse((string)value));
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((ClassWithTypeConverter)value).Info.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ClassWithTypeConverterContainer
    {
        public ClassWithTypeConverter Prop { get; set; }
    }

    [TypeConverter(typeof(MyClassConverterCSD))]
    public class ClassWithTypeConverter
    {
        int info;
        public ClassWithTypeConverter()
        {
            this.info = 0;
        }

        public ClassWithTypeConverter(int info)
        {
            this.info = info;
        }

        public int Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
            }
        }

        public override bool Equals(object obj)
        {
            ClassWithTypeConverter c = obj as ClassWithTypeConverter;

            return (c == null) ? false : (this.info == c.info);
        }

        public override int GetHashCode()
        {
            return Info;
        }
    }

    public class ClassWithListProperty
    {
        List<string> data = new List<string>();

        public List<string> Data
        { get { return data; } }
    }

    public class ClassWithListPropertyAndDefaultValue
    {
        [DefaultValue(null)]
        public List<string> Data
        {
            get;
            set;
        }
    }

    public class ClassWithDictionaryProperty
    {
        Dictionary<string, int> data = new Dictionary<string, int>();

        public Dictionary<string, int> Data
        { get { return data; } }
    }

    public class ClassWithDictionaryPropertyAndShouldSerialize
    {
        Dictionary<string, int> data = new Dictionary<string, int>();

        public Dictionary<string, int> Data
        { get { return data; } }

        public bool ShouldSerializeData()
        {
            return Data.Count != 1;
        }
    }

    public class DictionaryAllowingNullKey : IDictionary<string, string>
    {
        Dictionary<string, string> dict = new Dictionary<string,string>();
        bool hasNullKey = false;
        string valueForNullKey = null;

        ICollection<string> IDictionary<string, string>.Keys
        {
            get
            {
                if (hasNullKey)
                {
                    var keys = new List<string>(dict.Keys);
                    keys.Add(null);
                    return keys;
                }

                return dict.Keys;
            }
        }

        ICollection<string> IDictionary<string, string>.Values
        {
            get
            {
                if (hasNullKey)
                {
                    var values = new List<string>(dict.Values);
                    values.Add(valueForNullKey);
                    return values;
                }

                return dict.Values;
            }    
        }
        
        public string this[string key] 
        {
            get
            {
                if (key == null)
                {
                    return valueForNullKey;
                }
                return dict[key];
            }
            set
            {
                if (key == null)
                {
                    valueForNullKey = value;
                }
                else
                {
                    dict[key] = value;
                }
            }
        }

        public void Add(string key, string value)
        {
            if (key == null)
            {
                hasNullKey = true;
                valueForNullKey = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                return hasNullKey;
            }
            return dict.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (key == null)
            {
                hasNullKey = false;
                return true;
            }
            return dict.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            if (!ContainsKey(key))
            {
                value = null;
                return false;
            }

            value = this[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            if (hasNullKey)
            {
                yield return new KeyValuePair<string, string>(null, this.valueForNullKey);
            }
            foreach (var entry in dict)
            {
                yield return entry;
            }
        }

        public int Count
        {
            get
            {
                return hasNullKey ? dict.Count + 1 : dict.Count;
            }
        }

        public bool IsReadOnly 
        { 
            get
            {
                return false;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
            hasNullKey = false;
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return this.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return this.Remove(item.Key);
        }
    }


    public class ClassWithArrayProperty
    {
        Foos[] items;
        public Foos[] Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }
    }

    public class ClassWithPropertyWithDefaultValueOfNull
    {
        [DefaultValue(null)]
        public string Prop1
        { get; set; }
        public string Prop2
        { get; set; }
    }

    public class DefaultValueTestByTypeConverterClass
    {
        ClassWithTypeConverter data;
        public DefaultValueTestByTypeConverterClass()
        {
            this.data = null;
        }

        public DefaultValueTestByTypeConverterClass(ClassWithTypeConverter data)
        {
            this.data = data;
        }

        [DefaultValue(typeof(ClassWithTypeConverter), "100")]
        public ClassWithTypeConverter Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
    }

    public class Simple
    {
        public string Prop
        { get; set; }
    }

    [ContentProperty("Content")]
    public class NameScope : Element, INameScope
    {
        Dictionary<string, object> names = new Dictionary<string, object>();

        public object Content
        { get; set; }

        [DefaultValue(0)]
        public int RandomProp
        { get; set; }

        public object FindName(string name)
        {
            object value;
            if (names.TryGetValue(name, out value))
            {
                return value;
            }
            return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            names[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            names.Remove(name);
        }
    }

    public class RecursiveReference
    {
        public RecursiveReference()
        {
            RR = this;
        }

        public RecursiveReference RR
        { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class Person
    {
        List<Person> friends = new List<Person>();
        public string Name
        { get; set; }
        [DefaultValue(0)]
        public int Age
        { get; set; }
        public List<Person> Friends
        { get { return friends; } }
    }

    [RuntimeNameProperty("Name")]
    public class FooNamed
    {
        public string Name
        { get; set; }
    }

    public class MERefTest
    {
        public MERefTest Reference
        { get; set; }
    }

    public class MERefTestExtension : MarkupExtension
    {

        public MERefTest Value
        { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            MERefTest b = new MERefTest();
            b.Reference = b;
            return b;
        }
    }

    public class DictionaryContainer
    {
        public Dictionary<string, DictionaryContainer> Dict
        {
            get;
            set;
        }
    }

    public class ClassWithBadHashCodeImpl
    {
        public int Data
        { get; set; }

        public override bool Equals(object obj)
        {
            ClassWithBadHashCodeImpl that = obj as ClassWithBadHashCodeImpl;

            if (that == null)
            {
                return false;
            }

            return this.GetHashCode() == that.GetHashCode();
        }
        public override int GetHashCode()
        {
            return this.Data % 2;
        }
    }

    public class XNameToyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var name = value as XNameToy;
            if (destinationType == typeof(string) && name != null)
            {
                if (context != null)
                {
                    var lookupPrefix = (INamespacePrefixLookup)context.GetService(typeof(INamespacePrefixLookup));
                    if (lookupPrefix != null)
                    {
                        string prefix = lookupPrefix.LookupPrefix(name.Namespace);
                        if (prefix == String.Empty)
                        {
                            // Default namespace is in scope
                            //
                            return name.LocalName;
                        }
                        else
                        {
                            return prefix + ":" + name.LocalName;
                        }
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(XNameToyTypeConverter))]
    public class XNameToy
    {
        public string Namespace { get; set; }
        public string LocalName { get; set; }
    }

    public class XNameToyContainer
    {
        public XNameToy XName { get; set; }
    }

    [TypeConverter(typeof(PointWithNoCtorArgAttrsTypeConverter))]
    public class PointWithNoCtorArgAttrs
    {
        int x;
        int y;

        public PointWithNoCtorArgAttrs(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }
    }

    public class PointWithNoCtorArgAttrsTypeConverter : TypeConverter
    {
        public PointWithNoCtorArgAttrsTypeConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is PointWithNoCtorArgAttrs)
            {
                PointWithNoCtorArgAttrs pt = (PointWithNoCtorArgAttrs)value;

                ConstructorInfo ci = typeof(PointWithNoCtorArgAttrs).GetConstructor(new Type[] { typeof(int), typeof(int) });
                if (ci != null)
                {
                    return new InstanceDescriptor(ci, new object[] { pt.X, pt.Y }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class PointWithIncompleteConstructor
    {
        int x;
        int y;
        int z;

        public PointWithIncompleteConstructor(int x, int y)
        {
            this.x = x; this.y = y;
        }

        [ConstructorArgument("x")]
        public int X
        { get { return x; } }
        [ConstructorArgument("y")]
        public int Y
        { get { return y; } }
        public int Z
        { get { return z; } set { z = value; } }
    }

    [TypeConverter(typeof(NestedPointWithNoCtorArgAttrsTypeConverter))]
    public class NestedPointWithNoCtorArgAttrs
    {
        int x;
        int y;
        PointWithNoCtorArgAttrs pt;

        public NestedPointWithNoCtorArgAttrs(int x, int y, PointWithNoCtorArgAttrs pt)
        {
            this.x = x; this.y = y; this.pt = pt;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }
        public PointWithNoCtorArgAttrs Point
        { get { return pt; } }
    }

    public class NestedPointWithNoCtorArgAttrsTypeConverter : TypeConverter
    {
        public NestedPointWithNoCtorArgAttrsTypeConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is NestedPointWithNoCtorArgAttrs)
            {
                NestedPointWithNoCtorArgAttrs pt = (NestedPointWithNoCtorArgAttrs)value;

                ConstructorInfo ci = typeof(NestedPointWithNoCtorArgAttrs).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(PointWithNoCtorArgAttrs) });
                if (ci != null)
                {
                    return new InstanceDescriptor(ci, new object[] { pt.X, pt.Y, pt.Point }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class PointWithNoMatchingConstructor
    {
        int x;
        int y;

        public PointWithNoMatchingConstructor(int x, int y)
        {
            this.x = x; this.y = y;
        }

        [ConstructorArgument("x")]
        public int X
        { get { return x; } }

        public int Y
        { get { return y; } set { y = value; } }
    }

    public class PointWithNoMatchingConstructorIncorrectTypes
    {
        int x;
        int y;

        public PointWithNoMatchingConstructorIncorrectTypes(string x, string y)
        {
            this.x = Int32.Parse(x); this.y = Int32.Parse(y);
        }

        [ConstructorArgument("x")]
        public int X
        { get { return x; } }
        [ConstructorArgument("y")]
        public int Y
        { get { return y; } set { y = value; } }
    }



    public class ConstructorArgTypes1
    {

        public ConstructorArgTypes1(int x, string y)
        {
            this.x = x;
            this.y = y;
        }
        [ConstructorArgument("x")]
        public int x
        { get; set; }

        [ConstructorArgument("y")]
        public string y
        { get; set; }
    }

    public class ConstructorArgTypes5
    {

        public ConstructorArgTypes5(int? x)
        {
            this.nullableInt = x;
        }
        [ConstructorArgument("x")]
        public int? nullableInt
        { get; set; }
    }

    [TypeConverter(typeof(DifferentArgumentMethodClassTypeConverter))]
    public class DifferentArgumentMethodClass
    {
        public int x
        { get; set; }
        public IDictionary<int, int> dictionary
        { get; set; }
        public IList<int> list
        { get; set; }
        public int[] array { get; set; }
        public System.Collections.IEnumerable enumerable
        { get; set; }
        public int? nullableInt
        { get; set; }

        private DifferentArgumentMethodClass()
        {
        }

        public static DifferentArgumentMethodClass MyMethod(int x,
            IDictionary<int, int> dict,
            IList<int> list,
            int[] array,
            System.Collections.IEnumerable enumerable,
            int? nullable)
        {

            return new DifferentArgumentMethodClass()
            {
                x = x,
                dictionary = dict,
                array = array,
                enumerable = enumerable,
                list = list,
                nullableInt = nullable
            };
        }
    }
    public class DifferentArgumentMethodClassTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is DifferentArgumentMethodClass)
            {
                DifferentArgumentMethodClass pt = (DifferentArgumentMethodClass)value;
                MemberInfo mi = typeof(DifferentArgumentMethodClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    // swapped last two arguments //
                    return new InstanceDescriptor(mi, new object[] { pt.x, pt.dictionary, pt.list, pt.array, pt.nullableInt, pt.enumerable }, true);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(CleverTypeConverterForBadClass))]
    public class BadClassWithCleverTypeConverter
    {
        public BadClassWithCleverTypeConverter(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.X = a.ToString();
            this.Y = b.ToString();
            this.Z = c.ToString();
        }
        public BadClassWithCleverTypeConverter(string x, string y, string z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.A = Int32.Parse(x);
            this.B = Int32.Parse(y);
            this.C = Int32.Parse(z);
        }

        [ConstructorArgument("a")]
        public int A
        { get; set; }
        [ConstructorArgument("b")]
        public int B
        { get; set; }
        [ConstructorArgument("c")]
        public int C
        { get; set; }
        [ConstructorArgument("x")]
        public string X
        { get; set; }
        [ConstructorArgument("y")]
        public string Y
        { get; set; }
        [ConstructorArgument("z")]
        public string Z
        { get; set; }
    }

    public class CleverTypeConverterForBadClass : TypeConverter
    {
        public CleverTypeConverterForBadClass()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is BadClassWithCleverTypeConverter)
            {
                BadClassWithCleverTypeConverter bcwctc = (BadClassWithCleverTypeConverter)value;

                ConstructorInfo ci = typeof(BadClassWithCleverTypeConverter).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) });
                if (ci != null)
                {
                    return new InstanceDescriptor(ci, new object[] { bcwctc.A, bcwctc.B, bcwctc.C }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public static class PointFactory
    {
        public static PointWithFactory CreatePoint(int x, int y)
        {
            return new PointWithFactory(x, y);
        }
    }

    public class PointWithBadCasing
    {
        int x;
        int y;

        public PointWithBadCasing(int ecks, int why)
        {
            this.x = ecks; this.y = why;
        }

        [ConstructorArgument("EcKs")]
        public int X
        { get { return x; } }

        [ConstructorArgument("WhY")]
        public int Y
        { get { return y; } set { y = value; } }
    }

    [TypeConverter(typeof(PointWithFactoryTypeConverter))]
    public class PointWithFactory
    {
        int x;
        int y;

        internal PointWithFactory(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        { get { return this.x; } }
        public int Y
        { get { return this.y; } }
    }

    public class PointWithFactoryTypeConverter : TypeConverter
    {
        public PointWithFactoryTypeConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is PointWithFactory)
            {
                PointWithFactory pt = (PointWithFactory)value;

                MethodInfo mi = typeof(PointFactory).GetMethod("CreatePoint", new Type[] { typeof(int), typeof(int) });
                if (mi != null && mi.IsStatic == true)
                {
                    return new InstanceDescriptor(mi, new object[] { pt.X, pt.Y }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class PointWithMultipleMatchingConstructors
    {
        int x;
        int y;

        public PointWithMultipleMatchingConstructors(int x)
        {
            this.x = x; this.y = 0;
        }
        public PointWithMultipleMatchingConstructors(int x, int y)
        {
            this.x = x; this.y = y;
        }

        [ConstructorArgument("x")]
        public int X
        { get { return x; } }

        [ConstructorArgument("y")]
        public int Y
        { get { return y; } }
    }

    [TypeConverter(typeof(PointWithNonStaticMethodTypeConverter))]
    public class PointWithNonStaticMethod
    {
        int x;
        int y;

        internal PointWithNonStaticMethod(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }

        public PointWithNonStaticMethod Create(int x, int y)
        {
            return new PointWithNonStaticMethod(x, y);
        }
        public static PointWithNonStaticMethod CreateSecret(int x, int y)
        {
            return new PointWithNonStaticMethod(x, y);
        }
    }

    public class PointWithNonStaticMethodTypeConverter : TypeConverter
    {
        public PointWithNonStaticMethodTypeConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is PointWithNonStaticMethod)
            {
                PointWithNonStaticMethod pt = (PointWithNonStaticMethod)value;

                MethodInfo mi = typeof(PointWithNonStaticMethod).GetMethod("Create");
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[] { pt.X, pt.Y }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(PointWithStaticMethodTypeConverter))]
    public class PointWithStaticMethod
    {
        int x;
        int y;

        internal PointWithStaticMethod(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }

        public static PointWithStaticMethod Create(int x, int y)
        {
            return new PointWithStaticMethod(x, y);
        }
    }

    public class PointWithStaticMethodTypeConverter : TypeConverter
    {
        public PointWithStaticMethodTypeConverter()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is PointWithStaticMethod)
            {
                PointWithStaticMethod pt = (PointWithStaticMethod)value;

                MethodInfo mi = typeof(PointWithStaticMethod).GetMethod("Create");
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[] { pt.X, pt.Y }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(PointWithStaticMethodTypeConverter2))]
    public class PointWithStaticMethod2
    {
        int x;
        int y;

        internal PointWithStaticMethod2(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public int X
        { get { return x; } set { x = value; } }
        public int Y
        { get { return y; } }

        public static PointWithStaticMethod2 Create(int x, int y)
        {
            return new PointWithStaticMethod2(x, y);
        }
    }

    public class PointWithStaticMethodTypeConverter2 : TypeConverter
    {
        public PointWithStaticMethodTypeConverter2()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is PointWithStaticMethod2)
            {
                PointWithStaticMethod2 pt = (PointWithStaticMethod2)value;

                MethodInfo mi = typeof(PointWithStaticMethod2).GetMethod("Create");
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[] { pt.X, pt.Y }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class SimpleClass1
    {
        int a;
        string b;
        public SimpleClass1(int aval, string bval)
        {
            a = aval;
            b = bval;
        }

        public int A { get { return a; } }
        public string B { get { return b; } }
    }

    public class SimpleClass2
    {
        [TypeConverter(typeof(SimpleClassConverter))]
        public SimpleClass1 B { get; set; }
        public string A { get; set; }
    }

    public class SimpleClassConverter : TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(InstanceDescriptor)) return true;
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor)) return true;
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new Exception("Shouldn't get here");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new Exception("Converting to: " + destinationType.ToString());
        }
    }

    [TypeConverter(typeof(FoosConverter))]
    public class Foos
    {
        public int A
        { get; set; }
        public int B
        { get; set; }
    }

    public class BarsComeFromFoos
    {
        public BarsComeFromFoos(Foos source)
        {
            Source = source;
        }

        [ConstructorArgument("source")]
        public Foos Source
        { get; set; }
    }

    public class FoosConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {
                return new FoosHelperExtension((Foos)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class FoosHelperExtension : MarkupExtension
    {
        public FoosHelperExtension()
        {
        }

        internal FoosHelperExtension(Foos foo)
        {
            SomeA = foo.A;
            SomeB = foo.B;
        }

        public int SomeA
        { get; set; }
        public int SomeB
        { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Foos { A = SomeA, B = SomeB };
        }
    }

    public class ContainerOfIComparable
    {
        public IComparable X { get; set; }
    }

    public class DummyIComparableImpl : IComparable
    {
        public int CompareTo(object obj)
        {
            return 0;
        }

        public string Y { get; set; }
    }

    [ContentProperty("Data")]
    public class ClassWithListPropertyAsCPA
    {
        List<string> data = new List<string>();

        public List<string> Data
        { get { return data; } }
    }

    public class Zoo
    {
        List<Animal> animals;

        public Zoo()
        {
            this.animals = new List<Animal>();
        }
        [TypeConverter(typeof(AnimalCollectionConverter))]
        public List<Animal> Animals
        {
            get { return animals; }
            set { this.animals = value; }
        }
    }
    [ContentProperty("Animals")]
    public class Zoo2
    {
        List<Animal> animals;

        public Zoo2()
        {
            this.animals = new List<Animal>();
        }
        [TypeConverter(typeof(AnimalCollectionConverter))]
        public List<Animal> Animals
        {
            get { return animals; }
            set { this.animals = value; }
        }
    }

    public class Zoo3
    {
        List<Animal> animals;

        public Zoo3()
        {
            this.animals = new List<Animal>();
        }
        
        [TypeConverter(typeof(AnimalCollectionConverter))]
        public List<Animal> Animals
        {
            get { return animals; }
        }
    }

    public class Animal
    {
        string name;
        int number;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
    }

    public class AnimalCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                List<Animal> animals = new List<Animal>();
                string[] animalStrings = (value as string).Split(new char[] { '#', '$' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string animalString in animalStrings)
                {
                    string[] animalInfo = animalString.Split(':');
                    Animal animal = new Animal();
                    animal.Name = animalInfo[0];
                    animal.Number = Int32.Parse(animalInfo[1]);
                    animals.Add(animal);
                }

                return animals;
            }
            else
            {
                throw new ArgumentException("In ConvertFrom: can not convert to List<Animal>.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is List<Animal>)
            {
                List<Animal> animals = value as List<Animal>;
                StringBuilder text = new StringBuilder();

                for (int i = 0; i < animals.Count; i++)
                {
                    Animal animal = animals[i];
                    text.Append(animal.Name + ":" + animal.Number);

                    if (i != animals.Count - 1)
                    {
                        text.Append('#');
                    }
                }
                return text.ToString();
            }
            else
            {
                throw new ArgumentException(string.Format("In ConvertTo: can not convert type '{0}' to string.", value.GetType().AssemblyQualifiedName));
            }
        }
    }

    [ContentProperty("Data")]
    public class ClassWithDictionaryPropertyAsCPA
    {
        Dictionary<XNameToy, string> data = new Dictionary<XNameToy, string>();

        public Dictionary<XNameToy, string> Data
        { get { return data; } }
    }

    public class Outer
    {
        public class Inner
        {
        }

        public Outer.Inner InnerMember { get; set; }
    }

    public class InnerChild : Outer.Inner
    {
    }

    public class SimpleGenericType<T>
    {
        public T Info1
        { get; set; }
    }

    public class GenericType<T1, T2, T3>
    {
        public T1 Info1
        { get; set; }
        public T2 Info2
        { get; set; }
        public T3 Info3
        { get; set; }
    }


    public static class ZooRUs
    {
        static readonly AttachableMemberIdentifier animalsName = new AttachableMemberIdentifier(typeof(ZooRUs), "Animals");

        [TypeConverter(typeof(AnimalCollectionConverter))]
        public static List<Animal> GetAnimals(object target)
        {
            List<Animal> animals;
            return AttachablePropertyServices.TryGetProperty(target, animalsName, out animals) ? animals : null;
        }

        public static void SetAnimals(object target, List<Animal> animals)
        {
            AttachablePropertyServices.SetProperty(target, animalsName, animals);
        }
    }

    [ContentProperty("Data")]
    public class ContentPropertyNameTestClass
    {
        public int Data
        { get; set; }
    }

    public class StructWrapper
    {
        static readonly AttachableMemberIdentifier aName = new AttachableMemberIdentifier(typeof(StructWrapper), "A");

        public object Value
        { get; set; }

        public static int GetA(object target)
        {
            int value;
            if (AttachablePropertyServices.TryGetProperty(target, aName, out value))
            {
                return value;
            }
            return 0;
        }
        public static void SetA(object target, int value)
        {
            AttachablePropertyServices.SetProperty(target, aName, value);
        }
    }

    public class TypeContainer
    {
        public Type Type { get; set; }
    }

    public class ObjectContainer
    {
        public object O { get; set; }
    }

    [ContentProperty("Loader")]
    public class CargoShip
    {
        IXmlSerializable loader;
        string cargo;

        public IXmlSerializable Loader
        {
            get
            {
                if (this.loader == null)
                {
                    this.loader = new Serializable(this);
                }
                return this.loader;
            }
        }

        public string Cargo
        { get { return this.cargo; } }

        class Serializable : IXmlSerializable
        {
            CargoShip ship;

            public Serializable(CargoShip ship)
            {
                this.ship = ship;
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                var sb = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    Indent = true,
                    OmitXmlDeclaration = true,
                };
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    // Read past the initial node
                    reader.Read();
                    while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
                    {
                        writer.WriteNode(reader, true);
                    }
                }
                this.ship.cargo = sb.ToString();
            }

            public void WriteXml(XmlWriter writer)
            {
                if (String.IsNullOrEmpty(this.ship.cargo))
                {
                    throw new NotImplementedException();
                }
                var settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                };
                using (var reader = XmlReader.Create(new StringReader(this.ship.cargo), settings))
                {
                    while (!reader.EOF)
                    {
                        writer.WriteNode(reader, true);
                    }
                }
            }
        }
    }

    [XmlLangProperty("Culture")]
    public class XmlLangTestClass
    {
        public string Culture { get; set; }
    }

    [TypeConverter(typeof(AbstractBaseClassTC))]
    public abstract class AbstractBaseClasswithTC
    {
    }

    internal class InternalDerivedClass : AbstractBaseClasswithTC
    {
    }

    public class AbstractBaseClassTC : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string v = value as string;
            if (v == "InternalDerivedClassName")
            {
                return new InternalDerivedClass();
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return "InternalDerivedClassName";
            }
            return null;
        }
    }

    public class ContainerClassForInternalTypeTest
    {
        AbstractBaseClasswithTC prop = new InternalDerivedClass();
        public AbstractBaseClasswithTC Prop
        {
            get
            {
                return prop;
            }
            set
            {
                prop = value;
            }
        }
    }

    [TypeConverter(typeof(UsesContextInstanceConverter))]
    public class UsesContextInstance
    {
        public string Value { get; set; }

        internal bool CanConvertToString()
        {
            return false;
        }
    }

    public class UsesContextInstanceConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                UsesContextInstance obj = context.Instance as UsesContextInstance;
                if (obj != null)
                {
                    return obj.CanConvertToString();
                }
                return true;
            }
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                UsesContextInstance obj = value as UsesContextInstance;
                if (obj != null)
                {
                    return obj.Value;
                }
            }
            throw new NotSupportedException();
        }
    }

    public class ReadWriteGenericDictionaryContainer
    {
        public Dictionary<int, string> GenericDict { get; set; }
    }

    //public class GenericContainerType2<T1, T2, T3>
    //{
    //    public GenericType2<T1[], T2[][], T3[][][]> Infos { get; set; }
    //}

    /*
    public class ContentPropertyTestBase
    {
        public object ATest
        { get; set; }
    }

    [ContentProperty("ATest")]
    public class ContentPropertyTestDerived : ContentPropertyTestBase
    {
    }

    public class PatternSequence
    {
        ArrayList list = new ArrayList();

        public PatternSequence()
        {
        }

        public void Add(object item)
        {
            list.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var obj in list)
            {
                yield return obj;
            }
        }
    }

    // For testing run-time name property on non-string type member
    [RuntimeNameProperty("Name")]
    public class FooBad
    {
        public FooBad()
        {
        }
        public Guid Name
        { get; set; }
        public int SomeVal
        { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class RuntimeNamePropertyNameTestClass
    {
        public int Name
        { get; set; }
    }

    public class StringProps
    {
        public string A
        { get; set; }
        public string B
        { get; set; }
        public string C
        { get; set; }
    }
     
    [TypeConverter(typeof(TigerConverter))]
    public struct Tiger
    {
        string name;
        public Tiger(string name)
        {
            this.name = name;
        }
        public string NickName
        {
            get { return this.name; }
        }
    }
    public class TigerConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Tiger(value as string);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Tiger)
            {
                return ((Tiger)value).NickName;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }
    public struct ClassType1
    {
        public string Category
        { get; set; }
    }
    public class ClassType2
    {
        public ClassType1 Category
        { get; set; }
    }
    public class ClassType3
    {
    }
    public struct ClassType4
    {
    }
    public class ClassType5
    {
        public ClassType5()
        {
            this.Field1 = new ClassType1 { Category = "Next Gen" };
            this.Field2 = new ClassType2 { Category = new ClassType1() };
            this.Field3 = new ClassType3();
            this.Field4 = new ClassType4();
        }

        public ClassType1 Field1
        { get; set; }
        public ClassType2 Field2
        { get; set; }
        public ClassType3 Field3
        { get; set; }
        public ClassType4 Field4
        { get; set; }
    }

    public class SimpleClassWithUnparsedMembers
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public System.Xaml.XamlReader SingleValueUnparsed
        { get; set; }
    }

    
    public class NoDefaultCtor
    {
        public NoDefaultCtor(string a)
        {
        }
        public string A
        { get; set; }
    }

    public class Outer
    {
        public string A
        { get; set; }

        public class Inner
        {
            public string B
            { get; set; }
        }
    }

    public class Item
    {
        public Item()
        {
        }

        public Item(string itemName, double price)
        {
            ItemName = itemName;
            Price = price;
        }
        public string ItemName
        { get; set; }
        public double Price
        { get; set; }
    }

    public class CollectionContainerType3
    {
        Item[] items = new Item[3];

        public CollectionContainerType3()
        {
        }

        public Item[] Items { get { return this.items; } }
    }

    public struct NestedStruct
    {
        public struct InnerStruct
        {
            public List<object> listEmptyStruct;

            public int[] varrint;

            public InnerStruct(bool set)
            {
                varrint = new int[5] { 0, 1, 2, 3, 4 };
                listEmptyStruct = new List<object>();
            }
        }
    }

    public class TypeContaingingIXmlSerializableProperty
    {
        public string data;
        SimpleIXmlSerializable loader;

        public SimpleIXmlSerializable XmlProperty
        {
            get
            {
                if (this.loader == null)
                {
                    this.loader = new SimpleIXmlSerializable(this);
                }
                return this.loader;
            }
        }
    }

    public class SimpleIXmlSerializable : IXmlSerializable
    {
        TypeContaingingIXmlSerializableProperty Data;

        public SimpleIXmlSerializable(TypeContaingingIXmlSerializableProperty data)
        {
            Data = data;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    Indent = true,
                    OmitXmlDeclaration = true,
                };
            using (var writer = XmlWriter.Create(stringBuilder, settings))
            {
                while (!reader.EOF)
                {
                    writer.WriteNode(reader, true);
                    writer.Flush();
                }
            }
            this.Data.data = stringBuilder.ToString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var settings = new XmlReaderSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto
                };
            using (var reader = XmlReader.Create(new StringReader(Data.data), settings))
            {
                while (!reader.EOF)
                {
                    writer.WriteNode(reader, true);
                }
            }
        }
    }

    public class PersonWithEnumProperty
    {
        public string Name
        { get; set; }
        public AgeEnum Age
        { get; set; }
    }

    public enum AgeEnum
    {
        Old,
        Venerable,
        Ancient
    }

    public class UsesNested
    {
        [TypeConverter(typeof(NestedConverter))]
        public NestedBase MyNestedType
        { get; set; }
    }

    public class ParentOfNested
    {
        public class Nested : NestedBase
        {
            public string MyString
            { get; set; }
        }
    }

    public class NestedBase
    {
    }

    public class NestedConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return new ParentOfNested.Nested() { MyString = (string)value };
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() != typeof(ParentOfNested.Nested))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return ((ParentOfNested.Nested)value).MyString;
        }
    }


    public class UsesNestedPrivate
    {
        public UsesNestedPrivate()
        {
            MyNestedType = new Nested() { MyString = "string on nested type" };
        }

        [TypeConverter(typeof(NestedConverterPrivate))]
        public NestedBasePrivate MyNestedType
        { get; set; }

        static public NestedBasePrivate CreateNested(string nestedString)
        {
            return new Nested() { MyString = nestedString };
        }

        private class Nested : NestedBasePrivate
        {
        }
    }

    public class NestedBasePrivate
    {
        public string MyString
        { get; set; }
    }

    public class NestedConverterPrivate : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return UsesNestedPrivate.CreateNested((string)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType().IsAssignableFrom(typeof(NestedBasePrivate)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return ((NestedBasePrivate)value).MyString;
        }
    }
    */

    public class HasOneElement : Element
    {
        public Element Element { get; set; }
    }

    public class BadGetter
    {
        public string Foo
        {
            get { throw new ArgumentException(); }
            set { throw new ArgumentException(); }
        }
    }

    public class BaseWithVirtualProperties
    {
        public BaseWithVirtualProperties(string foo)
        {
            Foo = foo;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string HasVisibility { get; set; }

        [DefaultValue(null)]
        public virtual string HasDefaultValue { get; set; }

        [ConstructorArgument("foo")]
        public virtual string Foo { get; private set;  }
    }

    public class DerivedWithOverridenProperties : BaseWithVirtualProperties
    {
        public DerivedWithOverridenProperties(string foo)
            :base(foo)
        {
        }

        public override string HasVisibility { get; set; }

        public override string HasDefaultValue { get; set; }

        public override string Foo { get { return base.Foo; } }
    }
}

namespace Test.Elements.Derived
{
    public class DerivedElement : HasOneElement
    {
    }
}
