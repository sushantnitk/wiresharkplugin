// managedLibnids.h

#pragma once
extern "C"
{
	#include "nids.h"
}

using namespace System;
using namespace System::Runtime::InteropServices;

public delegate void DataCallbackDelagate(array<Byte>^ data,UInt32 sourceIP,UInt16 sourcePort,UInt32 destinationIP,UInt16 destinationPort, bool urgent);
delegate void TcpCallbackDelegate(IntPtr tcpStream, IntPtr arg);

namespace managedLibnids {

	public ref class LibnidsWrapper
	{
	public:
		static void Run(DataCallbackDelagate^ clientCallback, DataCallbackDelagate^ serverCallback);
		
		static void Run(String^ filename, DataCallbackDelagate^ clientCallback, DataCallbackDelagate^ serverCallback)
		{
			IntPtr p = Marshal::StringToHGlobalAnsi(filename);
			char *szFile = (char*)p.ToPointer();
			nids_params.filename = szFile;
			Run(clientCallback,serverCallback);
			Marshal::FreeHGlobal(p);
			nids_params.filename = NULL;
		}

	internal:
		static DataCallbackDelagate^ m_clientCallback; 
		static DataCallbackDelagate^ m_serverCallback;
	};
	static void tcp_callback (struct tcp_stream *a_tcp, void ** this_time_not_needed);
}
