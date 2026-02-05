namespace PepperDash.Plugin.UdmCws;
        // class declarations
         class StateSignalConverter;
         class UdmCWSController;
         class FeedbackMode;
         class UdmCwsConfig;
         class UdmCwsConfiguration;
         class DeviceMapping;
         class RoomStateActions;
         class StandardPropertiesConfig;
         class ReportStateEventArgs;
         class DesiredStateEventArgs;
         class CustomProperties;
         class PropertyKeys;
         class DeviceKeys;
         class DeviceStatus;
         class PatchRequestParser;
         class StandardProperties;
         class State;
         class StatusProperties;
         class UdmCwsActionPathsHandler;
         class WebRequestHandlerBase;
           class ActivityMapping;
           class PropertyMapping;
     class StateSignalConverter 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ExtractDesiredRoomState ( State desiredState );
        STRING_FUNCTION ExtractDesiredRoomActivity ( State desiredState );
        FUNCTION SetDeviceLabel ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetDeviceStatus ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetDeviceDescription ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetDeviceVideoSource ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetDeviceAudioSource ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetDeviceUsage ( SIGNED_LONG_INTEGER index , INTEGER value );
        FUNCTION SetDeviceError ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetCustomLabel ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetCustomValue ( SIGNED_LONG_INTEGER index , STRING value );
        FUNCTION SetStandardVersion ( STRING value );
        FUNCTION SetStandardState ( STRING value );
        FUNCTION SetStandardError ( STRING value );
        FUNCTION SetStandardHelpRequest ( STRING value );
        FUNCTION SetStandardActivity ( STRING value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

    static class UdmCWSController 
    {
        // class delegates
        delegate FUNCTION ReportStateRequestDelegate ( INTEGER dummy );
        delegate FUNCTION RoomStateChangeDelegate ( SIMPLSHARPSTRING desiredState );
        delegate FUNCTION RoomActivityChangeDelegate ( SIMPLSHARPSTRING desiredActivity );

        // class events

        // class functions
        static FUNCTION SetFeedbackMode ( INTEGER mode );
        static FUNCTION SetApiVersion ( SIMPLSHARPSTRING version );
        static FUNCTION SetPsk ( SIMPLSHARPSTRING psk );
        static FUNCTION SetRoutePrefix ( SIMPLSHARPSTRING prefix );
        static FUNCTION Initialize ();
        static FUNCTION Shutdown ();
        static FUNCTION SetDeviceLabel ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetDeviceStatus ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetDeviceDescription ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetDeviceVideoSource ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetDeviceAudioSource ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetDeviceUsage ( INTEGER index , INTEGER value );
        static FUNCTION SetDeviceError ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetCustomLabel ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetCustomValue ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetPropertyLabel ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetPropertyValue ( INTEGER index , SIMPLSHARPSTRING value );
        static FUNCTION SetStandardVersion ( SIMPLSHARPSTRING value );
        static FUNCTION SetStandardState ( SIMPLSHARPSTRING value );
        static FUNCTION SetStandardError ( SIMPLSHARPSTRING value );
        static FUNCTION SetStandardOccupancy ( INTEGER value );
        static FUNCTION SetStandardHelpRequest ( SIMPLSHARPSTRING value );
        static FUNCTION SetStandardActivity ( SIMPLSHARPSTRING value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty ReportStateRequestDelegate ReportStateRequest;
        DelegateProperty RoomStateChangeDelegate RoomStateChange;
        DelegateProperty RoomActivityChangeDelegate RoomActivityChange;
    };

    static class FeedbackMode // enum
    {
        static SIGNED_LONG_INTEGER Deferred;
        static SIGNED_LONG_INTEGER Immediate;
    };

     class UdmCwsConfig 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        FeedbackMode FeedbackMode;
        STRING ApiVersion[];
        STRING Psk[];
    };

     class UdmCwsConfiguration 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ApiVersion[];
        STRING Psk[];
        FeedbackMode FeedbackMode;
        STRING RoutePrefix[];
        RoomStateActions RoomStateActions;
        StandardPropertiesConfig StandardProperties;
    };

     class DeviceMapping 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING DeviceKey[];
        SIGNED_LONG_INTEGER DeviceIndex;
        STRING CustomLabel[];
        STRING Description[];
    };

     class RoomStateActions 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class StandardPropertiesConfig 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Version[];
        STRING RoomDeviceKey[];
        STRING OccupancyDeviceKey[];
        STRING HelpRequestDeviceKey[];
        STRING ActivityDeviceKey[];
        ActivityMapping ActivityMapping;
        PropertyMapping HelpRequestMapping;
    };

     class ReportStateEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        State ReportedState;
    };

     class CustomProperties 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Label[];
        STRING Value[];
    };

    static class PropertyKeys // enum
    {
        static SIGNED_LONG_INTEGER property1;
        static SIGNED_LONG_INTEGER property2;
        static SIGNED_LONG_INTEGER property3;
        static SIGNED_LONG_INTEGER property4;
        static SIGNED_LONG_INTEGER property5;
        static SIGNED_LONG_INTEGER property6;
        static SIGNED_LONG_INTEGER property7;
        static SIGNED_LONG_INTEGER property8;
        static SIGNED_LONG_INTEGER property9;
        static SIGNED_LONG_INTEGER property10;
        static SIGNED_LONG_INTEGER property11;
        static SIGNED_LONG_INTEGER property12;
        static SIGNED_LONG_INTEGER property13;
        static SIGNED_LONG_INTEGER property14;
        static SIGNED_LONG_INTEGER property15;
        static SIGNED_LONG_INTEGER property16;
        static SIGNED_LONG_INTEGER property17;
        static SIGNED_LONG_INTEGER property18;
        static SIGNED_LONG_INTEGER property19;
        static SIGNED_LONG_INTEGER property20;
    };

    static class DeviceKeys // enum
    {
        static SIGNED_LONG_INTEGER device1;
        static SIGNED_LONG_INTEGER device2;
        static SIGNED_LONG_INTEGER device3;
        static SIGNED_LONG_INTEGER device4;
        static SIGNED_LONG_INTEGER device5;
        static SIGNED_LONG_INTEGER device6;
        static SIGNED_LONG_INTEGER device7;
        static SIGNED_LONG_INTEGER device8;
        static SIGNED_LONG_INTEGER device9;
        static SIGNED_LONG_INTEGER device10;
        static SIGNED_LONG_INTEGER device11;
        static SIGNED_LONG_INTEGER device12;
        static SIGNED_LONG_INTEGER device13;
        static SIGNED_LONG_INTEGER device14;
        static SIGNED_LONG_INTEGER device15;
        static SIGNED_LONG_INTEGER device16;
        static SIGNED_LONG_INTEGER device17;
        static SIGNED_LONG_INTEGER device18;
        static SIGNED_LONG_INTEGER device19;
        static SIGNED_LONG_INTEGER device20;
    };

     class DeviceStatus 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Label[];
        STRING Status[];
        STRING Description[];
        STRING VideoSource[];
        STRING AudioSource[];
        STRING Error[];
    };

    static class PatchRequestParser 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class StandardProperties 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Version[];
        STRING State[];
        STRING Error[];
        STRING HelpRequest[];
        STRING Activity[];
    };

     class State 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ApiVersion[];
        StandardProperties Standard;
        StatusProperties Status;
    };

     class StatusProperties 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class UdmCwsActionPathsHandler 
    {
        // class delegates

        // class events
        EventHandler ReportStateEvent ( UdmCwsActionPathsHandler sender, ReportStateEventArgs args );
        EventHandler DesiredStateEvent ( UdmCwsActionPathsHandler sender, DesiredStateEventArgs args );

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        UdmCwsConfig Config;
    };

     class WebRequestHandlerBase 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

namespace PepperDash.Plugin.UdmCws.Configuration;
        // class declarations
         class PropertyMapping;
         class ActivityMapping;
         class CustomPropertyMapping;
     class PropertyMapping 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING DeviceKey[];
        STRING PropertyPath[];
        STRING Format[];
        STRING DefaultValue[];
    };

     class ActivityMapping 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        PropertyMapping GetProperty;
        STRING SetDeviceKey[];
    };

     class CustomPropertyMapping 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING PropertyKey[];
        STRING Label[];
        PropertyMapping Mapping;
    };

