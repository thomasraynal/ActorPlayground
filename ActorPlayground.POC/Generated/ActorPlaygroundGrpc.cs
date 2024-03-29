// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ActorPlayground.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace ActorPlayground.Remote {
  public static partial class Writer
  {
    static readonly string __ServiceName = "Writer";

    static readonly grpc::Marshaller<global::ActorPlayground.Remote.MessageEnvelope> __Marshaller_MessageEnvelope = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ActorPlayground.Remote.MessageEnvelope.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ActorPlayground.Remote.Unit> __Marshaller_Unit = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ActorPlayground.Remote.Unit.Parser.ParseFrom);

    static readonly grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope> __Method_Send = new grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Send",
        __Marshaller_MessageEnvelope,
        __Marshaller_MessageEnvelope);

    static readonly grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit> __Method_Emit = new grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Emit",
        __Marshaller_MessageEnvelope,
        __Marshaller_Unit);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Writer</summary>
    [grpc::BindServiceMethod(typeof(Writer), "BindService")]
    public abstract partial class WriterBase
    {
      public virtual global::System.Threading.Tasks.Task<global::ActorPlayground.Remote.MessageEnvelope> Send(global::ActorPlayground.Remote.MessageEnvelope request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ActorPlayground.Remote.Unit> Emit(global::ActorPlayground.Remote.MessageEnvelope request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Writer</summary>
    public partial class WriterClient : grpc::ClientBase<WriterClient>
    {
      /// <summary>Creates a new client for Writer</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public WriterClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Writer that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public WriterClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected WriterClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected WriterClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::ActorPlayground.Remote.MessageEnvelope Send(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Send(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ActorPlayground.Remote.MessageEnvelope Send(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Send, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.MessageEnvelope> SendAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return SendAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.MessageEnvelope> SendAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Send, null, options, request);
      }
      public virtual global::ActorPlayground.Remote.Unit Emit(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Emit(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ActorPlayground.Remote.Unit Emit(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Emit, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.Unit> EmitAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return EmitAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.Unit> EmitAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Emit, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override WriterClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new WriterClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(WriterBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Send, serviceImpl.Send)
          .AddMethod(__Method_Emit, serviceImpl.Emit).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, WriterBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Send, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope>(serviceImpl.Send));
      serviceBinder.AddMethod(__Method_Emit, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit>(serviceImpl.Emit));
    }

  }
  public static partial class Reader
  {
    static readonly string __ServiceName = "Reader";

    static readonly grpc::Marshaller<global::ActorPlayground.Remote.MessageEnvelope> __Marshaller_MessageEnvelope = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ActorPlayground.Remote.MessageEnvelope.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::ActorPlayground.Remote.Unit> __Marshaller_Unit = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::ActorPlayground.Remote.Unit.Parser.ParseFrom);

    static readonly grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit> __Method_ReceiveEvent = new grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ReceiveEvent",
        __Marshaller_MessageEnvelope,
        __Marshaller_Unit);

    static readonly grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope> __Method_ReceiveCommand = new grpc::Method<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ReceiveCommand",
        __Marshaller_MessageEnvelope,
        __Marshaller_MessageEnvelope);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::ActorPlayground.Remote.ActorPlaygroundReflection.Descriptor.Services[1]; }
    }

    /// <summary>Base class for server-side implementations of Reader</summary>
    [grpc::BindServiceMethod(typeof(Reader), "BindService")]
    public abstract partial class ReaderBase
    {
      public virtual global::System.Threading.Tasks.Task<global::ActorPlayground.Remote.Unit> ReceiveEvent(global::ActorPlayground.Remote.MessageEnvelope request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::ActorPlayground.Remote.MessageEnvelope> ReceiveCommand(global::ActorPlayground.Remote.MessageEnvelope request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Reader</summary>
    public partial class ReaderClient : grpc::ClientBase<ReaderClient>
    {
      /// <summary>Creates a new client for Reader</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public ReaderClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Reader that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public ReaderClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected ReaderClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected ReaderClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::ActorPlayground.Remote.Unit ReceiveEvent(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReceiveEvent(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ActorPlayground.Remote.Unit ReceiveEvent(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ReceiveEvent, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.Unit> ReceiveEventAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReceiveEventAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.Unit> ReceiveEventAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ReceiveEvent, null, options, request);
      }
      public virtual global::ActorPlayground.Remote.MessageEnvelope ReceiveCommand(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReceiveCommand(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::ActorPlayground.Remote.MessageEnvelope ReceiveCommand(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ReceiveCommand, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.MessageEnvelope> ReceiveCommandAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReceiveCommandAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::ActorPlayground.Remote.MessageEnvelope> ReceiveCommandAsync(global::ActorPlayground.Remote.MessageEnvelope request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ReceiveCommand, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override ReaderClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new ReaderClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(ReaderBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_ReceiveEvent, serviceImpl.ReceiveEvent)
          .AddMethod(__Method_ReceiveCommand, serviceImpl.ReceiveCommand).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, ReaderBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_ReceiveEvent, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.Unit>(serviceImpl.ReceiveEvent));
      serviceBinder.AddMethod(__Method_ReceiveCommand, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::ActorPlayground.Remote.MessageEnvelope, global::ActorPlayground.Remote.MessageEnvelope>(serviceImpl.ReceiveCommand));
    }

  }
}
#endregion
