#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Threading;
using System.Diagnostics;

namespace ClearCanvas.Ris.Client
{
    public class RemoteSuggestionProvider<TItem> : SuggestionProviderBase<TItem>
    {
        #region State classes (state pattern implementation)

        abstract class State
        {
            protected readonly RemoteSuggestionProvider<TItem> _owner;
            public State(RemoteSuggestionProvider<TItem> owner)
            {
                _owner = owner;
            }

            public abstract void GetShortList(string text);
            public abstract void OnRequestCompleted(bool success, TItem[] results);
        }

        class AsyncDefaultState : State
        {
            public AsyncDefaultState(RemoteSuggestionProvider<TItem> owner)
                :base(owner)
            {
            }

            public override void GetShortList(string text)
            {
                //if(_owner.IsRefinementOfLastRequestText(text))
                //{
                //    _owner.PostItemsMatching(text);
                //}
                //else
                //{
                    // not a refinement of the previous query (or there was no previous query)
                    // try to begin a new request
                    _owner._shortList = null;   // clear the shortlist
                    if (_owner.BeginRequest(text))
                    {
                        // modify state
                        _owner._state = new RequestPendingState(_owner, text);
                    }
                //}
            }

            public override void OnRequestCompleted(bool success, TItem[] results)
            {
                // should never get here in AsyncDefaultState
                throw new Exception("The method or operation is not implemented.");
            }
            
        }

        class RequestPendingState : State
        {
            private readonly string _query;

            public RequestPendingState(RemoteSuggestionProvider<TItem> owner, string query)
                :base(owner)
            {
                _query = query;
            }

            private void TryNewRequest(string text)
            {
                if (_owner.BeginRequest(text))
                {
                    _owner._state = new RequestPendingState(_owner, text);
                }
                else
                {
                    _owner._state = new AsyncDefaultState(_owner);
                }
            }

            public override void GetShortList(string text)
            {
                // do nothing while a request is pending
            }

            public override void OnRequestCompleted(bool success, TItem[] results)
            {
                if(success)
                {
                    // set the shortlist to the results of the request, and store the query used to obtain the results
                    _owner._shortList = new List<TItem>(results);
                    _owner._lastSuccessfulRequestQueryText = _query;

                    // also need to sort the shortlist for presentation in the UI
                    _owner._shortList.Sort(delegate(TItem x, TItem y) { return _owner.FormatItem(x).CompareTo(_owner.FormatItem(y)); });

                    // if current query text is equal to, or a refinement of, request query text,
                    // then consider the request successful and post the matching items
                    if(_owner.IsRefinementOfLastRequestText(_owner._currentQueryText))
                    {
                        _owner.UpdateSuggestions();
                        _owner._state = new AsyncDefaultState(_owner);
                    }
                    else
                    {
                        // the query text has changed, so we need to issue a new request
                        TryNewRequest(_owner._currentQueryText);
                    }
                }
                else
                {
                    // the request was not successful
                    // if the query text has changed we need to issue a new request immediately
                    if (_owner._currentQueryText != _query)
                    {
                        TryNewRequest(_owner._currentQueryText);
                    }
                    else
                    {
                        // otherwise just return to default state
                        _owner._state = new AsyncDefaultState(_owner);
                    }
                }
            }
        }

        #endregion


        public delegate bool RemoteQueryDelegate<T>(string query, out T[] results);

        private readonly RemoteQueryDelegate<TItem> _remoteQueryCallback;
        private readonly Converter<TItem, string> _formatHandler;

        private List<TItem> _shortList;
        private string _currentQueryText;
        private string _lastSuccessfulRequestQueryText;
        private BackgroundTask _requestTask;
        private State _state;

        public RemoteSuggestionProvider(RemoteQueryDelegate<TItem> remoteQueryCallback, Converter<TItem, string> formatHandler)
        {
            _remoteQueryCallback = remoteQueryCallback;
            _formatHandler = formatHandler;
            _state = new AsyncDefaultState(this);
        }

        protected override List<TItem> GetShortList(string query)
        {
            _state.GetShortList(query);
            _currentQueryText = query;
            return _shortList;
        }

        protected override bool IsQueryRefinement(string query, string previousQuery)
        {
            // don't care about the previousQuery, but rather, the last query that generated an initial shortlist
            return IsRefinementOfLastRequestText(query);
        }

        protected override string FormatItem(TItem item)
        {
            return _formatHandler(item);
        }

        private bool BeginRequest(string text)
        {
            if (!IsUsefulQueryText(text))
                return false;

            _requestTask = new BackgroundTask(
                delegate (IBackgroundTaskContext context)
                    {
                        try
                        {
                            string queryText = (string)context.UserState;
                            TItem[] results;
                            bool success = _remoteQueryCallback(queryText, out results);
                            context.Complete(success, results);
                        }
                        catch (Exception e)
                        {
                            context.Error(e);
                        }
                    }, false, text);
            _requestTask.Terminated += _requestTask_Terminated;
            _requestTask.Run();

            return true;
        }

        private void _requestTask_Terminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            if(e.Reason == BackgroundTaskTerminatedReason.Exception)
            {
                // not much we can do about it, except log it and return an empty list
                Platform.Log(LogLevel.Error, e.Exception);
                _state.OnRequestCompleted(false, new TItem[]{});
            }
            else
            {
                bool success = (bool)e.Results[0];
                TItem[] results = (TItem[])e.Results[1];

                _state.OnRequestCompleted(success, results);
            }
        }

        private bool IsUsefulQueryText(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Trim().Length > 0;
        }

        private bool IsRefinementOfLastRequestText(string text)
        {
            return !string.IsNullOrEmpty(_lastSuccessfulRequestQueryText) && text.StartsWith(_lastSuccessfulRequestQueryText);
        }
    }
}
