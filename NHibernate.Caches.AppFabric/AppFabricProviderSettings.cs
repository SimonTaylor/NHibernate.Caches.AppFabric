using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHibernate.Caches.AppFabric
{
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

        public static AppFabricProviderSettings Settings
        {
            get
            {
                return settings;
            }
        }

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
