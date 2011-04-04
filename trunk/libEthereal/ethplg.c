#include <windows.h>
#include "ethplg.h"

//#ifdef HAVE_CONFIG_H
//#include "config.h"
//#endif
//#include <gmodule.h>
//#include <epan/packet.h>
//#include <epan/prefs.h>

//////////imports from libethereal.dll
void (*dissector_add)(const char *abbrev, guint32 pattern,dissector_handle_t handle);
dissector_handle_t (*create_dissector_handle)(dissector_t dissector,int proto);
void *(*prefs_register_protocol)(int id, void (*apply_cb)(void));
int (*proto_register_protocol)(const char *name, const char *short_name, const char *filter_name);
gint	(*check_col)(column_info *cinfo, gint col);
void	(*col_set_str)(column_info *cinfo, gint col, const gchar * str);
proto_item * (*proto_tree_add_item)(proto_tree *tree, int hfindex, tvbuff_t *tvb,gint start, gint length, gboolean little_endian);
dissector_handle_t (*find_dissector)(const char *name);
int (*call_dissector)(dissector_handle_t handle, tvbuff_t *tvb,packet_info *pinfo, proto_tree *tree);
tvbuff_t* (*tvb_new_subset)(tvbuff_t* backing,gint backing_offset, gint backing_length, gint reported_length);
gint (*tvb_length_remaining)(tvbuff_t*, gint offset);
guint (*tvb_length)(tvbuff_t*);
void	(*col_append_str)(column_info *cinfo, gint col, const gchar *str);



void initfuncs()
{
  void * h = LoadLibrary("libethereal.dll");

  dissector_add = (void*)GetProcAddress(h,"dissector_add");
  create_dissector_handle = (void*)GetProcAddress(h,"create_dissector_handle");
  prefs_register_protocol = (void*)GetProcAddress(h,"prefs_register_protocol");
  proto_register_protocol = (void*)GetProcAddress(h,"proto_register_protocol");
  check_col = (void*)GetProcAddress(h,"check_col");
  col_set_str = (void*)GetProcAddress(h,"col_set_str");
  find_dissector = (void*)GetProcAddress(h,"find_dissector");
  call_dissector = (void*)GetProcAddress(h,"call_dissector");
  tvb_new_subset = (void*)GetProcAddress(h,"tvb_new_subset");
  tvb_length_remaining = (void*)GetProcAddress(h,"tvb_length_remaining");
  tvb_length = (void*)GetProcAddress(h,"tvb_length");
  col_append_str = (void*)GetProcAddress(h,"col_append_str");

}
//////////

static int proto_foo = -1;
static dissector_handle_t foo_handle;
static dissector_handle_t ip_handle;


void dissect_foo(tvbuff_t *tvb, packet_info *pinfo, proto_tree *tree)
{
  tvbuff_t *next_tvb;
  unsigned int captured_length = tvb_length_remaining(tvb, 14);

  //OutputDebugString("dissect_foo");

  next_tvb = tvb_new_subset(tvb, 0, tvb_length(tvb), tvb_length(tvb));

  call_dissector(ip_handle,next_tvb,pinfo,tree);

  if (check_col(pinfo->cinfo, COL_PROTOCOL))
  {
    col_append_str(pinfo->cinfo, COL_PROTOCOL, " FOO");
  }

}

void proto_reg_handoff_foo(void)
{
	static int Initialized=0;

  //OutputDebugString("proto_reg_handoff_foo");

	if (!Initialized) 
  {
    ip_handle = find_dissector("ip");
		foo_handle = create_dissector_handle(dissect_foo, proto_foo);
		dissector_add("ethertype", 0x0800, foo_handle);
	}
}

void proto_register_foo(void)
{
	void *foo_module;

  //OutputDebugString("proto_register_foo");

	if (proto_foo == -1) {
		proto_foo = proto_register_protocol (
			"FOO Protocol",		// name
			"FOO",		// short name
			"foo"		// abbrev
			);
	}
	foo_module	= prefs_register_protocol(proto_foo, proto_reg_handoff_foo);
}


//Define version if we are not building ethereal statically
__declspec(dllexport) const unsigned char version[] = "0.0";

__declspec(dllexport) void plugin_register(void)
{
  initfuncs();

  //OutputDebugString("plugin_register");

  //register the new protocol, protocol fields, and subtrees
  if (proto_foo == -1) 
  { //execute protocol initialization only once
      proto_register_foo();
  }
}

__declspec(dllexport) void plugin_reg_handoff(void)
{
  //OutputDebugString("plugin_reg_handoff");
  proto_reg_handoff_foo();
}


BOOL WINAPI DllMain(HINSTANCE hinstDLL,DWORD fdwReason,LPVOID lpvReserved)
{
  //OutputDebugString("DllMain");
  return TRUE;
}


