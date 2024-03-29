// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ActorPlayground.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ActorPlayground.Remote {

  /// <summary>Holder for reflection information generated from ActorPlayground.proto</summary>
  public static partial class ActorPlaygroundReflection {

    #region Descriptor
    /// <summary>File descriptor for ActorPlayground.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ActorPlaygroundReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVBY3RvclBsYXlncm91bmQucHJvdG8iIgoDUGlkEg8KB0FkZHJlc3MYASAB",
            "KAkSCgoCSWQYAiABKAkiMwoMTWVzc2FnZUJhdGNoEiMKCWVudmVsb3BlcxgB",
            "IAMoCzIQLk1lc3NhZ2VFbnZlbG9wZSJTCg9NZXNzYWdlRW52ZWxvcGUSFAoM",
            "bWVzc2FnZV90eXBlGAEgASgJEhQKDG1lc3NhZ2VfZGF0YRgCIAEoDBIUCgZz",
            "ZW5kZXIYBCABKAsyBC5QaWQiBgoEVW5pdDIuCglUcmFuc3BvcnQSIQoEU2Vu",
            "ZBIQLk1lc3NhZ2VFbnZlbG9wZRoFLlVuaXQiAEIZqgIWQWN0b3JQbGF5Z3Jv",
            "dW5kLlJlbW90ZWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ActorPlayground.Remote.Pid), global::ActorPlayground.Remote.Pid.Parser, new[]{ "Address", "Id" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ActorPlayground.Remote.MessageBatch), global::ActorPlayground.Remote.MessageBatch.Parser, new[]{ "Envelopes" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ActorPlayground.Remote.MessageEnvelope), global::ActorPlayground.Remote.MessageEnvelope.Parser, new[]{ "MessageType", "MessageData", "Sender" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::ActorPlayground.Remote.Unit), global::ActorPlayground.Remote.Unit.Parser, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Pid : pb::IMessage<Pid> {
    private static readonly pb::MessageParser<Pid> _parser = new pb::MessageParser<Pid>(() => new Pid());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Pid> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pid() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pid(Pid other) : this() {
      address_ = other.address_;
      id_ = other.id_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Pid Clone() {
      return new Pid(this);
    }

    /// <summary>Field number for the "Address" field.</summary>
    public const int AddressFieldNumber = 1;
    private string address_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Address {
      get { return address_; }
      set {
        address_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Id" field.</summary>
    public const int IdFieldNumber = 2;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Pid);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Pid other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Address != other.Address) return false;
      if (Id != other.Id) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Address.Length != 0) hash ^= Address.GetHashCode();
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Address.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Address);
      }
      if (Id.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Id);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Address.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Address);
      }
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Pid other) {
      if (other == null) {
        return;
      }
      if (other.Address.Length != 0) {
        Address = other.Address;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Address = input.ReadString();
            break;
          }
          case 18: {
            Id = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class MessageBatch : pb::IMessage<MessageBatch> {
    private static readonly pb::MessageParser<MessageBatch> _parser = new pb::MessageParser<MessageBatch>(() => new MessageBatch());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MessageBatch> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageBatch() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageBatch(MessageBatch other) : this() {
      envelopes_ = other.envelopes_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageBatch Clone() {
      return new MessageBatch(this);
    }

    /// <summary>Field number for the "envelopes" field.</summary>
    public const int EnvelopesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::ActorPlayground.Remote.MessageEnvelope> _repeated_envelopes_codec
        = pb::FieldCodec.ForMessage(10, global::ActorPlayground.Remote.MessageEnvelope.Parser);
    private readonly pbc::RepeatedField<global::ActorPlayground.Remote.MessageEnvelope> envelopes_ = new pbc::RepeatedField<global::ActorPlayground.Remote.MessageEnvelope>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::ActorPlayground.Remote.MessageEnvelope> Envelopes {
      get { return envelopes_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MessageBatch);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MessageBatch other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!envelopes_.Equals(other.envelopes_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= envelopes_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      envelopes_.WriteTo(output, _repeated_envelopes_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += envelopes_.CalculateSize(_repeated_envelopes_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MessageBatch other) {
      if (other == null) {
        return;
      }
      envelopes_.Add(other.envelopes_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            envelopes_.AddEntriesFrom(input, _repeated_envelopes_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class MessageEnvelope : pb::IMessage<MessageEnvelope> {
    private static readonly pb::MessageParser<MessageEnvelope> _parser = new pb::MessageParser<MessageEnvelope>(() => new MessageEnvelope());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MessageEnvelope> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageEnvelope() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageEnvelope(MessageEnvelope other) : this() {
      messageType_ = other.messageType_;
      messageData_ = other.messageData_;
      sender_ = other.sender_ != null ? other.sender_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MessageEnvelope Clone() {
      return new MessageEnvelope(this);
    }

    /// <summary>Field number for the "message_type" field.</summary>
    public const int MessageTypeFieldNumber = 1;
    private string messageType_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MessageType {
      get { return messageType_; }
      set {
        messageType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "message_data" field.</summary>
    public const int MessageDataFieldNumber = 2;
    private pb::ByteString messageData_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString MessageData {
      get { return messageData_; }
      set {
        messageData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "sender" field.</summary>
    public const int SenderFieldNumber = 4;
    private global::ActorPlayground.Remote.Pid sender_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::ActorPlayground.Remote.Pid Sender {
      get { return sender_; }
      set {
        sender_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MessageEnvelope);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MessageEnvelope other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MessageType != other.MessageType) return false;
      if (MessageData != other.MessageData) return false;
      if (!object.Equals(Sender, other.Sender)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (MessageType.Length != 0) hash ^= MessageType.GetHashCode();
      if (MessageData.Length != 0) hash ^= MessageData.GetHashCode();
      if (sender_ != null) hash ^= Sender.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MessageType.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(MessageType);
      }
      if (MessageData.Length != 0) {
        output.WriteRawTag(18);
        output.WriteBytes(MessageData);
      }
      if (sender_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Sender);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MessageType.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MessageType);
      }
      if (MessageData.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(MessageData);
      }
      if (sender_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Sender);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MessageEnvelope other) {
      if (other == null) {
        return;
      }
      if (other.MessageType.Length != 0) {
        MessageType = other.MessageType;
      }
      if (other.MessageData.Length != 0) {
        MessageData = other.MessageData;
      }
      if (other.sender_ != null) {
        if (sender_ == null) {
          Sender = new global::ActorPlayground.Remote.Pid();
        }
        Sender.MergeFrom(other.Sender);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            MessageType = input.ReadString();
            break;
          }
          case 18: {
            MessageData = input.ReadBytes();
            break;
          }
          case 34: {
            if (sender_ == null) {
              Sender = new global::ActorPlayground.Remote.Pid();
            }
            input.ReadMessage(Sender);
            break;
          }
        }
      }
    }

  }

  public sealed partial class Unit : pb::IMessage<Unit> {
    private static readonly pb::MessageParser<Unit> _parser = new pb::MessageParser<Unit>(() => new Unit());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Unit> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Unit() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Unit(Unit other) : this() {
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Unit Clone() {
      return new Unit(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Unit);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Unit other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Unit other) {
      if (other == null) {
        return;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
