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

using System.Configuration;

namespace NHibernate.Caches.AppFabric
{
    /// <summary>
    /// Enacpsulates the AppFabric provider configuration settings.
    /// </summary>
    public class AppFabricProviderSettings : ConfigurationSection
    {
        #region Constants

        private const string SectionName = "AppFabricProviderSettings";

        private const string CacheTypeSetting                = "CacheType";
        private const string RegionCacheTypeCacheNameSetting = "RegionCacheTypeCacheName";
        private const string NamedCacheTypeRegionNameSetting = "NamedCacheTypeRegionName";
        private const string NamedCachesMustExistSetting     = "NamedCachesMustExist";
        private const string LockTimeoutSetting              = "LockTimeout";
        private const string LocksRegionNameSetting          = "LocksRegionName";
        private const string SerializationProviderSetting    = "SerializationProvider";

        private const int DefaultTimeout = 30000;

        private const string DefaultCacheName       = "nhibernate";
        private const string DefaultRegionName      = "nhibernate";
        private const string DefaultLocksRegionName = "Locks";

        #endregion

        #region Class variables

        private static AppFabricProviderSettings settings = ConfigurationManager.GetSection(SectionName) as AppFabricProviderSettings;

        #endregion

        #region Properties

        /// <summary>
        /// The settings.
        /// </summary>
        public static AppFabricProviderSettings Settings
        {
            get
            {
                return settings;
            }
        }

        /// <summary>
        /// The type of app fabric cache provider to create. For example Named where an AppFabric named cache is used
        /// for each NHibernate cache region, or Region where an AppFabric region is used for each NHibernate cache region.
        /// </summary>
        [ConfigurationProperty(CacheTypeSetting, DefaultValue = AppFabricCacheAdapterType.Region, IsRequired = false)]
        public AppFabricCacheAdapterType CacheType
        {
            get
            {
                return (AppFabricCacheAdapterType)this[CacheTypeSetting];
            }
            set
            {
                this[CacheTypeSetting] = value;
            }
        }

        /// <summary>
        /// If the cache type is configured as Region, this setting defines the AppFabric named cache that the regions will be
        /// created in.
        /// </summary>
        [ConfigurationProperty(RegionCacheTypeCacheNameSetting, DefaultValue = DefaultCacheName, IsRequired = false)]
        public string RegionCacheTypeCacheName
        {
            get
            {
                return (string)this[RegionCacheTypeCacheNameSetting];
            }
            set
            {
                this[RegionCacheTypeCacheNameSetting] = value;
            }
        }

        /// <summary>
        /// If the cache type is configured as Named, this setting defnes the AppFabric region that the data is cached in,
        /// within the AppFabric named caches.
        /// </summary>
        [ConfigurationProperty(NamedCacheTypeRegionNameSetting, DefaultValue = DefaultRegionName, IsRequired = false)]
        public string NamedCacheTypeRegionName
        {
            get
            {
                return (string)this[NamedCacheTypeRegionNameSetting];
            }
            set
            {
                this[NamedCacheTypeRegionNameSetting] = value;
            }
        }

        /// <summary>
        /// NHibernate uses some internal regions, for example, for caching queries. If the cache type is configured as Named,
        /// this setting specifes whether all regions (AppFabric named caches) must exist or whether it will fail over and use
        /// the default cache if a named cache does not exist.
        /// </summary>
        [ConfigurationProperty(NamedCachesMustExistSetting, DefaultValue = false, IsRequired = false)]
        public bool NamedCachesMustExist
        {
            get
            {
                return (bool)this[NamedCachesMustExistSetting];
            }
            set
            {
                this[NamedCachesMustExistSetting] = value;
            }
        }

        /// <summary>
        /// Specifies the timout in milliseconds when locking items in the cache.
        /// </summary>
        [IntegerValidator(MinValue = 0)]
        [ConfigurationProperty(LockTimeoutSetting, DefaultValue = DefaultTimeout, IsRequired = false)]
        public int LockTimeout
        {
            get
            {
                return (int)this[LockTimeoutSetting];
            }
            set
            {
                this[LockTimeoutSetting] = value;
            }
        }

        /// <summary>
        /// The provider needs to store the locks in a separate named cache. This setting defines the name of that cache.
        /// </summary>
        [ConfigurationProperty(LocksRegionNameSetting, DefaultValue = DefaultLocksRegionName, IsRequired = false)]
        public string LocksRegionName
        {
            get
            {
                return (string)this[LocksRegionNameSetting];
            }
            set
            {
                this[LocksRegionNameSetting] = value;
            }
        }

        /// <summary>
        /// A class that implements <see cref="ISerializationProvider"/> to be used to override the default XML serialization
        /// of AppFabric.
        /// </summary>
        [ConfigurationProperty(SerializationProviderSetting, DefaultValue = null, IsRequired = false)]
        public string SerializationProvider
        {
            get
            {
                return (string)this[SerializationProviderSetting];
            }
            set
            {
                this[SerializationProviderSetting] = value;
            }
        }

        #endregion
    }
}
