using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace Aras.Tools.InnovatorAdmin
{
  public static class Iom
  {
    private static string _defaultVersion;
    private static Dictionary<string, string> _versionPaths = new Dictionary<string,string>();
    private static Dictionary<string, IConnectionFactory> _versions = new Dictionary<string, IConnectionFactory>();

    static Iom() 
    {
      var versionData = (from f in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                        let v = FileVersionInfo.GetVersionInfo(f)
                        where v.OriginalFilename == "IOM.dll"
                        orderby v.ProductMajorPart, v.ProductMinorPart, v.ProductBuildPart, v.ProductPrivatePart
                        select Tuple.Create(v.ProductVersion, f));

      foreach (var version in versionData)
      {
        if (string.IsNullOrEmpty(_defaultVersion)) _defaultVersion = version.Item1;
        _versionPaths[version.Item1] = version.Item2;
      }
    }

    public static IConnectionFactory GetFactory(string version)
    {
      IConnectionFactory factory;
      if (string.IsNullOrEmpty(version)) version = _defaultVersion;
      if (!_versions.TryGetValue(version, out factory))
      {
        if (!_versionPaths.ContainsKey(version))
          throw new ArgumentException("IOM is not available for the specified version");

        var domain = AppDomain.CreateDomain("IOM_" + version);
        domain.AssemblyResolve += domain_AssemblyResolve;
        factory = (IConnectionFactory)domain.CreateInstanceAndUnwrap("IomWrapper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "IomWrapper.IomConnectionFactory");
        _versions[version] = factory;
      }
      
      return factory;
    }

    static System.Reflection.Assembly domain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      var domain = (AppDomain)sender;
      var version = domain.FriendlyName.Substring(4);
      var path = _versionPaths[version];
      return Assembly.LoadFrom(path);
    }

    public static IEnumerable<string> Versions()
    {
      return _versionPaths.Keys;
    }

    ///// <summary>
    ///// Wraps an instance of TInterface. If the instance is a 
    ///// MarshalByRefObject, this class acts as a sponsor for its lifetime 
    ///// service (until disposed/finalized). Disposing the sponsor implicitly 
    ///// disposes the instance.
    ///// </summary>
    ///// <typeparam name="TInterface">
    ///// </typeparam>
    //[Serializable]
    //[SecurityPermission(SecurityAction.Demand, Infrastructure = true)]
    //public sealed class Sponsor<TInterface> : ISponsor, IDisposable
    //    where TInterface : class
    //{ 
    //    #region Fields

    //    private TInterface instance;

    //    #endregion

    //    #region Constructors and Destructors

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Sponsor{TInterface}"/> class. 
    //    /// Initialises a new instance of the Sponsor&lt;TInterface&gt; class, 
    //    /// wrapping the specified object instance.
    //    /// </summary>
    //    /// <param name="instance">
    //    /// </param>
    //    public Sponsor(TInterface instance)
    //    {
    //        this.Instance = instance;

    //        var refObject = this.Instance as MarshalByRefObject;
    //        if (refObject != null)
    //        {
    //            var lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)this.Instance);
    //            var service = lifetimeService as ILease;
    //            if (service != null)
    //            {
    //                var lease = service;
    //                lease.Register(this);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Finalizes an instance of the <see cref="Sponsor{TInterface}"/> class. 
    //    /// Finaliser.
    //    /// </summary>
    //    ~Sponsor()
    //    {
    //        this.Dispose(false);
    //    }

    //    #endregion

    //    #region Public Properties

    //    /// <summary>
    //    /// Gets the wrapped instance of TInterface.
    //    /// </summary>
    //    public TInterface Instance
    //    {
    //        get
    //        {
    //            if (this.IsDisposed)
    //            {
    //                throw new ObjectDisposedException("Instance");
    //            }

    //            return this.instance;
    //        }

    //        private set
    //        {
    //            this.instance = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets whether the sponsor has been disposed.
    //    /// </summary>
    //    public bool IsDisposed { get; private set; }

    //    #endregion

    //    #region Public Methods and Operators

    //    /// <summary>
    //    /// Disposes the sponsor and the instance it wraps.
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        this.Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    #endregion

    //    #region Explicit Interface Methods

    //    /// <summary>
    //    /// Renews the lease on the instance as though it has been called normally.
    //    /// </summary>
    //    /// <param name="lease">
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="TimeSpan"/>.
    //    /// </returns>
    //    TimeSpan ISponsor.Renewal(ILease lease)
    //    {
    //        if (this.IsDisposed)
    //        {
    //            return TimeSpan.Zero;
    //        }

    //        return LifetimeServices.RenewOnCallTime;
    //    }

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// Disposes the sponsor and the instance it wraps.
    //    /// </summary>
    //    /// <param name="disposing">
    //    /// </param>
    //    private void Dispose(bool disposing)
    //    {
    //        if (!this.IsDisposed)
    //        {
    //            if (disposing)
    //            {
    //                var disposable = this.Instance as IDisposable;
    //                if (disposable != null)
    //                {
    //                    disposable.Dispose();
    //                }

    //                var refObject = this.Instance as MarshalByRefObject;
    //                if (refObject != null)
    //                {
    //                    var lifetimeService =
    //                        RemotingServices.GetLifetimeService((MarshalByRefObject)(object)this.Instance);
    //                    var service = lifetimeService as ILease;
    //                    if (service != null)
    //                    {
    //                        var lease = service;
    //                        lease.Unregister(this);
    //                    }
    //                }
    //            }

    //            this.Instance = null;
    //            this.IsDisposed = true;
    //        }
    //    }

    //    #endregion
    //}
  }
}
