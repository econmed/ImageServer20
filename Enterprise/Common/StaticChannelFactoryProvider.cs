using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IChannelFactoryProvider"/> that provides
    /// channel factories based on a single statically defined primary endpoint
    /// and a single statically defined failover endpoint.
    /// </summary>
    internal class StaticChannelFactoryProvider : IChannelFactoryProvider
    {
        #region Node class

        class Node
        {
            private Uri _url;
            private DateTime _blackOutExpiry;

            public Node(string url)
            {
                _url = new Uri(url);
                _blackOutExpiry = DateTime.MinValue;
            }

            public Uri Url
            {
                get { return _url; }
            }

            public bool IsBlackedOut
            {
                get { return _blackOutExpiry > DateTime.Now; }
            }

            public void Blackout(TimeSpan howLong)
            {
                _blackOutExpiry = DateTime.Now + howLong;
            }
        }

        #endregion

        private readonly TimeSpan _blackoutPeriod = TimeSpan.FromSeconds(30);
        private readonly RemoteServiceProviderArgs _args;
        private readonly List<Node> _nodes = new List<Node>();

        public StaticChannelFactoryProvider(RemoteServiceProviderArgs args)
        {
            _args = args;

            // add a node for each URL, ensuring that the primary is the first entry in the list
            _nodes.Add(new Node(_args.BaseUrl));
            if (!string.IsNullOrEmpty(_args.FailoverBaseUrl))
            {
                _nodes.Add(new Node(_args.FailoverBaseUrl));
            }
        }

        public ChannelFactory GetPrimary(Type serviceContract)
        {
            ChannelFactory factory = GetFirstLiveChannel(serviceContract);
            if (factory != null)
                return factory;

            // if there were no live nodes in the list, 
            // best thing we can do is return the primary Uri
            return GetChannelFactory(serviceContract, new Uri(_args.BaseUrl));
        }

        public ChannelFactory GetFailover(Type serviceContract, EndpointAddress failedEndpoint)
        {
            // find the failed node and marked it as blacked out
            Node failedNode = CollectionUtils.SelectFirst(_nodes,
                delegate(Node n) { return Equals(failedEndpoint.Uri, GetFullUri(serviceContract, n.Url)); });
            failedNode.Blackout(_blackoutPeriod);

            // get the first live node
            ChannelFactory factory = GetFirstLiveChannel(serviceContract);
            if (factory != null)
                return factory;

            return null;
        }

        private ChannelFactory GetFirstLiveChannel(Type serviceContract)
        {
            // find the first non-blacked out node in the list
            Node node = CollectionUtils.SelectFirst(_nodes,
                delegate(Node n) { return !n.IsBlackedOut; });

            return node == null ? null : GetChannelFactory(serviceContract, node.Url);
        }

        private ChannelFactory GetChannelFactory(Type serviceContract, Uri baseUri)
        {
            Uri uri = GetFullUri(serviceContract, baseUri);
            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceContract });
            return _args.Configuration.ConfigureChannelFactory(
                new ServiceChannelConfigurationArgs(channelFactoryClass,
                                                    uri,
                                                    AuthenticationAttribute.IsAuthenticationRequired(serviceContract),
                                                    _args.MaxReceivedMessageSize,
                                                    _args.CertificateValidationMode,
                                                    _args.RevocationMode));
        }

        private static Uri GetFullUri(Type serviceContract, Uri baseUri)
        {
            return new Uri(baseUri, serviceContract.FullName);
        }
    }
}
