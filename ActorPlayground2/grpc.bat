protoc ActorPlayground.proto -I. -I.. --csharp_out=.\Generated   --csharp_opt=file_extension=.g.cs --grpc_out .\Generated --plugin=protoc-gen-grpc=%USERPROFILE%\.nuget\packages\Grpc.Tools\2.23.0\tools\windows_x64\grpc_csharp_plugin.exe