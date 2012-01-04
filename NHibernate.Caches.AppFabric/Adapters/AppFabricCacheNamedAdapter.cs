#region License

//Microsoft Public License (Ms-PL)
//
//This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
//
//1. Definitions
//
//The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
//
//A "contribution" is the original software, or any additions or changes to the software.
//
//A "contributor" is any person that distributes its contribution under this license.
//
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
//2. Grant of Rights
//
//(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
//
//(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
//
//3. Conditions and Limitations
//
//(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
//
//(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
//
//(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
//
//(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
//
//(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

#endregion

using Microsoft.ApplicationServer.Caching;

namespace NHibernate.Caches.AppFabric.Adapters
{
    /// <summary>
    /// An implementation of the the <see cref="AppFabricCacheAdapter"/> that uses an AppFabric named cache
    /// for each NHibernate cache region.
    /// </summary>
    public class AppFabricCacheNamedAdapter : AppFabricCacheAdapter
    {
        #region Constructor

        /// <summary>
        /// Creates and instance of the class.
        /// </summary>
        /// <param name="regionName">The name of the NHibernate cache region this adapter is for.</param>
        public AppFabricCacheNamedAdapter(string regionName)
            : base(regionName)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Data is cached within a region within the named cache this adapter is for. The name of this region is configurable
        /// in App/Web.config.
        /// </summary>
        protected internal override string AppFabricRegionName
        {
            get
            {
                return AppFabricProviderSettings.Settings.NamedCacheTypeRegionName;
            }
        }

        #endregion

        #region Methods

        protected internal override DataCache GetCache(IAppFabricCacheFactory cacheFactory)
        {
            return cacheFactory.GetCache(RegionName, !AppFabricProviderSettings.Settings.NamedCachesMustExist);
        }

        #endregion
    }
}
