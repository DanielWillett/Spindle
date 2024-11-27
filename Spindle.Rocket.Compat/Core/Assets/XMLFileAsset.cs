using Rocket.API;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Rocket.Core.Assets;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class XMLFileAsset<T> : Asset<T> where T : class, IDefaultable
{
    private readonly XmlSerializer _serializer;
    private readonly string _file;
    private readonly T _defaultInstance;

    public XMLFileAsset(string file, Type[] extraTypes = null, T defaultInstance = null)
    {
        _serializer = new XmlSerializer(typeof(T), extraTypes ?? Type.EmptyTypes);
        _file = file;
        _defaultInstance = defaultInstance;
        Load(null);
    }

    public override T Save()
    {
        try
        {
            string directoryName = Path.GetDirectoryName(_file);
            if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            using StreamWriter streamWriter = new StreamWriter(_file);
            if (instance == null)
            {
                if (_defaultInstance == null)
                {
                    instance = Activator.CreateInstance<T>();
                    instance.LoadDefaults();
                }
                else
                    instance = _defaultInstance;
            }
            _serializer.Serialize(streamWriter, instance);
            return instance;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to serialize XMLFileAsset: {_file}", ex);
        }
    }

    public override void Load(AssetLoaded<T> callback = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(_file) && File.Exists(_file))
            {
                using StreamReader streamReader = new StreamReader(_file);
                instance = (T)_serializer.Deserialize(streamReader);
            }
            Save();
            if (callback == null)
                return;
            callback(this);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to deserialize XMLFileAsset: {_file}", ex);
        }
    }
}