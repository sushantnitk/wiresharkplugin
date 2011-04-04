#include <time.h>

typedef char   gchar;
typedef short  gshort;
typedef long   glong;
typedef int    gint;
typedef gint   gboolean;
typedef unsigned char	guchar;
typedef unsigned short	gushort;
typedef unsigned long	gulong;
typedef unsigned int	guint;
typedef float	gfloat;
typedef double	gdouble;
typedef void* gpointer;
typedef signed char gint8;
typedef unsigned char guint8;
typedef signed short gint16;
typedef unsigned short guint16;
typedef signed int gint32;
typedef unsigned int guint32;


typedef struct {
	time_t	secs;
	int	nsecs;
} nstime_t;

/** the maximum length of a protocol field string representation */
#define ITEM_LABEL_LENGTH	240

struct tvbuff;
typedef struct tvbuff tvbuff_t;

typedef struct _header_field_info header_field_info;

/** information describing a header field */
struct _header_field_info {
	/* ---------- set by dissector --------- */
	const char				*name;      /**< full name of this field */
	const char				*abbrev;    /**< abbreviated name of this field */
	enum ftenum			type;       /**< field type, one of FT_ (from ftypes.h) */
	int					display;	/**< one of BASE_, or number of field bits for FT_BOOLEAN */
	const void			*strings;	/**< _value_string (or true_false_string for FT_BOOLEAN), typically converted by VALS() or TFS() If this is an FT_PROTOCOL then it points to the associated protocol_t structure*/
	guint32				bitmask;    /**< FT_BOOLEAN only: bitmask of interesting bits */
	const char				*blurb;		/**< Brief description of field. */

	/* ------- set by proto routines (prefilled by HFILL macro, see below) ------ */
	int				id;		/**< Field ID */
	int				parent;		/**< parent protocol tree */
		/* This field keeps track of whether a field is 
		 * referenced in any filter or not and if so how 
		 * many times. If a filter is being referenced the 
		 * refcount for the parent protocol is updated as well 
		 */
	int				ref_count;	/**< is this field referenced by a filter or not */
	int				bitshift;	/**< bits to shift (FT_BOOLEAN only) */
	header_field_info		*same_name_next; /**< Link to next hfinfo with same abbrev*/
	header_field_info		*same_name_prev; /**< Link to previous hfinfo with same abbrev*/
};


/** string representation, if one of the proto_tree_add_..._format() functions used */
typedef struct _item_label_t {
	char representation[ITEM_LABEL_LENGTH];
} item_label_t;


typedef struct _fvalue_t {
	/*ftype_t*/void	*ftype;
	union {
		/* Put a few basic types in here */
		gpointer	pointer;
		guint32		integer;
		//guint64		integer64;
		gdouble		floating;
		gchar		*string;
		guchar		*ustring;
		//GString		*gstring;
		//ipv4_addr	ipv4;
		nstime_t	time;
		tvbuff_t	*tvb;
	} value;

	/* The following is provided for private use
	 * by the fvalue. */
	gboolean	fvalue_gboolean1;

} fvalue_t;


/** Contains the field information for the proto_item. */
typedef struct field_info {
	header_field_info	*hfinfo;    /**< pointer to registered field information */
	gint				start;      /**< current start of data in field_info.ds_tvb */
	gint				length;     /**< current data length of item in field_info.ds_tvb */
	gint				tree_type;  /**< one of ETT_ or -1 */
	item_label_t		*rep;       /**< string for GUI tree */
	int					flags;      /**< bitfield like FI_GENERATED, ... */
	tvbuff_t			*ds_tvb;    /**< data source tvbuff */
	fvalue_t			value;
} field_info;



typedef struct _proto_node {
	struct _proto_node *first_child;
	struct _proto_node *last_child;
	struct _proto_node *next;
	struct _proto_node *parent;
	/*field_info*/ void *finfo;
	/*tree_data_t*/void *tree_data;
} proto_node;


typedef proto_node proto_tree;


/* Types of port numbers Ethereal knows about. */
typedef enum {
  PT_NONE,		/* no port number */
  PT_SCTP,		/* SCTP */
  PT_TCP,		/* TCP */
  PT_UDP,		/* UDP */
  PT_DCCP,		/* DCCP */
  PT_IPX,		/* IPX sockets */
  PT_NCP,		/* NCP connection */
  PT_EXCHG,		/* Fibre Channel exchange */
  PT_DDP,		/* DDP AppleTalk connection */
  PT_SBCCS,		/* FICON */
  PT_IDP,		/* XNS IDP sockets */
  PT_TIPC		/* TIPC PORT */
} port_type;


/*
 * All of the possible columns in summary listing.
 *
 * NOTE: The SRC and DST entries MUST remain in this order, or else you
 * need to fix the offset #defines before get_column_format!
 */
enum {
  COL_NUMBER,         /* Packet list item number */
  COL_CLS_TIME,       /* Command line-specified time (default relative) */
  COL_REL_TIME,       /* Relative time */
  COL_ABS_TIME,       /* Absolute time */
  COL_ABS_DATE_TIME,  /* Absolute date and time */
  COL_DELTA_TIME,     /* Delta time */
  COL_DEF_SRC,        /* Source address */
  COL_RES_SRC,        /* Resolved source */
  COL_UNRES_SRC,      /* Unresolved source */
  COL_DEF_DL_SRC,     /* Data link layer source address */
  COL_RES_DL_SRC,     /* Resolved DL source */
  COL_UNRES_DL_SRC,   /* Unresolved DL source */
  COL_DEF_NET_SRC,    /* Network layer source address */
  COL_RES_NET_SRC,    /* Resolved net source */
  COL_UNRES_NET_SRC,  /* Unresolved net source */
  COL_DEF_DST,        /* Destination address */
  COL_RES_DST,        /* Resolved dest */
  COL_UNRES_DST,      /* Unresolved dest */
  COL_DEF_DL_DST,     /* Data link layer dest address */
  COL_RES_DL_DST,     /* Resolved DL dest */
  COL_UNRES_DL_DST,   /* Unresolved DL dest */
  COL_DEF_NET_DST,    /* Network layer dest address */
  COL_RES_NET_DST,    /* Resolved net dest */
  COL_UNRES_NET_DST,  /* Unresolved net dest */
  COL_DEF_SRC_PORT,   /* Source port */
  COL_RES_SRC_PORT,   /* Resolved source port */
  COL_UNRES_SRC_PORT, /* Unresolved source port */
  COL_DEF_DST_PORT,   /* Destination port */
  COL_RES_DST_PORT,   /* Resolved dest port */
  COL_UNRES_DST_PORT, /* Unresolved dest port */
  COL_PROTOCOL,       /* Protocol */
  COL_INFO,           /* Description */
  COL_PACKET_LENGTH,  /* Packet length in bytes */
  COL_CUMULATIVE_BYTES, /* Cumulative number of bytes */
  COL_OXID,           /* Fibre Channel OXID */
  COL_RXID,           /* Fibre Channel RXID */
  COL_IF_DIR,         /* FW-1 monitor interface/direction */
  COL_CIRCUIT_ID,     /* Circuit ID */
  COL_SRCIDX,         /* Src port idx - Cisco MDS-specific */
  COL_DSTIDX,         /* Dst port idx - Cisco MDS-specific */
  COL_VSAN,           /* VSAN - Cisco MDS-specific */
  COL_TX_RATE,        /* IEEE 802.11 - TX rate in Mbps */
  COL_RSSI,           /* IEEE 802.11 - received signal strength */
  COL_HPUX_SUBSYS,    /* HP-UX Nettl Subsystem */
  COL_HPUX_DEVID,     /* HP-UX Nettl Device ID */
  COL_DCE_CALL,       /* DCE/RPC call id OR datagram sequence number */
  NUM_COL_FMTS        /* Should always be last */
};

/* Types of circuit IDs Ethereal knows about. */
typedef enum {
  CT_NONE,		/* no circuit type */
  CT_DLCI,		/* Frame Relay DLCI */
  CT_ISDN,		/* ISDN channel number */
  CT_X25,		/* X.25 logical channel number */
  CT_ISUP,		/* ISDN User Part CIC */
  CT_IAX2,		/* IAX2 call id */
  CT_H223,		/* H.223 logical channel number */
  CT_BICC		/* BICC Circuit identifier */
  /* Could also have ATM VPI/VCI pairs */
} circuit_type;


typedef enum {
  AT_NONE,		/* no link-layer address */
  AT_ETHER,		/* MAC (Ethernet, 802.x, FDDI) address */
  AT_IPv4,		/* IPv4 */
  AT_IPv6,		/* IPv6 */
  AT_IPX,		/* IPX */
  AT_SNA,		/* SNA */
  AT_ATALK,		/* Appletalk DDP */
  AT_VINES,		/* Banyan Vines */
  AT_OSI,		/* OSI NSAP */
  AT_ARCNET,	/* ARCNET */
  AT_FC,		/* Fibre Channel */
  AT_SS7PC,		/* SS7 Point Code */
  AT_STRINGZ,	/* null-terminated string */
  AT_EUI64,		/* IEEE EUI-64 */
  AT_URI,		/* URI/URL/URN */
  AT_TIPC		/* TIPC Address Zone,Subnetwork,Processor */
} address_type;

typedef struct _address {
  address_type  type;		/* type of address */
  int           len;		/* length of address, in bytes */
  const guint8 *data;		/* bytes that constitute address */
} address;


typedef void (*tvbuff_free_cb_t)(void*);


typedef enum {
	TVBUFF_REAL_DATA,
	TVBUFF_SUBSET,
	TVBUFF_COMPOSITE
} tvbuff_type;


struct tvbuff;

typedef struct {
	/* The backing tvbuff_t */
	struct tvbuff	*tvb;

	/* The offset/length of 'tvb' to which I'm privy */
	guint		offset;
	guint		length;

} tvb_backing_t;


typedef struct _GSList		GSList;

struct _GSList
{
  gpointer data;
  GSList *next;
};


typedef struct _GString		GString;

typedef unsigned int gsize;


struct _GString
{
  gchar  *str;
  gsize len;    
  gsize allocated_len;
};


typedef struct {
	GSList		*tvbs;

	/* Used for quick testing to see if this
	 * is the tvbuff that a COMPOSITE is
	 * interested in. */
	guint		*start_offsets;
	guint		*end_offsets;

} tvb_comp_t;


struct tvbuff {
  /* Record-keeping */
	tvbuff_type		type;
	gboolean		initialized;
	guint			usage_count;
	struct tvbuff		*ds_tvb;  /* data source top-level tvbuff */

	/* The tvbuffs in which this tvbuff is a member
	 * (that is, a backing tvbuff for a TVBUFF_SUBSET
	 * or a member for a TVB_COMPOSITE) */
	GSList			*used_in;

	/* TVBUFF_SUBSET and TVBUFF_COMPOSITE keep track
	 * of the other tvbuff's they use */
	union {
		tvb_backing_t	subset;
		tvb_comp_t	composite;
	} tvbuffs;

	/* We're either a TVBUFF_REAL_DATA or a
	 * TVBUFF_SUBSET that has a backing buffer that
	 * has real_data != NULL, or a TVBUFF_COMPOSITE
	 * which has flattened its data due to a call
	 * to tvb_get_ptr().
	 */
	const guint8		*real_data;

	/* Length of virtual buffer (and/or real_data). */
	guint			length;

	/* Reported length. */
	guint			reported_length;

	/* Offset from beginning of first TVBUFF_REAL. */
	gint			raw_offset;

	/* Func to call when actually freed */
	tvbuff_free_cb_t	free_cb;
};


#define MAX_NUMBER_OF_PPIDS     2


typedef struct _column_info {
  gint          num_cols;    /* Number of columns */
  gint         *col_fmt;     /* Format of column */
  gboolean    **fmt_matx;    /* Specifies which formats apply to a column */
  gint         *col_first;   /* First column number with a given format */
  gint         *col_last;    /* Last column number with a given format */
  gchar       **col_title;   /* Column titles */
  const gchar **col_data;    /* Column data */
  gchar       **col_buf;     /* Buffer into which to copy data for column */
  int         *col_fence;    /* Stuff in column buffer before this index is immutable */
  gchar      **col_expr;     /* Filter expression */
  gchar      **col_expr_val; /* Value for filter expression */
  gboolean     writable;     /* Are we still writing to the columns? */
} column_info;



typedef struct _frame_data {
  struct _frame_data *next; /* Next element in list */
  struct _frame_data *prev; /* Previous element in list */
  GSList      *pfd;         /* Per frame proto data */
  guint32      num;         /* Frame number */
  guint32      pkt_len;     /* Packet length */
  guint32      cap_len;     /* Amount actually captured */
  guint32      cum_bytes;   /* Cumulative bytes into the capture */
  nstime_t     abs_ts;      /* Absolute timestamp */
  nstime_t     rel_ts;      /* Relative timestamp (yes, it can be negative) */
  nstime_t     del_ts;      /* Delta timestamp (yes, it can be negative) */
  long         file_off;    /* File offset */
  int          lnk_t;       /* Per-packet encapsulation/data-link type */
  struct {
	unsigned int passed_dfilter	: 1; /* 1 = display, 0 = no display */
  	unsigned int encoding		: 2; /* Character encoding (ASCII, EBCDIC...) */
	unsigned int visited		: 1; /* Has this packet been visited yet? 1=Yes,0=No*/
	unsigned int marked             : 1; /* 1 = marked by user, 0 = normal */
	unsigned int ref_time		: 1; /* 1 = marked as a reference time frame, 0 = normal */
  } flags;
  void *color_filter;       /* Per-packet matching color_filter_t object */
} frame_data;



typedef struct _packet_info {
  const char *current_proto;	/* name of protocol currently being dissected */
  column_info *cinfo;		/* Column formatting information */
  frame_data *fd;
  union wtap_pseudo_header *pseudo_header;
  GSList *data_src;		/* Frame data sources */
  address dl_src;		/* link-layer source address */
  address dl_dst;		/* link-layer destination address */
  address net_src;		/* network-layer source address */
  address net_dst;		/* network-layer destination address */
  address src;			/* source address (net if present, DL otherwise )*/
  address dst;			/* destination address (net if present, DL otherwise )*/
  guint32 ethertype;		/* Ethernet Type Code, if this is an Ethernet packet */
  guint32 ipproto;		/* IP protocol, if this is an IP packet */
  guint32 ipxptype;		/* IPX packet type, if this is an IPX packet */
  circuit_type ctype;		/* type of circuit, for protocols with a VC identifier */
  guint32 circuit_id;		/* circuit ID, for protocols with a VC identifier */
  const char *noreassembly_reason;  /* reason why reassembly wasn't done, if any */
  gboolean fragmented;		/* TRUE if the protocol is only a fragment */
  gboolean in_error_pkt;	/* TRUE if we're inside an {ICMP,CLNP,...} error packet */
  port_type ptype;		/* type of the following two port numbers */
  guint32 srcport;		/* source port */
  guint32 destport;		/* destination port */
  guint32 match_port;
  const char *match_string;	/* Subdissectors with string dissector tables use this */
  guint16 can_desegment;	/* >0 if this segment could be desegmented.
				   A dissector that can offer this API (e.g.
				   TCP) sets can_desegment=2, then
				   can_desegment is decremented by 1 each time
				   we pass to the next subdissector. Thus only
				   the dissector immediately above the
				   protocol which sets the flag can use it*/
  guint16 saved_can_desegment;	/* Value of can_desegment before current
				   dissector was called.  Supplied so that
				   dissectors for proxy protocols such as
				   SOCKS can restore it, allowing the
				   dissectors that they call to use the
				   TCP dissector's desegmentation (SOCKS
				   just retransmits TCP segments once it's
				   finished setting things up, so the TCP
				   desegmentor can desegment its payload). */
  int desegment_offset;		/* offset to stuff needing desegmentation */
  guint32 desegment_len;	/* requested desegmentation additional length */
  guint16 want_pdu_tracking;	/* >0 if the subdissector has specified
				   a value in 'bytes_until_next_pdu'.
				   When a dissector detects that the next PDU
				   will start beyond the start of the next
				   segment, it can set this value to 2
				   and 'bytes_until_next_pdu' to the number of
				   bytes beyond the next segment where the
				   next PDU starts.

				   If the protocol dissector below this
				   one is capable of PDU tracking it can
				   use this hint to detect PDUs that starts
				   unaligned to the segment boundaries.
				   The TCP dissector is using this hint from
				   (some) protocols to detect when a new PDU
				   starts in the middle of a tcp segment.

				   There is intelligence in the glue between
				   dissector layers to make sure that this
				   request is only passed down to the protocol
				   immediately below the current one and not
				   any further.
				*/
  guint32 bytes_until_next_pdu;


  int     iplen;
  int     iphdrlen;
  int	  p2p_dir;              /* Packet was captured as an 
                                       outbound (P2P_DIR_SENT) 
                                       inbound (P2P_DIR_RECV) 
                                       unknown (P2P_DIR_UNKNOWN) */
  guint16 oxid;                 /* next 2 fields reqd to identify fibre */
  guint16 rxid;                 /* channel conversations */
  guint8  r_ctl;                /* R_CTL field in Fibre Channel Protocol */
  guint8  sof_eof;              /* FC's SOF/EOF encoding passed to FC decoder
                                 * Bit 7 set if Last frame in sequence
                                 * Bit 6 set if invalid frame content
                                 * Bit 2 set if SOFf
                                 * Bit 1 set if first frame in sequence
                                 */
  guint16 src_idx;              /* Source port index (Cisco MDS-specific) */
  guint16 dst_idx;              /* Dest port index (Cisco MDS-specific) */
  guint16 vsan;                 /* Fibre channel/Cisco MDS-specific */

  /* Extra data for DCERPC handling and tracking of context ids */
  guint16 dcectxid;             /* Context ID (DCERPC-specific) */
  int     dcetransporttype;     /* Transport type
                                 * Value -1 means "not a DCERPC packet"
                                 */
  guint16 dcetransportsalt;	/* fid: if transporttype==DCE_CN_TRANSPORT_SMBPIPE */

  /* Extra data for handling of decryption of GSSAPI wrapped tvbuffs.
     Caller sets decrypt_gssapi_tvb if this service is requested.
     If gssapi_encrypted_tvb is NULL, then the rest of the tvb data following
     the gssapi blob itself is decrypted othervise the gssapi_encrypted_tvb
     tvb will be decrypted (DCERPC has the data before the gssapi blob)
     If, on return, gssapi_data_encrypted is FALSE, the wrapped tvbuff
     was signed (i.e., an encrypted signature was present, to check
     whether the data was modified by a man in the middle) but not sealed
     (i.e., the data itself wasn't encrypted).
  */
  
  #define DECRYPT_GSSAPI_NORMAL	1
  #define DECRYPT_GSSAPI_DCE	2
  
  guint16 decrypt_gssapi_tvb;
  tvbuff_t *gssapi_wrap_tvb;
  tvbuff_t *gssapi_encrypted_tvb;
  tvbuff_t *gssapi_decrypted_tvb;
  gboolean gssapi_data_encrypted;
 
  guint32 ppid[MAX_NUMBER_OF_PPIDS]; /* The first NUMBER_OF_PPIDS PPIDS which are present
                                      * in the SCTP packet
                                      */
  void    *private_data;	/* pointer to data passed from one dissector to another */
  GString *layer_names; 	/* layers of each protocol */
  guint16 link_number;
  gchar   annex_a_used;

} packet_info;


typedef void (*dissector_t)(tvbuff_t *tvb, packet_info *pinfo, proto_tree *tree);
#define dissector_handle_t void *
typedef proto_node proto_item;