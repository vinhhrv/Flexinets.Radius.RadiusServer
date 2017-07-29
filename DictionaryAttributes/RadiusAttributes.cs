
using System;
using System.Net;

namespace Flexinets.Radius.DictionaryAttributes
{
    public partial class UserNameAttribute : StringAttribute
    {
        public const UInt32 Code = 1;
        public UserNameAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class UserPasswordAttribute : OctetAttribute
    {
        public const UInt32 Code = 2;
        public UserPasswordAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class CHAPPasswordAttribute : OctetAttribute
    {
        public const UInt32 Code = 3;
        public CHAPPasswordAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class NASIPAddressAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 4;
        public NASIPAddressAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class NASPortAttribute : IntegerAttribute
    {
        public const UInt32 Code = 5;
        public NASPortAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ServiceTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 6;
        public ServiceTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FramedProtocolAttribute : IntegerAttribute
    {
        public const UInt32 Code = 7;
        public FramedProtocolAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FramedIPAddressAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 8;
        public FramedIPAddressAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class FramedIPNetmaskAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 9;
        public FramedIPNetmaskAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class FramedRoutingAttribute : IntegerAttribute
    {
        public const UInt32 Code = 10;
        public FramedRoutingAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FilterIdAttribute : StringAttribute
    {
        public const UInt32 Code = 11;
        public FilterIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedMTUAttribute : IntegerAttribute
    {
        public const UInt32 Code = 12;
        public FramedMTUAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FramedCompressionAttribute : IntegerAttribute
    {
        public const UInt32 Code = 13;
        public FramedCompressionAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LoginIPHostAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 14;
        public LoginIPHostAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class LoginServiceAttribute : IntegerAttribute
    {
        public const UInt32 Code = 15;
        public LoginServiceAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LoginTCPPortAttribute : IntegerAttribute
    {
        public const UInt32 Code = 16;
        public LoginTCPPortAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class OldPasswordAttribute : StringAttribute
    {
        public const UInt32 Code = 17;
        public OldPasswordAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ReplyMessageAttribute : StringAttribute
    {
        public const UInt32 Code = 18;
        public ReplyMessageAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class CallbackNumberAttribute : StringAttribute
    {
        public const UInt32 Code = 19;
        public CallbackNumberAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class CallbackIdAttribute : StringAttribute
    {
        public const UInt32 Code = 20;
        public CallbackIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedRouteAttribute : StringAttribute
    {
        public const UInt32 Code = 22;
        public FramedRouteAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedIPXNetworkAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 23;
        public FramedIPXNetworkAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class StateAttribute : OctetAttribute
    {
        public const UInt32 Code = 24;
        public StateAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class ClassAttribute : StringAttribute
    {
        public const UInt32 Code = 25;
        public ClassAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class VendorSpecificAttribute : StringAttribute
    {
        public const UInt32 Code = 26;
        public VendorSpecificAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class SessionTimeoutAttribute : IntegerAttribute
    {
        public const UInt32 Code = 27;
        public SessionTimeoutAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class IdleTimeoutAttribute : IntegerAttribute
    {
        public const UInt32 Code = 28;
        public IdleTimeoutAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class TerminationActionAttribute : IntegerAttribute
    {
        public const UInt32 Code = 29;
        public TerminationActionAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class CalledStationIdAttribute : StringAttribute
    {
        public const UInt32 Code = 30;
        public CalledStationIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class CallingStationIdAttribute : StringAttribute
    {
        public const UInt32 Code = 31;
        public CallingStationIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class NASIdentifierAttribute : StringAttribute
    {
        public const UInt32 Code = 32;
        public NASIdentifierAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ProxyStateAttribute : OctetAttribute
    {
        public const UInt32 Code = 33;
        public ProxyStateAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class LoginLATServiceAttribute : StringAttribute
    {
        public const UInt32 Code = 34;
        public LoginLATServiceAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class LoginLATNodeAttribute : StringAttribute
    {
        public const UInt32 Code = 35;
        public LoginLATNodeAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class LoginLATGroupAttribute : StringAttribute
    {
        public const UInt32 Code = 36;
        public LoginLATGroupAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedAppleTalkLinkAttribute : IntegerAttribute
    {
        public const UInt32 Code = 37;
        public FramedAppleTalkLinkAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FramedAppleTalkNetworkAttribute : IntegerAttribute
    {
        public const UInt32 Code = 38;
        public FramedAppleTalkNetworkAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class FramedAppleTalkZoneAttribute : StringAttribute
    {
        public const UInt32 Code = 39;
        public FramedAppleTalkZoneAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctStatusTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 40;
        public AcctStatusTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctDelayTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 41;
        public AcctDelayTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctInputOctetsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 42;
        public AcctInputOctetsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctOutputOctetsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 43;
        public AcctOutputOctetsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctSessionIdAttribute : StringAttribute
    {
        public const UInt32 Code = 44;
        public AcctSessionIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctAuthenticAttribute : IntegerAttribute
    {
        public const UInt32 Code = 45;
        public AcctAuthenticAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctSessionTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 46;
        public AcctSessionTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctInputPacketsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 47;
        public AcctInputPacketsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctOutputPacketsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 48;
        public AcctOutputPacketsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctTerminateCauseAttribute : IntegerAttribute
    {
        public const UInt32 Code = 49;
        public AcctTerminateCauseAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctMultiSessionIdAttribute : StringAttribute
    {
        public const UInt32 Code = 50;
        public AcctMultiSessionIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctLinkCountAttribute : IntegerAttribute
    {
        public const UInt32 Code = 51;
        public AcctLinkCountAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctInputGigawordsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 52;
        public AcctInputGigawordsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctOutputGigawordsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 53;
        public AcctOutputGigawordsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class EventTimestampAttribute : IntegerAttribute
    {
        public const UInt32 Code = 55;
        public EventTimestampAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class CHAPChallengeAttribute : OctetAttribute
    {
        public const UInt32 Code = 60;
        public CHAPChallengeAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class NASPortTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 61;
        public NASPortTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class PortLimitAttribute : IntegerAttribute
    {
        public const UInt32 Code = 62;
        public PortLimitAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LoginLATPortAttribute : StringAttribute
    {
        public const UInt32 Code = 63;
        public LoginLATPortAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class TunnelPasswordAttribute : StringAttribute
    {
        public const UInt32 Code = 69;
        public TunnelPasswordAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ARAPPasswordAttribute : StringAttribute
    {
        public const UInt32 Code = 70;
        public ARAPPasswordAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ARAPFeaturesAttribute : StringAttribute
    {
        public const UInt32 Code = 71;
        public ARAPFeaturesAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ARAPZoneAccessAttribute : IntegerAttribute
    {
        public const UInt32 Code = 72;
        public ARAPZoneAccessAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ARAPSecurityAttribute : IntegerAttribute
    {
        public const UInt32 Code = 73;
        public ARAPSecurityAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ARAPSecurityDataAttribute : StringAttribute
    {
        public const UInt32 Code = 74;
        public ARAPSecurityDataAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class PasswordRetryAttribute : IntegerAttribute
    {
        public const UInt32 Code = 75;
        public PasswordRetryAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class PromptAttribute : IntegerAttribute
    {
        public const UInt32 Code = 76;
        public PromptAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ConnectInfoAttribute : StringAttribute
    {
        public const UInt32 Code = 77;
        public ConnectInfoAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class ConfigurationTokenAttribute : OctetAttribute
    {
        public const UInt32 Code = 78;
        public ConfigurationTokenAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class EAPMessageAttribute : OctetAttribute
    {
        public const UInt32 Code = 79;
        public EAPMessageAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class MessageAuthenticatorAttribute : OctetAttribute
    {
        public const UInt32 Code = 80;
        public MessageAuthenticatorAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class AcctInterimIntervalAttribute : IntegerAttribute
    {
        public const UInt32 Code = 85;
        public AcctInterimIntervalAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AcctTunnelPacketsLostAttribute : IntegerAttribute
    {
        public const UInt32 Code = 86;
        public AcctTunnelPacketsLostAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class NASPortIdAttribute : StringAttribute
    {
        public const UInt32 Code = 87;
        public NASPortIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedPoolAttribute : StringAttribute
    {
        public const UInt32 Code = 88;
        public FramedPoolAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class CUIAttribute : StringAttribute
    {
        public const UInt32 Code = 89;
        public CUIAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class NASIPv6AddressAttribute : OctetAttribute
    {
        public const UInt32 Code = 95;
        public NASIPv6AddressAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class FramedInterfaceIdAttribute : StringAttribute
    {
        public const UInt32 Code = 96;
        public FramedInterfaceIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class FramedIPv6PrefixAttribute : StringAttribute
    {
        public const UInt32 Code = 97;
        public FramedIPv6PrefixAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class LoginIPv6HostAttribute : OctetAttribute
    {
        public const UInt32 Code = 98;
        public LoginIPv6HostAttribute(Byte[] value)
        {
            Value = value;
        }
    }
    public partial class FramedIPv6RouteAttribute : StringAttribute
    {
        public const UInt32 Code = 99;
        public FramedIPv6RouteAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class GricARSServerIdAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 100;
        public GricARSServerIdAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class ErrorCauseAttribute : IntegerAttribute
    {
        public const UInt32 Code = 101;
        public ErrorCauseAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class GricIspIdAttribute : StringAttribute
    {
        public const UInt32 Code = 102;
        public GricIspIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class TimestampAttribute : IntegerAttribute
    {
        public const UInt32 Code = 103;
        public TimestampAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class GricTimezoneAttribute : IntegerAttribute
    {
        public const UInt32 Code = 104;
        public GricTimezoneAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class GricRequestTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 105;
        public GricRequestTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRLinkStatusDlciAttribute : IntegerAttribute
    {
        public const UInt32 Code = 106;
        public AscendFRLinkStatusDlciAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCallingSubadddressAttribute : StringAttribute
    {
        public const UInt32 Code = 107;
        public AscendCallingSubadddressAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendCallbackDelayAttribute : IntegerAttribute
    {
        public const UInt32 Code = 108;
        public AscendCallbackDelayAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendEndpointDiscAttribute : StringAttribute
    {
        public const UInt32 Code = 109;
        public AscendEndpointDiscAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendRemoteFWAttribute : StringAttribute
    {
        public const UInt32 Code = 110;
        public AscendRemoteFWAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendMulticastGLeaveDelayAttribute : IntegerAttribute
    {
        public const UInt32 Code = 111;
        public AscendMulticastGLeaveDelayAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCBCPEnableAttribute : IntegerAttribute
    {
        public const UInt32 Code = 112;
        public AscendCBCPEnableAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCBCPModeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 113;
        public AscendCBCPModeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCBCPDelayAttribute : IntegerAttribute
    {
        public const UInt32 Code = 114;
        public AscendCBCPDelayAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCBCPTrunkGroupAttribute : IntegerAttribute
    {
        public const UInt32 Code = 115;
        public AscendCBCPTrunkGroupAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendAppletalkRouteAttribute : StringAttribute
    {
        public const UInt32 Code = 116;
        public AscendAppletalkRouteAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendAppletalkPeerModeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 117;
        public AscendAppletalkPeerModeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendRouteAppletalkAttribute : IntegerAttribute
    {
        public const UInt32 Code = 118;
        public AscendRouteAppletalkAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFCPParameterAttribute : StringAttribute
    {
        public const UInt32 Code = 119;
        public AscendFCPParameterAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendModemPortNoAttribute : IntegerAttribute
    {
        public const UInt32 Code = 120;
        public AscendModemPortNoAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendModemSlotNoAttribute : IntegerAttribute
    {
        public const UInt32 Code = 121;
        public AscendModemSlotNoAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendModemShelfNoAttribute : IntegerAttribute
    {
        public const UInt32 Code = 122;
        public AscendModemShelfNoAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCallAttemptLimitAttribute : IntegerAttribute
    {
        public const UInt32 Code = 123;
        public AscendCallAttemptLimitAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCallBlockDurationAttribute : IntegerAttribute
    {
        public const UInt32 Code = 124;
        public AscendCallBlockDurationAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMaximumCallDurationAttribute : IntegerAttribute
    {
        public const UInt32 Code = 125;
        public AscendMaximumCallDurationAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendRoutePreferenceAttribute : IntegerAttribute
    {
        public const UInt32 Code = 126;
        public AscendRoutePreferenceAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class TunnelingProtocolAttribute : IntegerAttribute
    {
        public const UInt32 Code = 127;
        public TunnelingProtocolAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendSharedProfileEnableAttribute : IntegerAttribute
    {
        public const UInt32 Code = 128;
        public AscendSharedProfileEnableAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPrimaryHomeAgentAttribute : StringAttribute
    {
        public const UInt32 Code = 129;
        public AscendPrimaryHomeAgentAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendSecondaryHomeAgentAttribute : StringAttribute
    {
        public const UInt32 Code = 130;
        public AscendSecondaryHomeAgentAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendDialoutAllowedAttribute : IntegerAttribute
    {
        public const UInt32 Code = 131;
        public AscendDialoutAllowedAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendClientGatewayAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 132;
        public AscendClientGatewayAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendBACPEnableAttribute : IntegerAttribute
    {
        public const UInt32 Code = 133;
        public AscendBACPEnableAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDHCPMaximumLeasesAttribute : IntegerAttribute
    {
        public const UInt32 Code = 134;
        public AscendDHCPMaximumLeasesAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendClientPrimaryDNSAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 135;
        public AscendClientPrimaryDNSAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendClientSecondaryDNSAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 136;
        public AscendClientSecondaryDNSAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendClientAssignDNSAttribute : IntegerAttribute
    {
        public const UInt32 Code = 137;
        public AscendClientAssignDNSAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 138;
        public AscendUserAcctTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctHostAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 139;
        public AscendUserAcctHostAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctPortAttribute : IntegerAttribute
    {
        public const UInt32 Code = 140;
        public AscendUserAcctPortAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctKeyAttribute : StringAttribute
    {
        public const UInt32 Code = 141;
        public AscendUserAcctKeyAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctBaseAttribute : IntegerAttribute
    {
        public const UInt32 Code = 142;
        public AscendUserAcctBaseAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendUserAcctTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 143;
        public AscendUserAcctTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendAssignIPClientAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 144;
        public AscendAssignIPClientAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class LASStartTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 145;
        public LASStartTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LASCodeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 146;
        public LASCodeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LASDurationAttribute : IntegerAttribute
    {
        public const UInt32 Code = 147;
        public LASDurationAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class LocalDurationAttribute : IntegerAttribute
    {
        public const UInt32 Code = 148;
        public LocalDurationAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ServiceClassAttribute : StringAttribute
    {
        public const UInt32 Code = 149;
        public ServiceClassAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class Port_EntryAttribute : StringAttribute
    {
        public const UInt32 Code = 150;
        public Port_EntryAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class TokenPoolAttribute : StringAttribute
    {
        public const UInt32 Code = 155;
        public TokenPoolAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendFRCircuitNameAttribute : StringAttribute
    {
        public const UInt32 Code = 156;
        public AscendFRCircuitNameAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendFRLinkUpAttribute : IntegerAttribute
    {
        public const UInt32 Code = 157;
        public AscendFRLinkUpAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRNailedGrpAttribute : IntegerAttribute
    {
        public const UInt32 Code = 158;
        public AscendFRNailedGrpAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 159;
        public AscendFRTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRLinkMgtAttribute : IntegerAttribute
    {
        public const UInt32 Code = 160;
        public AscendFRLinkMgtAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRN391Attribute : IntegerAttribute
    {
        public const UInt32 Code = 161;
        public AscendFRN391Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDCEN392Attribute : IntegerAttribute
    {
        public const UInt32 Code = 162;
        public AscendFRDCEN392Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDTEN392Attribute : IntegerAttribute
    {
        public const UInt32 Code = 163;
        public AscendFRDTEN392Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDCEN393Attribute : IntegerAttribute
    {
        public const UInt32 Code = 164;
        public AscendFRDCEN393Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDTEN393Attribute : IntegerAttribute
    {
        public const UInt32 Code = 165;
        public AscendFRDTEN393Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRT391Attribute : IntegerAttribute
    {
        public const UInt32 Code = 166;
        public AscendFRT391Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRT392Attribute : IntegerAttribute
    {
        public const UInt32 Code = 167;
        public AscendFRT392Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendBridgeAddressAttribute : StringAttribute
    {
        public const UInt32 Code = 168;
        public AscendBridgeAddressAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendTSIdleLimitAttribute : IntegerAttribute
    {
        public const UInt32 Code = 169;
        public AscendTSIdleLimitAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendTSIdleModeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 170;
        public AscendTSIdleModeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDBAMonitorAttribute : IntegerAttribute
    {
        public const UInt32 Code = 171;
        public AscendDBAMonitorAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendBaseChannelCountAttribute : IntegerAttribute
    {
        public const UInt32 Code = 172;
        public AscendBaseChannelCountAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMinimumChannelsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 173;
        public AscendMinimumChannelsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendIPXRouteAttribute : StringAttribute
    {
        public const UInt32 Code = 174;
        public AscendIPXRouteAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendFT1CallerAttribute : IntegerAttribute
    {
        public const UInt32 Code = 175;
        public AscendFT1CallerAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendBackupAttribute : StringAttribute
    {
        public const UInt32 Code = 176;
        public AscendBackupAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendCallTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 177;
        public AscendCallTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendGroupAttribute : StringAttribute
    {
        public const UInt32 Code = 178;
        public AscendGroupAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDLCIAttribute : IntegerAttribute
    {
        public const UInt32 Code = 179;
        public AscendFRDLCIAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRProfileNameAttribute : StringAttribute
    {
        public const UInt32 Code = 180;
        public AscendFRProfileNameAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendAraPWAttribute : StringAttribute
    {
        public const UInt32 Code = 181;
        public AscendAraPWAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendIPXNodeAddrAttribute : StringAttribute
    {
        public const UInt32 Code = 182;
        public AscendIPXNodeAddrAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendHomeAgentIPAddrAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 183;
        public AscendHomeAgentIPAddrAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendHomeAgentPasswordAttribute : StringAttribute
    {
        public const UInt32 Code = 184;
        public AscendHomeAgentPasswordAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendHomeNetworkNameAttribute : StringAttribute
    {
        public const UInt32 Code = 185;
        public AscendHomeNetworkNameAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendHomeAgentUDPPortAttribute : IntegerAttribute
    {
        public const UInt32 Code = 186;
        public AscendHomeAgentUDPPortAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMultilinkIDAttribute : IntegerAttribute
    {
        public const UInt32 Code = 187;
        public AscendMultilinkIDAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendNumInMultilinkAttribute : IntegerAttribute
    {
        public const UInt32 Code = 188;
        public AscendNumInMultilinkAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFirstDestAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 189;
        public AscendFirstDestAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendPreInputOctetsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 190;
        public AscendPreInputOctetsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPreOutputOctetsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 191;
        public AscendPreOutputOctetsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPreInputPacketsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 192;
        public AscendPreInputPacketsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPreOutputPacketsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 193;
        public AscendPreOutputPacketsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMaximumTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 194;
        public AscendMaximumTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDisconnectCauseAttribute : IntegerAttribute
    {
        public const UInt32 Code = 195;
        public AscendDisconnectCauseAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendConnectProgressAttribute : IntegerAttribute
    {
        public const UInt32 Code = 196;
        public AscendConnectProgressAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDataRateAttribute : IntegerAttribute
    {
        public const UInt32 Code = 197;
        public AscendDataRateAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPreSessionTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 198;
        public AscendPreSessionTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendTokenIdleAttribute : IntegerAttribute
    {
        public const UInt32 Code = 199;
        public AscendTokenIdleAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendTokenImmediateAttribute : IntegerAttribute
    {
        public const UInt32 Code = 200;
        public AscendTokenImmediateAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendRequireAuthAttribute : IntegerAttribute
    {
        public const UInt32 Code = 201;
        public AscendRequireAuthAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendNumberSessionsAttribute : StringAttribute
    {
        public const UInt32 Code = 202;
        public AscendNumberSessionsAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendAuthenAliasAttribute : StringAttribute
    {
        public const UInt32 Code = 203;
        public AscendAuthenAliasAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendTokenExpiryAttribute : IntegerAttribute
    {
        public const UInt32 Code = 204;
        public AscendTokenExpiryAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMenuSelectorAttribute : StringAttribute
    {
        public const UInt32 Code = 205;
        public AscendMenuSelectorAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendMenuItemAttribute : StringAttribute
    {
        public const UInt32 Code = 206;
        public AscendMenuItemAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendPWWarntimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 207;
        public AscendPWWarntimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPWLifetimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 208;
        public AscendPWLifetimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AvailableTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 209;
        public AvailableTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class InfoPortAttribute : IntegerAttribute
    {
        public const UInt32 Code = 210;
        public InfoPortAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class ProxyActionAttribute : StringAttribute
    {
        public const UInt32 Code = 211;
        public ProxyActionAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class SignatureAttribute : StringAttribute
    {
        public const UInt32 Code = 212;
        public SignatureAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class TokenAttribute : StringAttribute
    {
        public const UInt32 Code = 213;
        public TokenAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctRateAttribute : StringAttribute
    {
        public const UInt32 Code = 214;
        public AcctRateAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctChargeAttribute : StringAttribute
    {
        public const UInt32 Code = 215;
        public AcctChargeAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctTransactionIdAttribute : StringAttribute
    {
        public const UInt32 Code = 216;
        public AcctTransactionIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AcctChargeAllowedAttribute : StringAttribute
    {
        public const UInt32 Code = 217;
        public AcctChargeAllowedAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class MaximumTimeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 218;
        public MaximumTimeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendFRDirectAttribute : IntegerAttribute
    {
        public const UInt32 Code = 219;
        public AscendFRDirectAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class TimeUsedAttribute : IntegerAttribute
    {
        public const UInt32 Code = 220;
        public TimeUsedAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class HuntgroupNameAttribute : StringAttribute
    {
        public const UInt32 Code = 221;
        public HuntgroupNameAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class UserIdAttribute : StringAttribute
    {
        public const UInt32 Code = 222;
        public UserIdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class UserRealmAttribute : StringAttribute
    {
        public const UInt32 Code = 223;
        public UserRealmAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendIPXAliasAttribute : IntegerAttribute
    {
        public const UInt32 Code = 224;
        public AscendIPXAliasAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMetricAttribute : IntegerAttribute
    {
        public const UInt32 Code = 225;
        public AscendMetricAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPRINumberTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 226;
        public AscendPRINumberTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDialNumberAttribute : StringAttribute
    {
        public const UInt32 Code = 227;
        public AscendDialNumberAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendRouteIPAttribute : IntegerAttribute
    {
        public const UInt32 Code = 228;
        public AscendRouteIPAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendRouteIPXAttribute : IntegerAttribute
    {
        public const UInt32 Code = 229;
        public AscendRouteIPXAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendBridgeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 230;
        public AscendBridgeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendSendAuthAttribute : IntegerAttribute
    {
        public const UInt32 Code = 231;
        public AscendSendAuthAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendSendPasswdAttribute : StringAttribute
    {
        public const UInt32 Code = 232;
        public AscendSendPasswdAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendLinkCompressionAttribute : IntegerAttribute
    {
        public const UInt32 Code = 233;
        public AscendLinkCompressionAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendTargetUtilAttribute : IntegerAttribute
    {
        public const UInt32 Code = 234;
        public AscendTargetUtilAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendMaximumChannelsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 235;
        public AscendMaximumChannelsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendIncChannelCountAttribute : IntegerAttribute
    {
        public const UInt32 Code = 236;
        public AscendIncChannelCountAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDecChannelCountAttribute : IntegerAttribute
    {
        public const UInt32 Code = 237;
        public AscendDecChannelCountAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendSecondsOfHistoryAttribute : IntegerAttribute
    {
        public const UInt32 Code = 238;
        public AscendSecondsOfHistoryAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendHistoryWeighTypeAttribute : IntegerAttribute
    {
        public const UInt32 Code = 239;
        public AscendHistoryWeighTypeAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendAddSecondsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 240;
        public AscendAddSecondsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendRemoveSecondsAttribute : IntegerAttribute
    {
        public const UInt32 Code = 241;
        public AscendRemoveSecondsAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendIdleLimitAttribute : IntegerAttribute
    {
        public const UInt32 Code = 244;
        public AscendIdleLimitAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendPreemptLimitAttribute : IntegerAttribute
    {
        public const UInt32 Code = 245;
        public AscendPreemptLimitAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendCallbackAttribute : IntegerAttribute
    {
        public const UInt32 Code = 246;
        public AscendCallbackAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendDataSvcAttribute : IntegerAttribute
    {
        public const UInt32 Code = 247;
        public AscendDataSvcAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendForce56Attribute : IntegerAttribute
    {
        public const UInt32 Code = 248;
        public AscendForce56Attribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendBillingNumberAttribute : StringAttribute
    {
        public const UInt32 Code = 249;
        public AscendBillingNumberAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendCallByCallAttribute : IntegerAttribute
    {
        public const UInt32 Code = 250;
        public AscendCallByCallAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendTransitNumberAttribute : StringAttribute
    {
        public const UInt32 Code = 251;
        public AscendTransitNumberAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendHostInfoAttribute : StringAttribute
    {
        public const UInt32 Code = 252;
        public AscendHostInfoAttribute(String value)
        {
            Value = value;
        }
    }
    public partial class AscendPPPAddressAttribute : IPAddressAttribute
    {
        public const UInt32 Code = 253;
        public AscendPPPAddressAttribute(IPAddress value)
        {
            Value = value;
        }
    }
    public partial class AscendMPPIdlePercentAttribute : IntegerAttribute
    {
        public const UInt32 Code = 254;
        public AscendMPPIdlePercentAttribute(UInt32 value)
        {
            Value = value;
        }
    }
    public partial class AscendXmitRateAttribute : IntegerAttribute
    {
        public const UInt32 Code = 255;
        public AscendXmitRateAttribute(UInt32 value)
        {
            Value = value;
        }
    }
}