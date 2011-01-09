// This is the main DLL file.

#include "stdafx.h"

#include "managedLibnids.h"
using namespace managedLibnids;

void managedLibnids::tcp_callback (struct tcp_stream *a_tcp, void ** this_time_not_needed)
{
  if (a_tcp->nids_state == NIDS_JUST_EST)
    {
    // connection described by a_tcp is established
    // here we decide, if we wish to follow this stream
    // sample condition: if (a_tcp->addr.dest!=23) return;
    // in this simple app we follow each stream, so..
      a_tcp->client.collect++; // we want data received by a client
      a_tcp->server.collect++; // and by a server, too
      a_tcp->server.collect_urg++; // we want urgent data received by a
                                   // server
      a_tcp->client.collect_urg++; // if we don't increase this value,
                                   // we won't be notified of urgent data
                                   // arrival
      return;
    }
  if (a_tcp->nids_state == NIDS_CLOSE)
    {
      // connection has been closed normally
      return;
    }
  if (a_tcp->nids_state == NIDS_RESET)
    {
      // connection has been closed by RST
      return;
    }

  if (a_tcp->nids_state == NIDS_DATA)
    {
      // new data has arrived; gotta determine in what direction
      // and if it's urgent or not

      struct half_stream *hlf;

      if (a_tcp->server.count_new_urg)
      {
        // new byte of urgent data has arrived
		 int len = a_tcp->server.count_new_urg;
		 array<Byte>^ urgdata = gcnew array<Byte>(len);
		 Marshal::Copy((IntPtr)&a_tcp->server.urgdata,urgdata,0, len);
		 LibnidsWrapper::m_serverCallback(urgdata,a_tcp->addr.saddr,a_tcp->addr.source,a_tcp->addr.daddr,a_tcp->addr.dest,true);	

        return;
      }
      if (a_tcp->client.count_new_urg)
      {
        // new byte of urgent data has arrived
		 int len = a_tcp->client.count_new_urg;
		 array<Byte>^ urgdata = gcnew array<Byte>(len);
		 Marshal::Copy((IntPtr)&a_tcp->client.urgdata,urgdata,0, len);
		 LibnidsWrapper::m_clientCallback(urgdata,a_tcp->addr.saddr,a_tcp->addr.source,a_tcp->addr.daddr,a_tcp->addr.dest,true);	

        return;
      }
      // So, we have some normal data to take care of.
      if (a_tcp->client.count_new)
	{
          // new data for client
		  hlf = &a_tcp->client; // from now on, we will deal with hlf var,
                                // which will point to client side of conn
	}
      else
	{
	  hlf = &a_tcp->server; // analogical
	}
	//we send the newly arrived data
	 int len = hlf->count_new;
	 array<Byte>^ data = gcnew array<Byte>(len);
	 Marshal::Copy((IntPtr)hlf->data,data,0, len);
	 
	 if (a_tcp->client.count_new)
		LibnidsWrapper::m_clientCallback(data,a_tcp->addr.saddr,a_tcp->addr.source,a_tcp->addr.daddr,a_tcp->addr.dest,false);	
	 else
 		 LibnidsWrapper::m_serverCallback(data,a_tcp->addr.saddr,a_tcp->addr.source,a_tcp->addr.daddr,a_tcp->addr.dest,false);	
    }
  if (a_tcp->nids_state == NIDS_EXITING)
    {
    }
  return ;
}

void managedLibnids::LibnidsWrapper::Run(DataCallbackDelagate^ clientCallback, DataCallbackDelagate^ serverCallback)
{
	managedLibnids::LibnidsWrapper::m_clientCallback = clientCallback;
	managedLibnids::LibnidsWrapper::m_serverCallback = serverCallback;
	if (!nids_init ())
	{
		return;
	}
	nids_register_tcp (tcp_callback);
	nids_run ();
}
