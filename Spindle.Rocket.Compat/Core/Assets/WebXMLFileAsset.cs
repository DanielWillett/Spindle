using Rocket.API;
using Rocket.Core.Utils;
using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace Rocket.Core.Assets;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class WebXMLFileAsset<T> : Asset<T> where T : class
{
    private readonly RocketWebClient _webClient = new RocketWebClient();
    private readonly XmlRootAttribute _rootAttribute;
    private readonly Uri _url;

    private DownloadStringCompletedEventHandler _handler = (_, _) => { };
    private bool _isWaiting;

    public WebXMLFileAsset(Uri url = null, XmlRootAttribute attr = null, AssetLoaded<T> callback = null)
    {
        _url = url;
        _rootAttribute = attr;
        
        // ReSharper disable once VirtualMemberCallInConstructor
        Load(callback);
    }

    public override void Load(AssetLoaded<T> callback = null)
    {
        try
        {
            if (this._isWaiting)
                return;
            Logger.Log($"Updating WebXMLFileAsset {typeof(T).Name} from {_url}");
            _isWaiting = true;
            _webClient.DownloadStringCompleted -= _handler;
            _handler = (_, e) =>
            {
                if (e.Error != null)
                {
                    Logger.Log($"Error retrieving WebXMLFileAsset {typeof(T).Name} from {_url}: {e.Error.Message}");
                }
                else
                {
                    try
                    {
                        using StringReader stringReader = new StringReader(e.Result);
                        T result = (T)new XmlSerializer(typeof(T), _rootAttribute).Deserialize(stringReader);
                        if (result != null)
                        {
                            TaskDispatcher.QueueOnMainThread(() =>
                            {
                                this.instance = result;
                                Logger.Log($"Successfully updated WebXMLFileAsset {typeof(T).Name} from {_url}");
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error retrieving WebXMLFileAsset {typeof(T).Name} from {_url}: {ex.Message}");
                    }
                }

                TaskDispatcher.QueueOnMainThread(() =>
                {
                    AssetLoaded<T> assetLoaded = callback;
                    if (assetLoaded != null)
                        assetLoaded(this);
                    _isWaiting = false;
                });
            };
            _webClient.DownloadStringCompleted += _handler;
            _webClient.DownloadStringAsync(_url);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error retrieving WebXMLFileAsset {typeof(T).Name} from {_url}: {ex.Message}");
        }
    }
}