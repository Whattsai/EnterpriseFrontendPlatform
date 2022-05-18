using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace HtmlConvertTool.DataClass
{
    public class DataClass : IEqualityComparer<DataClass>, IEquatable<DataClass>

{

    public DataClass()

    {

        PropertyInfo = new PropertyInfo();

    }

    public string ParentClassName { get; set; }



    public PropertyInfo PropertyInfo { get; set; }



    public bool Equals(DataClass? x, DataClass? y)

    {

        if (x == null || y == null)

            return x == y;



        return x.ParentClassName == y.ParentClassName &&

            x.PropertyInfo.Equals(this.PropertyInfo);

    }



    public bool Equals(DataClass? other)

    {

        if (other == null || this == null)

            return other == this;



        return other.ParentClassName == this.ParentClassName &&

            other.PropertyInfo.Equals(this.PropertyInfo);

    }



    public int GetHashCode([DisallowNull] DataClass obj)

    {

        return obj == null ? 0 : (obj.PropertyInfo.GetHashCode() ^ obj.PropertyInfo.GetHashCode() ^ obj.ParentClassName.GetHashCode());

    }

}



public class PropertyInfo : IEqualityComparer<PropertyInfo>, IEquatable<PropertyInfo>

{

    public string Name { get; set; }



    // boolean、string、number、object、boolean[]、string[]、number[]、objcet[]

    public string Type { get; set; }



    public bool Equals(PropertyInfo? x, PropertyInfo? y)

    {

        if (x == null || y == null)

            return x == y;



        return x.Name == y.Name &&

            x.Type == y.Type;

    }



    public bool Equals(PropertyInfo? other)

    {

        if (other == null || this == null)

            return other == this;



        return other.Name == this.Name &&

            other.Type == this.Type;

    }



    public int GetHashCode([DisallowNull] PropertyInfo obj)

    {

        return obj == null ? 0 : (obj.Type.GetHashCode() ^ obj.Name.GetHashCode());

    }

}
}
