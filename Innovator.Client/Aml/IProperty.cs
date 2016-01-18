using System;
namespace Innovator.Client
{
  public interface IReadOnlyProperty : IReadOnlyElement
  {
    /// <summary>Value converted to a nullable boolean.  
    /// If the value cannot be converted, an exception is thrown</summary>
    bool? AsBoolean();
    /// <summary>Value converted to a boolean using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    bool AsBoolean(bool defaultValue);
    /// <summary>Value converted to a nullable DateTime in the local timezone.  
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime? AsDateTime();
    /// <summary>Value converted to a DateTime in the local timezone using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime AsDateTime(DateTime defaultValue);
    /// <summary>Value converted to a nullable DateTime in the UTC timezone.  
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime? AsDateTimeUtc();
    /// <summary>Value converted to a DateTime in the UTC timezone using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime AsDateTimeUtc(DateTime defaultValue);
    /// <summary>Value converted to a nullable double.  
    /// If the value cannot be converted, an exception is thrown</summary>
    double? AsDouble();
    /// <summary>Value converted to a double using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    double AsDouble(double defaultValue);
    /// <summary>Value converted to a nullable Guid.  
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid? AsGuid();
    /// <summary>Value converted to a Guid using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid AsGuid(Guid defaultValue);
    /// <summary>Value converted to a nullable int.  
    /// If the value cannot be converted, an exception is thrown</summary>
    int? AsInt();
    /// <summary>Value converted to a int using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    int AsInt(int defaultValue);
    /// <summary>Value converted to a nullable long.  
    /// If the value cannot be converted, an exception is thrown</summary>
    long? AsLong();
    /// <summary>Value converted to a long using the default value if null.  
    /// If the value cannot be converted, an exception is thrown</summary>
    long AsLong(long defaultValue);
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' (.Exists = false) item is returned</summary>
    IReadOnlyItem AsItem();
    /// <summary>Value converted to a string using the default value if null.</summary>
    string AsString(string defaultValue);
  }

  public interface IProperty : IReadOnlyProperty, IElement
  {
    new IItem AsItem();
    void Set(object value);
  }
}
