using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using BtcTransmuter.Extension.NBXplorer.Models;
using NBitcoin;
using NBXplorer;
using NBXplorer.Models;

namespace BtcTransmuter.Extension.NBXplorer.Services
{
    public class NBXplorerSummaryProvider
    {
        private readonly NBXplorerOptions _options;

        private readonly ConcurrentDictionary<string, NBXplorerSummary>
            _summaries = new ConcurrentDictionary<string, NBXplorerSummary>();

        public NBXplorerSummaryProvider(NBXplorerOptions options)
        {
            _options = options;
        }

        public ImmutableDictionary<string, NBXplorerSummary> GetSummaries()
        {
            return _summaries.ToImmutableDictionary(pair => pair.Key, pair => pair.Value);
        }

        public NBXplorerSummary GetSummary(string cryptoCode)
        {
            return _summaries.TryGet(cryptoCode);
        }

        public async Task UpdateClientState(ExplorerClient client, CancellationToken cancellation)
        {
            var state = (NBXplorerState?) null;
            string error = null;
            StatusResult status = null;
            try
            {
                status = await client.GetStatusAsync(cancellation);
                if (status == null)
                {
                    state = NBXplorerState.NotConnected;
                }
                else if (status.IsFullySynched)
                {
                    state = NBXplorerState.Ready;
                }else if (!status.IsFullySynched)
                {
                    state = NBXplorerState.Synching;
                }
            }
            catch (Exception ex) when (!cancellation.IsCancellationRequested)
            {
                error = ex.Message;
            }

            if (status != null && error == null && status.NetworkType != _options.NetworkType)
            {
                    error =
                        $"{client.CryptoCode}: NBXplorer is on a different ChainType (actual: {status.NetworkType}, expected: {_options.NetworkType})";
            }

            if (error != null)
            {
                state = NBXplorerState.NotConnected;
            }

            _summaries.AddOrReplace(client.CryptoCode, new NBXplorerSummary()
            {
                Status = status,
                State = state.GetValueOrDefault(NBXplorerState.NotConnected),
                Error = error
            });
        }
    }
}